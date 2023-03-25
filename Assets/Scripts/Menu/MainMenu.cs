using Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
	public class MainMenu : MonoBehaviour
	{
		[Header("Navigation Button")] [SerializeField]
		private Button PlayNavButton;

		[SerializeField] private Button InputNavButton;
		[SerializeField] private Button SkinNavButton;
		[SerializeField] private Button SettingsNavButton;
		[SerializeField] private Button QuitNavButton;

		[Header("Panels")] [SerializeField]
		private GameObject multiplayerPannel;
		[SerializeField] private GameObject inputsPannel;
		[SerializeField] private GameObject skinPannel;
		[SerializeField] private GameObject settingsPannel;

		[Header("Audio")]
		[SerializeField] private AudioManager audioManager;

		#region Trigger

		#region Add/Remove Trigger

		private void AddListener()
		{
			PlayNavButton.onClick.AddListener(OnPlayNavButton);
			InputNavButton.onClick.AddListener(OnInputNavButton);
			SkinNavButton.onClick.AddListener(OnSkinNavButton);
			SettingsNavButton.onClick.AddListener(OnSettingsNavButton);
			QuitNavButton.onClick.AddListener(OnQuitNavButton);
		}

		private void RemoveListener()
		{
			PlayNavButton.onClick.RemoveAllListeners();
			InputNavButton.onClick.RemoveAllListeners();
			SkinNavButton.onClick.RemoveAllListeners();
			SettingsNavButton.onClick.RemoveAllListeners();
			QuitNavButton.onClick.RemoveAllListeners();
		}

		#endregion

		private void OnPlayNavButton()
		{
			if (!multiplayerPannel.activeSelf && !skinPannel.activeSelf && !inputsPannel.activeSelf)
				multiplayerPannel.SetActive(true);
			else
			{
				inputsPannel.SetActive(false);
				skinPannel.SetActive(false);
				settingsPannel.SetActive(false);
				multiplayerPannel.SetActive(true);
			}
		}

		private void OnInputNavButton()
		{
			if (!multiplayerPannel.activeSelf && !skinPannel.activeSelf && !settingsPannel.activeSelf)
				inputsPannel.SetActive(true);
			else
			{
				skinPannel.SetActive(false);
				settingsPannel.SetActive(false);
				multiplayerPannel.SetActive(false);
				inputsPannel.SetActive(true);
			}
		}

		private void OnSkinNavButton()
		{
			if (!multiplayerPannel.activeSelf && !settingsPannel.activeSelf && !inputsPannel.activeSelf)
				skinPannel.SetActive(true);
			else
			{
				inputsPannel.SetActive(false);
				settingsPannel.SetActive(false);
				multiplayerPannel.SetActive(false);
				skinPannel.SetActive(true);
			}
		}

		private void OnSettingsNavButton()
		{
			if (!multiplayerPannel.activeSelf && !skinPannel.activeSelf)
				settingsPannel.SetActive(true);
			else
			{
				multiplayerPannel.SetActive(false);
				skinPannel.SetActive(false);
				settingsPannel.SetActive(true);
			}
		}

		private void OnQuitNavButton()
		{
			audioManager.StopMusic();
			Application.Quit();
		}

		#endregion

		private void Start()
		{
			AddListener();
			if (audioManager.GetActualMusic() != MusicType.MenuMusic)
				audioManager.StartMusic(MusicType.MenuMusic);
		}

		private void OnDestroy()
		{
			RemoveListener();
		}
	}
}