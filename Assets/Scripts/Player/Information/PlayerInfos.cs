using System;
using Mirror;
using UnityEngine;

namespace Player.Information
{

    public class PlayerInfos : MonoBehaviour
    {
        private static PlayerInfos instance;

        #region Speudo

        private string pseudoKey = "pseudo";
        [field: SerializeField] protected string pseudo = "Player Name...";

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

        private string volumeKey = "volume";
        [field: SerializeField] protected float volume = 0;

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

        private string fullscreenKey = "fullscreen";
        [field: SerializeField] protected bool fullscreen = true;

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

        private string resolutionKey = "resolution";
        [field: SerializeField] protected string resolution = "1920x1080";

        public void SetResolution(string newResolution)
        {
            resolution = newResolution;
        }

        public string GetResolution()
        {
            return resolution;
        }

        #endregion

        #region Save/Load Pref

        public void SavePref()
        {
            PlayerPrefs.SetString(pseudoKey, pseudo);
            PlayerPrefs.SetFloat(volumeKey, volume);
            PlayerPrefs.SetInt(fullscreenKey, fullscreen ? 1 : 0);
            PlayerPrefs.SetString(resolutionKey, resolution);
            PlayerPrefs.Save();
        }

        private void loadPref()
        {
            pseudo = PlayerPrefs.GetString(pseudoKey, pseudo);
            volume = PlayerPrefs.GetFloat(volumeKey, volume);
            fullscreen = (PlayerPrefs.GetInt(fullscreenKey, 1) == 1) ? true : false;
            resolution = PlayerPrefs.GetString(resolutionKey, resolution);
        }

        #endregion

        protected virtual void Awake()
        {
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            DontDestroyOnLoad(gameObject);
            loadPref();
        }
    }
}