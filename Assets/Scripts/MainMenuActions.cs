using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Lobbying
{

	public class MainMenuActions : MonoBehaviour
	{

		public static MainMenuActions instance;

		[Header("Room Creation")]
		public Button createButton;
		[SerializeField] TMPro.TMP_InputField newRoomNameIF;
		[SerializeField] TMPro.TMP_InputField newRoomPasswordIF;

		[Header("Room Join")]
		public Button joinButton;

		[Header("Room List")]
		[SerializeField] Transform RoomList;
		[SerializeField] GameObject RoomItemListPrefab;
		//[SerializeField] List<Selectable> roomSelectables = new List<Selectable>();

		private Room selectedRoom;

		void Start()
		{
			instance = this;
			createButton.onClick.AddListener(OnCreateButtonClick);
			joinButton.onClick.AddListener(OnJoinButtonClick);
		}

		void OnCreateButtonClick()
		{
			Debug.Log("Creating a room...");
			Host();
		}

		void OnJoinButtonClick()
		{
			Debug.Log("Joining a room...");
			Join();
		}

		public void Host()
		{
			LocalClient.localClient.HostGame(newRoomNameIF.text, newRoomPasswordIF.text);
		}

		public void HostSuccess(bool success, string roomName, bool roomIsPrivate)
		{
			GameObject newRoom = Instantiate(RoomItemListPrefab, RoomList);
			UIRoom roomUI = newRoom.GetComponent<UIRoom>();
			roomUI.SetRoomUI(roomName, roomIsPrivate);
		}

		private void Join()
		{
			LocalClient.localClient.JoinRoom(selectedRoom.roomName);
		}

		public void JoinSuccess(bool success, string roomName)
		{
			Debug.Log("Room " + roomName + " joined success status:" + success);
			if (success)
			{
			}
			else
			{
			}
		}

	}

}