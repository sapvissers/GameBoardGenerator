/*
	Project	:	Project Falcon
	Author	:	Sven Vissers
	Date	:	2014-09-22
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFalcon.Gameplay.GameBoard.VoronoiBoard
{
	public class TileLogic : MonoBehaviour
	{
		// Center of the tile
		private Vector3 centerPoint;
		public Vector3 CenterPoint { get { return centerPoint; } set { centerPoint = value; } }

		// List of vertices that make the border of the tile
		public List<Vector3> borderVertices;

		public bool IsBorderTile = false;
		public bool IsSelected = false;

		private List<GameObject> adjacentTiles;
		private List<GameObject> areas;

		private AreaType areaType;

		private Color normalColor;
		private Color hoverColor;
		private Color selectedColor;
		private Color adjacentColor;
		private Color adjacentHoverColor;

		#region unity callbacks
		void OnMouseOver()
		{
			if (!CheckIfReady())
			{
				return;
			}

			if (!IsSelected)
			{
				if (CheckIfSelectable())
				{
					renderer.materials[0].color = adjacentHoverColor;
				}
				else
				{
					renderer.materials[0].color = hoverColor;
				}
			}
		}

		void OnMouseExit()
		{
			if (!CheckIfReady())
			{
				return;
			}

			if (!IsSelected)
			{
				if (CheckIfSelectable())
				{
					renderer.materials[0].color = adjacentColor;
				}
				else
				{
					renderer.materials[0].color = normalColor;
				}
			}
		}

		void OnMouseDown()
		{
			if( !CheckIfReady())
			{
				return;
			}

			if (CheckIfSelectable() || IsBorderTile)
			{
				renderer.materials[0].color = selectedColor;
				IsSelected = true;

				for (int i = 0; i < adjacentTiles.Count; i++)
				{
					MeshRenderer adjacentMeshRenderer = adjacentTiles[i].GetComponent<MeshRenderer>();

					if (!adjacentTiles[i].GetComponent<TileLogic>().IsSelected)
					{
						adjacentMeshRenderer.materials[0].color = adjacentColor;
					}
				}

				for (int i = 0; i < areas.Count; i++)
				{
					areas[i].GetComponent<TileLogic>().IsBorderTile = false;
				}
			}
		}
		#endregion

		#region public methods
		/// <summary>
		/// Set all data needed for an area.
		/// </summary>
		/// <param name="areaType">Type of area, not used at the moment</param>
		/// <param name="borderVertices">List of vertices that make the border of this area.</param>
		/// <param name="boardSize">Size of the game board.</param>
		/// /// <param name="areas">All areas on the board.</param>
		public void SetupArea(AreaType areaType, List<Vector3> borderVertices, Vector2 boardSize, List<GameObject> areas)
		{
			this.areaType = areaType;
			this.borderVertices = borderVertices;

			normalColor = GetAreaTypeColor();
			hoverColor = normalColor - new Color(0.2F, 0.2F, 0.2F);
			selectedColor = new Color(1, 1, 1);
			adjacentColor = new Color(0.5F, 0.5F, 0.5F);
			adjacentHoverColor = new Color(0.7F, 0.7F, 0.7F);

			renderer.materials[0].color = normalColor;

			for (int i = 0; i < borderVertices.Count; i++)
			{
				if (borderVertices[i].x == 0 || borderVertices[i].z == 0 ||
					borderVertices[i].x == boardSize.x || borderVertices[i].z == boardSize.y)
				{
					IsBorderTile = true;
				}
			}

			if (areas == null)
			{
				throw new NullReferenceException("areas can't be null");
			}

			this.areas = areas;

			StartCoroutine("FindAdjacentTiles");
		}
		#endregion

		#region private methods
		IEnumerator FindAdjacentTiles()
		{
			List<GameObject> adjacents = new List<GameObject>();

			for (int i = 0; i < areas.Count; i++)
			{
				TileLogic areaTileLogic = null;
				do
				{
					areaTileLogic = areas[i].GetComponent<TileLogic>();
					yield return new WaitForEndOfFrame();
				}
				while (areaTileLogic == null);


				for (int j = 0; j < areaTileLogic.borderVertices.Count; j++)
				{
					for (int k = 0; k < borderVertices.Count; k++)
					{
						if (areaTileLogic.borderVertices[j] == borderVertices[k])
						{
							adjacents.Add(areas[i]);
						}
					}
				}
			}

			adjacentTiles = adjacents;
		}

		private Color GetAreaTypeColor()
		{
			Color color;

			switch (areaType)
			{
				/*  Not implemented yet => provide random color for now
				case AreaType.Farmland:
					color = new Color(1,1,0);
					break;
				case AreaType.Hills:
					color = new Color(0.5f, 1, 0.5f);
					break;
				case AreaType.Mountains:
					color = new Color(0.7f, 0.7f, 0.7f);
					break;
				case AreaType.Swamp:
					color = new Color(0.2f, 0.5f, 0.2f);
					break;
				case AreaType.Forest:
					color = new Color(0.4f, 0.7f, 0.4f);
					break;
				*/
				default:
					color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
					break;
			}

			return color;
		}

		private bool CheckIfSelectable()
		{
			if (adjacentTiles == null || adjacentTiles.Count == 0)
			{
				return false;
			}

			for (int i = 0; i < adjacentTiles.Count; i++)
			{
				if (adjacentTiles[i].GetComponent<TileLogic>().IsSelected)
				{
					return true;
				}
			}

			return false;
		}

		private bool CheckIfReady()
		{
			if (adjacentTiles == null || adjacentTiles.Count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		#endregion
	}
}