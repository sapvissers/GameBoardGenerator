/*
	Project	:	Project Falcon
	Author	:	Sven Vissers
	Date	:	2014-09-22
*/

using csDelaunay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFalcon.Gameplay.GameBoard.VoronoiBoard
{
	public class GameBoardGenerator : MonoBehaviour
	{
		#region public vars
		private int tiles = 50;
		public int Tiles { get { return tiles; } set { tiles = value; } }

		private Vector2 boardSize = new Vector2(50, 50);
		public Vector2 BoardSize { get { return boardSize; } set { boardSize = value; } }

		private int evenDistribution = 2;
		public int EvenDistribution { get { return evenDistribution; } set { evenDistribution = value; } }

		#endregion

		#region private vars
		[SerializeField]
		private float areaBorderSize = 0.2f;
		
		private Dictionary<Vector2f, Site> sites;
		private List<Edge> edges;

		[SerializeField]
		private Material areaMaterial;

		[SerializeField]
		private Material boardEdgeMaterial;

		private List<GameObject> areas;

		private List<List<Vector3>> areasBorderVertices;
		#endregion

		#region unity callbacks
		void Start()
		{
			GenerateBoard(tiles, boardSize);
		}
		#endregion

		#region public methods
		public void GenerateBoard(int tiles, Vector2 boardSize)
		{
			ResetBoard();

			if (tiles == null)
			{
				throw new NullReferenceException("tiles can't be null.");
			}

			if (tiles <= 0)
			{
				throw new NullReferenceException("tiles can't be negative.");
			}

			if (boardSize == null)
			{
				throw new NullReferenceException("boardSize can't be null.");
			}

			if (boardSize.x <= 0 || boardSize.y <= 0)
			{
				throw new NullReferenceException("boardSize can't be negative.");
			}

			this.tiles = tiles;
			this.boardSize = boardSize;

			// Create your sites (lets call that the center of your polygons)
			List<Vector2f> points = CreateRandomPoint();

			// Create the bounds of the voronoi diagram
			Rectf bounds = new Rectf(0, 0, boardSize.x, boardSize.y);

			Voronoi voronoi = new Voronoi(points, bounds, evenDistribution);

			// Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
			sites = voronoi.SitesIndexedByLocation;
			edges = voronoi.Edges;

			int areaIndex = 0;

			foreach (KeyValuePair<Vector2f, Site> site in sites)
			{
				List<Vector3> borderVertices = new List<Vector3>();

				List<Edge> areaEdges = site.Value.Edges;

				foreach (Edge edge in areaEdges)
				{
					if (edge.ClippedEnds == null) continue;

					Vector3 point1 = new Vector3(edge.ClippedEnds[LR.LEFT].x, 0, edge.ClippedEnds[LR.LEFT].y);
					Vector3 point2 = new Vector3(edge.ClippedEnds[LR.RIGHT].x, 0, edge.ClippedEnds[LR.RIGHT].y);

					if (!FindVector3InList(borderVertices, point1))
					{
						borderVertices.Add(point1);
					}

					if (!FindVector3InList(borderVertices, point2))
					{
						borderVertices.Add(point2);
					}
				}

				Vector3 centerVertex = new Vector3(site.Key.x, 0, site.Key.y);

				GameObject area = new Area(borderVertices, centerVertex, areaIndex, boardSize, areaMaterial, areaBorderSize).AreaInstance;

				area.transform.parent = transform;

				areas.Add(area);
				areasBorderVertices.Add(borderVertices);

				areaIndex += 1;
			}

			for (int i = 0; i < areas.Count; i++)
			{
				TileLogic tileLogic = areas[i].GetComponent<TileLogic>();
				tileLogic.SetupArea(AreaType.Swamp, areasBorderVertices[i], boardSize, areas);
			}

			CreateBoardEdge();
		}
		#endregion

		#region private methods
		private List<Vector2f> CreateRandomPoint()
		{
			List<Vector2f> points = new List<Vector2f>();
			for (int i = 0; i < tiles; i++)
			{
				points.Add(new Vector2f(UnityEngine.Random.Range(0, boardSize.x), UnityEngine.Random.Range(0, boardSize.y)));
			}

			return points;
		}

		private bool FindVector3InList(List<Vector3> vectorList, Vector3 findVector)
		{
			for (int i = 0; i < vectorList.Count; i++)
			{
				if (vectorList[i] == findVector)
				{
					return true;
				}
			}

			return false;
		}

		private void CreateBoardEdge()
		{
			GameObject boardEdge = new GameObject("BoardEdge", typeof(MeshFilter), typeof(MeshRenderer));

			MeshFilter meshFilter = boardEdge.GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = boardEdge.GetComponent<MeshRenderer>();
			MeshCollider meshCollider = boardEdge.GetComponent<MeshCollider>();

			Mesh mesh = new Mesh();

			#region Vertex registration
			List<Vector3> vertices = new List<Vector3>();

			vertices.Add(new Vector3(0, -1f, 0));
			vertices.Add(new Vector3(0, 0, 0));
			vertices.Add(new Vector3(-0.9f, 0, -0.9f));
			vertices.Add(new Vector3(-1.4f, -1f, -1.4f));
			vertices.Add(new Vector3(-2.3f, -1.2f, -2.3f));
			vertices.Add(new Vector3(-2.3f, -3f, -2.3f));

			vertices.Add(new Vector3(0, -1f, boardSize.y + 0));
			vertices.Add(new Vector3(0, 0, boardSize.y + 0));
			vertices.Add(new Vector3(-0.9f, 0, boardSize.y + 0.9f));
			vertices.Add(new Vector3(-1.4f, -1f, boardSize.y + 1.4f));
			vertices.Add(new Vector3(-2.3f, -1.2f, boardSize.y + 2.3f));
			vertices.Add(new Vector3(-2.3f, -3f, boardSize.y + 2.3f));

			vertices.Add(new Vector3(boardSize.x + 0, -1f, boardSize.y + 0));
			vertices.Add(new Vector3(boardSize.x + 0, 0, boardSize.y + 0));
			vertices.Add(new Vector3(boardSize.x + 0.9f, 0, boardSize.y + 0.9f));
			vertices.Add(new Vector3(boardSize.x + 1.4f, -1f, boardSize.y + 1.4f));
			vertices.Add(new Vector3(boardSize.x + 2.3f, -1.2f, boardSize.y + 2.3f));
			vertices.Add(new Vector3(boardSize.x + 2.3f, -3f, boardSize.y + 2.3f));

			vertices.Add(new Vector3(boardSize.x + 0, -1f, 0));
			vertices.Add(new Vector3(boardSize.x + 0, 0, 0));
			vertices.Add(new Vector3(boardSize.x + 0.9f, 0, -0.9f));
			vertices.Add(new Vector3(boardSize.x + 1.4f, -1f, -1.4f));
			vertices.Add(new Vector3(boardSize.x + 2.3f, -1.2f, -2.3f));
			vertices.Add(new Vector3(boardSize.x + 2.3f, -3f, -2.3f));
			#endregion

			#region Triangle registration
			List<int> triangles = new List<int>();
			// top cap
			triangles.Add(0);
			triangles.Add(6);
			triangles.Add(12);

			triangles.Add(12);
			triangles.Add(18);
			triangles.Add(0);

			// side triangles
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (j == 3)
					{
						triangles.Add(i + (j * 6));
						triangles.Add(i + (j * 6) + 1);
						triangles.Add(i + 1);

						triangles.Add(i + 1);
						triangles.Add(i);
						triangles.Add(i + (j * 6));
					}
					else
					{
						triangles.Add(i + (j * 6));
						triangles.Add(i + (j * 6) + 1);
						triangles.Add(i + (j * 6) + 7);

						triangles.Add(i + (j * 6) + 7);
						triangles.Add(i + (j * 6) + 6);
						triangles.Add(i + (j * 6));
					}
				}
			}

			// bottom cap
			triangles.Add(11);
			triangles.Add(5);
			triangles.Add(23);

			triangles.Add(23);
			triangles.Add(17);
			triangles.Add(11);
			#endregion

			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();

			meshRenderer.material = boardEdgeMaterial;

			mesh.RecalculateNormals();

			meshFilter.mesh = mesh;

			boardEdge.transform.position = new Vector3(0, 0.5f, 0);

			boardEdge.transform.parent = transform;
		}

		private void ResetBoard()
		{
			areas = new List<GameObject>();

			areasBorderVertices = new List<List<Vector3>>();

			var children = new List<GameObject>();
			foreach (Transform child in transform) children.Add(child.gameObject);
			children.ForEach(child => Destroy(child));
		}
		#endregion
	}
}