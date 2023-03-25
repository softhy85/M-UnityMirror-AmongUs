using Audio;
using Mirror;
using Network;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Menu {
	public class PauseMenu : NetworkBehaviour
	{
		[Header("Navigation Button")]
		[SerializeField] private Button ContinueButton;
		[SerializeField] private Button DisconnectButton;

		[Header("Pause Menu")] [SerializeField]
		private GameObject pauseMenu;

		private MenuController menuController;

		private RoomManager roomManager;

		private AudioManager audioManager;

		private bool isOpen = false;

		#region Trigger

		#region Add/Remove Trigger

		private void AddListener()
		{
			ContinueButton.onClick.AddListener(OnContinueButton);
			DisconnectButton.onClick.AddListener(OnDisconnectButton);
			menuController.Menu.Pause.started += OnTriggerPause;
		}

		private void RemoveListener()
		{
			ContinueButton.onClick.RemoveAllListeners();
			DisconnectButton.onClick.RemoveAllListeners();
			menuController.Menu.Pause.started -= OnTriggerPause;
		}

		#endregion

		private void OnContinueButton()
		{
			ClosePauseMenu();
		}

		private void OnDisconnectButton()
		{
			audioManager.StopMusic();
			if (isClientOnly)
			{
				roomManager.StopClient();
			} else {
				roomManager.StopHost();
			}
		}

		private void OnTriggerPause(InputAction.CallbackContext ctx)
		{
			if (isOpen)
				ClosePauseMenu();
			else
				OpenPauseMenu();
		}

		#endregion

		private void OpenPauseMenu()
		{

			isOpen = true;
			pauseMenu.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
		}

		private void ClosePauseMenu()
		{
			isOpen = false;
			pauseMenu.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
		}

		public bool IsOpen()
		{
			return isOpen;
		}

		private void Start()
		{
			menuController = new MenuController();
			menuController.Menu.Enable();
			roomManager = (RoomManager)NetworkManager.singleton;
			var audioManagers =
				GameObject.FindGameObjectsWithTag("AudioManager");
			if (audioManagers.Length == 1)
			{
				if (audioManagers[0]
				    .TryGetComponent<AudioManager>(out var actAudioManager))
				{
					audioManager = actAudioManager;
				}
			}
			AddListener();
			ClosePauseMenu();
		}

		private void OnDestroy()
		{
			RemoveListener();
			Cursor.lockState = CursorLockMode.None;
		}
	}
}