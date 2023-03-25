using System;
using Mirror;
using UnityEngine;
using UnityEngine.Audio;

namespace Player.Information
{

    public class PlayerInfos : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private string pseudo = "Player Name...";
        [SerializeField] private float volume = 0;
        [SerializeField] private bool fullscreen = true;
        [SerializeField] private string resolution = "1920x1080";

        private string pseudoKey = "pseudo";
        private string volumeKey = "volume";
        private string fullscreenKey = "fullscreen";
        private string resolutionKey = "resolution";

        [Header("Slime Pref")]
        [SerializeField] private int slimeType = 0;
        [SerializeField] private int slimeHat = 0;
        [SerializeField] private Color slimeColor = new Color(0, 0, 0, 0);

        private string slimeTypeKey = "slime type";
        private string slimeHatKey = "slime hat";
        private string slimeColorRKey = "slime color r";
        private string slimeColorGKey = "slime color g";
        private string slimeColorBKey = "slime color b";

        [Header("Monster Pref")]
        [SerializeField] private int monsterType = 0;
        [SerializeField] private Color monsterColor = new Color(0, 0, 0, 0);

        private string monsterTypeKey = "monster type";
        private string monsterColorRKey = "monster color r";
        private string monsterColorGKey = "monster color g";
        private string monsterColorBKey = "monster color b";

        [Header("Audio")]
        [SerializeField] private AudioMixer audioMixer;

        private static PlayerInfos instance;


        #region Pseudo

        public void SetPseudo(string newPseudo)
        {
            pseudo = newPseudo;
        }

        public string GetPseudo()
        {
            return pseudo;
        }

        #endregion

        #region Volume

        public void SetVolume(float newVolume)
        {
            volume = newVolume;
        }

        public float GetVolume()
        {
            return volume;
        }

        #endregion

        #region Fullscreen

        public void SetFullscreen(bool newFullscreen)
        {
            fullscreen = newFullscreen;
        }

        public bool GetFullscreen()
        {
            return fullscreen;
        }

        #endregion

        #region Resolution

        public void SetResolution(string newResolution)
        {
            resolution = newResolution;
        }

        public string GetResolution()
        {
            return resolution;
        }

        #endregion

        #region Slime Type

        public void SetSlimeType(int newSlimeType)
        {
            slimeType = newSlimeType;
        }

        public int GetSlimeType()
        {
            return slimeType;
        }

        #endregion

        #region Slime hat

        public void SetSlimeHat(int newSlimeHat)
        {
            slimeHat = newSlimeHat;
        }

        public int GetSlimeHat()
        {
            return slimeHat;
        }

        #endregion

        #region Slime Color

        public void SetSlimeColor(Color newSlimeColor)
        {
            slimeColor = newSlimeColor;
        }

        public Color GetSlimeColor()
        {
            return slimeColor;
        }

        #endregion

        #region Monster Type

        public void SetMonsterType(int newMonsterType)
        {
            monsterType = newMonsterType;
        }

        public int GetMonsterType()
        {
            return monsterType;
        }


        #endregion

        #region Monster Color

        public void SetMonsterColor(Color newMonsterColor)
        {
            monsterColor = newMonsterColor;
        }

        public Color GetMonsterColor()
        {
            return monsterColor;
        }

        #endregion

        #region Save/Load Pref

        public void SavePref()
        {
            PlayerPrefs.SetString(pseudoKey, pseudo);
            PlayerPrefs.SetFloat(volumeKey, volume);
            PlayerPrefs.SetInt(fullscreenKey, fullscreen ? 1 : 0);
            PlayerPrefs.SetString(resolutionKey, resolution);

            PlayerPrefs.SetInt(slimeTypeKey, slimeType);
            PlayerPrefs.SetInt(slimeHatKey, slimeHat);
            PlayerPrefs.SetFloat(slimeColorRKey, slimeColor.r);
            PlayerPrefs.SetFloat(slimeColorGKey, slimeColor.g);
            PlayerPrefs.SetFloat(slimeColorBKey, slimeColor.b);

            PlayerPrefs.SetInt(monsterTypeKey, monsterType);
            PlayerPrefs.SetFloat(monsterColorRKey, monsterColor.r);
            PlayerPrefs.SetFloat(monsterColorGKey, monsterColor.g);
            PlayerPrefs.SetFloat(monsterColorBKey, monsterColor.b);

            PlayerPrefs.Save();
        }

        private void loadPref()
        {
            pseudo = PlayerPrefs.GetString(pseudoKey, pseudo);
            volume = PlayerPrefs.GetFloat(volumeKey, volume);
            fullscreen = (PlayerPrefs.GetInt(fullscreenKey, 1) == 1) ? true : false;
            resolution = PlayerPrefs.GetString(resolutionKey, resolution);

            audioMixer.SetFloat("Main Volume", volume);
            Screen.fullScreen = fullscreen;
            var temp = resolution.Split("x");
            if (temp.Length == 2) {
                var width = int.Parse(temp[0]);
                var height = int.Parse(temp[1]);
                Screen.SetResolution(width, height, Screen.fullScreen);
            }

            slimeType = PlayerPrefs.GetInt(slimeTypeKey, slimeType);
            slimeHat = PlayerPrefs.GetInt(slimeHatKey, slimeHat);
            var tempR = PlayerPrefs.GetFloat(slimeColorRKey, slimeColor.r);
            var tempG = PlayerPrefs.GetFloat(slimeColorGKey, slimeColor.g);
            var tempB = PlayerPrefs.GetFloat(slimeColorBKey, slimeColor.b);
            slimeColor = new Color(tempR, tempG, tempB);

            monsterType = PlayerPrefs.GetInt(monsterTypeKey, monsterType);
            tempR = PlayerPrefs.GetFloat(monsterColorRKey, monsterColor.r);
            tempG = PlayerPrefs.GetFloat(monsterColorGKey, monsterColor.g);
            tempB = PlayerPrefs.GetFloat(monsterColorBKey, monsterColor.b);
            monsterColor = new Color(tempR, tempG, tempB);
        }

        #endregion

        private void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            loadPref();
        }
    }
}