/*
	Project	:	Project Falcon
	Author	:	Sven Vissers
	Date	:	2014-09-22
*/

using UnityEngine;
using System.Collections;
using ProjectFalcon.Gameplay.GameBoard.VoronoiBoard;

namespace ProjectFalcon.Gameplay.GameBoard
{
	public class CameraRig : MonoBehaviour
	{
		#region private vars
		[SerializeField]
		private GameBoardGenerator gameBoardGenerator;

		[SerializeField]
		private GameObject rotator;

		[SerializeField]
		private Camera camera;

		private Vector2 boardSize;

		private float rotHorizontal;
		private float rotVertical;
		private float zoom;
		#endregion

		#region unity callbacks
		void Awake()
		{
			boardSize = gameBoardGenerator.BoardSize;

			rotHorizontal = rotator.transform.localEulerAngles.y;
			rotVertical = rotator.transform.localEulerAngles.x;
			zoom = camera.fieldOfView;
		}
		
		void Update()
		{
			if (boardSize != gameBoardGenerator.BoardSize)
			{
				boardSize = gameBoardGenerator.BoardSize;

				transform.position = new Vector3(boardSize.x / 2, 0, boardSize.y / 2);
			}

			if(Input.GetMouseButton(2))
			{
				if(Input.GetAxis("Mouse Y") != 0)
				{
					rotHorizontal += Input.GetAxis("Mouse X") * 5;

					rotator.transform.localEulerAngles = new Vector3(rotVertical % 360, rotHorizontal % 360, 0);
				}

				if (Input.GetAxis("Mouse X") != 0)
				{
					rotVertical -= Input.GetAxis("Mouse Y") * 5;
					rotator.transform.localEulerAngles = new Vector3(rotVertical % 360, rotHorizontal % 360, 0);
				}
			}

			if (Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				zoom -= Input.GetAxis("Mouse ScrollWheel") * 5;

				if (zoom < 1)
				{
					zoom = 1;
				}

				camera.fieldOfView = zoom;
			}
		}
		#endregion
	}
}