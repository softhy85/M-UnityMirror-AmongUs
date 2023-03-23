using System;
using Mirror;
using UnityEngine;

namespace Player.Information
{

    public class PlayerInfos : MonoBehaviour
    {
        public static PlayerInfos instance;

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

        public void SavePref()
        {
            PlayerPrefs.SetString(pseudoKey, pseudo);
            PlayerPrefs.Save();
        }

        private void loadPref()
        {
            pseudo = PlayerPrefs.GetString(pseudoKey, pseudo);
        }

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