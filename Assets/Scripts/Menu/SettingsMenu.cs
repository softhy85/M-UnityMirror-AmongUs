using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;
using Player.Information;
using TMPro;


namespace Menu
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Audio")] [SerializeField] private AudioMixer audioMixer;

        [Header("UI Inputs")] [SerializeField]
        private TMP_Dropdown resolutionDropdown;

        [SerializeField] private Toggle fullScreenToggle;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private TMP_InputField pseudoInput;
        [SerializeField] private Button closeButton;

        [Header("Panel")] [SerializeField] private GameObject settingsPanel;

        [Header("Player")] [SerializeField] private PlayerInfos playerInfos;

        private Resolution[] resolutions;

        #region Trigger

        #region Add/Remove Trigger

        private void AddListener()
        {
            resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdown);
            volumeSlider.onValueChanged.AddListener(OnVolumeSlider);
            fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggle);
            pseudoInput.onValueChanged.AddListener(OnPseudoInput);
            closeButton.onClick.AddListener(OncloseButton);
        }

        private void RemoveListener()
        {
            resolutionDropdown.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.RemoveAllListeners();
            fullScreenToggle.onValueChanged.RemoveAllListeners();
            pseudoInput.onValueChanged.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        #endregion

        private void OnResolutionDropdown(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            playerInfos.SetResolution(
                resolution.width + "x" + resolution.height);

            Screen.SetResolution(resolution.width, resolution.height,
                Screen.fullScreen);
        }

        private void OnVolumeSlider(float volume)
        {
            audioMixer.SetFloat("Main Volume", volume);
            playerInfos.SetVolume(volume);
        }

        private void OnFullScreenToggle(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;
            playerInfos.SetFullscreen(isFullScreen);
        }

        private void OnPseudoInput(string newPseudo)
        {
            playerInfos.SetPseudo(newPseudo);
        }

        private void OncloseButton()
        {
            settingsPanel.SetActive(false);
        }

        #endregion

        private void Start()
        {
            InitializeResolution();
            InitializeFullscreen();
            InitializeVolumeSlider();
            InitializePseudoInput();
        }

        private void OnEnable()
        {
            AddListener();
        }

        private void OnDisable()
        {
            RemoveListener();
            playerInfos.SavePref();
        }

        private void OnDestroy()
        {
            RemoveListener();
            playerInfos.SavePref();
        }

        private void InitializeResolution()
        {
            var saveResolution = playerInfos.GetResolution();
            int saveResolutionIt = -1;
            resolutions = Screen.resolutions.Select(resolution => new Resolution
                    { width = resolution.width, height = resolution.height })
                .Distinct().ToArray();
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option =
                    resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (option == saveResolution)
                {
                    saveResolutionIt = i;
                }

                if (resolutions[i].width == Screen.width &&
                    resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = (saveResolutionIt != -1)
                ? saveResolutionIt
                : currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void InitializeVolumeSlider()
        {
            volumeSlider.minValue = -80;
            volumeSlider.maxValue = 0;
            volumeSlider.value = playerInfos.GetVolume();
            audioMixer.SetFloat("Main Volume", volumeSlider.value);
        }

        private void InitializeFullscreen()
        {
            fullScreenToggle.isOn = playerInfos.GetFullscreen();
            Screen.fullScreen = fullScreenToggle.isOn;
        }

        private void InitializePseudoInput()
        {
            pseudoInput.text = playerInfos.GetPseudo();
        }
    }
}