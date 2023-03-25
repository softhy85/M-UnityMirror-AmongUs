using Mirror;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
	public class SimpleMultiMenu : MonoBehaviour
	{
		[Header("UI Input")] [SerializeField] private TMP_InputField IPInput;

		// [SerializeField] private TMP_InputField PortInput;
		[SerializeField] private Button HostButton;
		[SerializeField] private Button JoinButton;
		[SerializeField] private Button QuitButton;

		[Header("Panels")] [SerializeField]
		private GameObject multiplayerPannel;

		[Header("Multiplayer")] [SerializeField]
		private RoomManager roomManager;

		[SerializeField] private Transport transport;

		#region Trigger

		#region Add/Remove Trigger

		private void AddListener()
		{
			HostButton.onClick.AddListener(OnHostButton);
			JoinButton.onClick.AddListener(OnJoinButton);
			QuitButton.onClick.AddListener(OnQuitNavButton);
		}

		private void RemoveListener()
		{
			HostButton.onClick.RemoveAllListeners();
			JoinButton.onClick.RemoveAllListeners();
			QuitButton.onClick.RemoveAllListeners();
		}

		#endregion

		private void OnHostButton()
		{
			roomManager.StartHost();
		}

		private void OnJoinButton()
		{
			roomManager.networkAddress = IPInput.text;
			roomManager.StartClient();
		}

		private void OnQuitNavButton()
		{
			multiplayerPannel.SetActive(false);
		}

		#endregion

		private void OnEnable()
		{
			AddListener();
			IPInput.text = "localhost";
		}

		private void OnDisable()
		{
			RemoveListener();
		}

		private void OnDestroy()
		{
			RemoveListener();
		}
	}
}