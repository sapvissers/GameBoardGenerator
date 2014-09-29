/*
	Project	:	Project Falcon
	Author	:	Sven Vissers
	Date	:	2014-09-22
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFalcon.Gameplay.GameBoard.VoronoiBoard
{
	public class Area
	{
		public List<Vector3> Vertices;
		public List<int> Triangles;

		public bool IsCornerArea = false;

		public GameObject AreaInstance;

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;

		private Vector2 boardSize;

		private float areaBorderSize;

		private Vector3 centerVertex;

		private List<Vector3> borderVertices;
		public List<Vector3> BorderVertices { get { return borderVertices; } }

		private List<Vector3> innerBorderVertices;

		private Mesh mesh;

		public Area(List<Vector3> unsortedBorderVertices, Vector3 centerVertex, int areaIndex, Vector2 boardSize, Material material, float areaBorderSize)
		{
			this.centerVertex = centerVertex;

			borderVertices = unsortedBorderVertices;

			this.boardSize = boardSize;

			this.areaBorderSize = areaBorderSize;

			AreaInstance = new GameObject("Area" + areaIndex, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider), typeof(TileLogic));

			MeshFilter meshFilter = AreaInstance.GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = AreaInstance.GetComponent<MeshRenderer>();
			MeshCollider meshCollider = AreaInstance.GetComponent<MeshCollider>();
			TileLogic tileLogic = AreaInstance.GetComponent<TileLogic>();

			meshRenderer.material = material;

			tileLogic.CenterPoint = centerVertex;

			mesh = new Mesh();

			Vertices = new List<Vector3>();

			Vertices.Add(centerVertex);

			GenerateCornerVertex();

			borderVertices = SortVertices(borderVertices);

			Vertices.AddRange(borderVertices);

			GenerateInnerBorderVertices();

			Vertices.AddRange(innerBorderVertices);
			

			Triangles = new List<int>();


			// Generate outer triangles
			for (int i = 1; i < borderVertices.Count + 1; i++)
			{
				Vertices[i] = new Vector3(Vertices[i].x, -0.1f, Vertices[i].z);

				if (i + 1 <= borderVertices.Count)
				{
					Triangles.Add(i + innerBorderVertices.Count);
					Triangles.Add(i);
					Triangles.Add(i + 1);
					
					Triangles.Add(i + 1);
					Triangles.Add(i + innerBorderVertices.Count + 1);
					Triangles.Add(i + innerBorderVertices.Count);
				}
				else
				{
					Triangles.Add(i + innerBorderVertices.Count);
					Triangles.Add(i);
					Triangles.Add(1);

					Triangles.Add(1);
					Triangles.Add(innerBorderVertices.Count + 1);
					Triangles.Add(i + innerBorderVertices.Count);
				}
			}

			// Generate inner triangles
			for (int i = 1; i < innerBorderVertices.Count + 1; i++)
			{
				Triangles.Add(0);
				Triangles.Add(i + innerBorderVertices.Count);

				if (i + 1 < innerBorderVertices.Count + 1)
				{
					Triangles.Add(i + innerBorderVertices.Count + 1);
				}
				else
				{
					Triangles.Add(1 + innerBorderVertices.Count);
				}
			}
						
			mesh.vertices = Vertices.ToArray();
			mesh.triangles = Triangles.ToArray();

			mesh.RecalculateNormals();

			meshFilter.mesh = mesh;

			meshCollider.sharedMesh = mesh;
		}

		private void GenerateCornerVertex()
		{
			Vector3 cornerVertex;

			List<int> borders = new List<int>();

			for (int i = 0; i < borderVertices.Count; i++)
			{
				if (borderVertices[i].x == 0)
				{
					borders.Add(1);
				}
				if (borderVertices[i].z == boardSize.y)
				{
					borders.Add(2);
				}
				if (borderVertices[i].x == boardSize.x)
				{
					borders.Add(3);
				}
				if (borderVertices[i].z == 0)
				{
					borders.Add(4);
				}
			}

			if (borders.Count == 2)
			{
				if (borders[0] == borders[1])
				{
					return;
				}

				if (borders[0] + 2 == borders[1] || borders[0] - 2 == borders[1])
				{
					Debug.LogError("Unable to create corner vertex, borders are touching oposite sides of the board.");
					return;
				}

				float x = 0;
				float y = 0;

				if (borders[0] == 1 || borders[1] == 1)
				{
					x = 0;
				}
				else if (borders[0] == 3 || borders[1] == 3)
				{
					x = boardSize.x;
				}

				if (borders[0] == 2 || borders[1] == 2)
				{
					y = boardSize.y;
				}
				else if (borders[0] == 4 || borders[1] == 4)
				{
					y = 0;
				}

				cornerVertex = new Vector3(x, 0, y);

				borderVertices.Add(cornerVertex);

			}
		}

		private List<Vector3> SortVertices(List<Vector3> vertices)
		{
			if (centerVertex == null)
			{
				throw new NullReferenceException("centerVertex can't be null");
			}

			if (vertices == null)
			{
				throw new NullReferenceException("centerVertex can't be null");
			}

			List<Vector3> sortedVertices = new List<Vector3>();
			List<float> sortedAngles = new List<float>();

			sortedVertices.Add(vertices[0]);
			sortedAngles.Add(GetAngle(vertices[0]));

			for (int i = 1; i < vertices.Count; i++)
			{
				float angle = GetAngle(vertices[i]);

				int determinedIndex = DetermineIndex(sortedAngles, angle);

				if (determinedIndex == sortedVertices.Count)
				{
					sortedVertices.Add(vertices[i]);
					sortedAngles.Add(angle);
				}
				else
				{
					sortedVertices.Insert(determinedIndex, vertices[i]);
					sortedAngles.Insert(determinedIndex, angle);
				}
			}

			return sortedVertices;
		}

		private float GetAngle(Vector3 vertex)
		{
			double horizontal = vertex.x - centerVertex.x;
			double vertical = vertex.z - centerVertex.z;

			double angle = 0;

			if (horizontal < 0 && vertical > 0)
			{
				angle = Math.Atan2(-horizontal, vertical) * Mathf.Rad2Deg;
				angle = 90 - angle;
			}
			else if (horizontal > 0 && vertical > 0)
			{
				angle = Math.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
				angle = 90 - angle;
				angle += 90;
			}
			else if (horizontal > 0 && vertical < 0)
			{
				angle = Math.Atan2(horizontal, -vertical) * Mathf.Rad2Deg;
				angle = 90 - angle;
				angle += 180;
			}
			else
			{
				angle = Math.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
				angle = 90 - angle;
				angle += 270;
			}

			return (float)angle;
		}

		private int DetermineIndex(List<float> sortedList, float angle)
		{
			for (int i = 0; i < sortedList.Count; i++)
			{
				if (angle < sortedList[i])
				{
					return i;
				}
			}

			return sortedList.Count;
		}

		private void GenerateInnerBorderVertices()
		{
			innerBorderVertices = borderVertices;

			for(int i = 0; i < innerBorderVertices.Count; i++)
			{
				Vector3 borderVertex = innerBorderVertices[i];

				float totalWidth = borderVertex.x - centerVertex.x;
				float totalHeight = borderVertex.z - centerVertex.z;
				float totalDistance = Mathf.Sqrt((Mathf.Pow(totalHeight, 2) + Mathf.Pow(totalWidth, 2)));

				float scale = areaBorderSize / totalDistance;

				Vector3 innerCoords = new Vector3(borderVertex.x - (totalWidth * scale), 0, borderVertex.z - (totalHeight * scale));

				innerBorderVertices[i] = innerCoords;
			}
		}
	}
}