/*
	Project	:	Project Falcon
	Author	:	Sven Vissers
	Date	:	2014-09-22
*/

using ProjectFalcon.Gameplay.GameBoard.VoronoiBoard;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ProjectFalcon.Gameplay.GameBoard
{
	public class BoardSetupPanel : MonoBehaviour
	{
		[SerializeField]
		private GameBoardGenerator generator;

		[SerializeField]
		private string HelpText;

		private int tiles = 50;
		private string tilesString = "";

		private Vector2 boardSize = new Vector2(50, 50);
		private string boardSizeX = "";
		private string boardSizeY = "";

		private bool showHelp = false;

		void Awake()
		{
			tilesString = tiles.ToString();

			boardSizeX = boardSize.x.ToString();
			boardSizeY = boardSize.y.ToString();
		}

		void OnGUI()
		{
			UnityEngine.GUI.Window(0, new Rect(10, 10, 150, 180), SetupPanelWindow, "Board setup panel");

			if (UnityEngine.GUI.Button(new Rect(Screen.width - 50, 0, 50, 50), "Help"))
			{
				showHelp = !showHelp;
			}

			if (showHelp)
			{
				UnityEngine.GUI.Window(1, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 200, 300), HelpWindow, "Help");
			}
		}

		void Update()
		{
			if (tiles.ToString() != tilesString)
			{
				tilesString = Regex.Replace(tilesString, "[^0-9]", "");

				if (tilesString == "")
				{
					tilesString = tiles.ToString();
				}

				if (float.Parse(tilesString) > 0 && tilesString != "")
				{
					tiles = int.Parse(tilesString);
				}
				else
				{
					tilesString = tiles.ToString();
				}
			}

			if (boardSize.x.ToString() != boardSizeX)
			{
				boardSizeX = Regex.Replace(boardSizeX, "[^0-9]", "");

				if (boardSizeX == "")
				{
					boardSizeX = boardSize.x.ToString();
				}

				if (float.Parse(boardSizeX) > 0 && boardSizeX != "")
				{
					boardSize.x = float.Parse(boardSizeX);
				}
				else
				{
					boardSizeX = boardSize.x.ToString();
				}
			}

			if (boardSize.y.ToString() != boardSizeY)
			{
				boardSizeY = Regex.Replace(boardSizeY, "[^0-9]", "");

				if (boardSizeY == "")
				{
					boardSizeY = boardSize.y.ToString();
				}

				if (float.Parse(boardSizeY) > 0)
				{
					boardSize.y = float.Parse(boardSizeY);
				}
				else
				{
					boardSizeY = boardSize.y.ToString();
				}
			}
		}

		private void SetupPanelWindow(int ID)
		{
			UnityEngine.GUI.Label(new Rect(10, 30, 130, 20), "Number of tiles: ");
			tilesString = UnityEngine.GUI.TextField(new Rect(10, 50, 30, 20), tilesString);

			UnityEngine.GUI.Label(new Rect(10, 80, 80, 20), "Board size: ");
			UnityEngine.GUI.Label(new Rect(10, 100, 40, 20), "X: ");
			boardSizeX = UnityEngine.GUI.TextField(new Rect(40, 100, 30, 20), boardSizeX);
			UnityEngine.GUI.Label(new Rect(10, 120, 40, 20), "Y: ");
			boardSizeY = UnityEngine.GUI.TextField(new Rect(40, 120, 30, 20), boardSizeY);

			if (UnityEngine.GUI.Button(new Rect(10, 150, 130, 20), "Generate"))
			{
				generator.GenerateBoard(tiles, boardSize);
			}
		}

		private void HelpWindow(int ID)
		{
			UnityEngine.GUI.Label(new Rect(10, 30, 180, 230), HelpText);

			if (UnityEngine.GUI.Button(new Rect(140, 270, 50, 20), "Close"))
			{
				showHelp = false;
			}
		}
	}
}