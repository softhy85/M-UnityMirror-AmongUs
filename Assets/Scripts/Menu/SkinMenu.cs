using System.Collections.Generic;
using Player.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class SkinMenu : MonoBehaviour
    {
        [Header("UI Inputs")] [SerializeField]
        private TMP_Dropdown slimeTypeDropdown;

        [SerializeField] private TMP_Dropdown slimeHatDropdown;
        [SerializeField] private FlexibleColorPicker slimeColor;
        [SerializeField] private TMP_Dropdown monsterTypeDropdown;
        [SerializeField] private FlexibleColorPicker monsterColor;
        [SerializeField] private Button closeButton;

        [Header("Panel")] [SerializeField] private GameObject skinPanel;

        [Header("Player")] [SerializeField] private PlayerInfos playerInfos;

        #region Trigger

        #region Add/Remove Trigger

        private void AddListener()
        {
            slimeTypeDropdown.onValueChanged.AddListener(OnSlimeTypeDropdown);
            slimeHatDropdown.onValueChanged.AddListener(OnSlimeHatDropdown);
            slimeColor.onColorChange.AddListener(OnSlimeColor);

            monsterTypeDropdown.onValueChanged.AddListener(
                OnMonsterTypeDropdown);
            monsterColor.onColorChange.AddListener(OnMonsterColor);
            closeButton.onClick.AddListener(OncloseButton);
        }

        private void RemoveListener()
        {
            slimeTypeDropdown.onValueChanged.RemoveAllListeners();
            slimeHatDropdown.onValueChanged.RemoveAllListeners();

            monsterTypeDropdown.onValueChanged.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }

        #endregion

        private void OnSlimeTypeDropdown(int newSlimeType)
        {
            playerInfos.SetSlimeType(newSlimeType);
        }

        private void OnSlimeHatDropdown(int newSlimeHat)
        {
            playerInfos.SetSlimeHat(newSlimeHat);
        }

        private void OnSlimeColor(Color newSlimeColor)
        {
            playerInfos.SetSlimeColor(newSlimeColor);
        }


        private void OnMonsterTypeDropdown(int newMonsterType)
        {
            playerInfos.SetMonsterType(newMonsterType);
        }

        private void OnMonsterColor(Color newMonsterColor)
        {
            playerInfos.SetMonsterColor(newMonsterColor);
        }

        private void OncloseButton()
        {
            skinPanel.SetActive(false);
        }

        #endregion

        private void Start()
        {
        }

        private void OnEnable()
        {
            AddListener();
            InitializeSlime();
            InitializeMonster();
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

        private void InitializeSlime()
        {
            var slimeType = new List<string>
                { "normal slime", "bunny slime", "cat slime" };
            var slimeHat = new List<string>
                { "none", "king", "metal helmet", "viking", "leaf", "sprout" };
            var actualSlimeType = playerInfos.GetSlimeType();
            var actualSlimeHat = playerInfos.GetSlimeHat();
            var actualSlimeColor = playerInfos.GetSlimeColor();

            if (slimeTypeDropdown.options.Count == 0)
            {
                slimeTypeDropdown.AddOptions(slimeType);
                slimeTypeDropdown.value = actualSlimeType;
            }

            if (slimeHatDropdown.options.Count == 0)
            {
                slimeHatDropdown.AddOptions(slimeHat);
                slimeHatDropdown.value = actualSlimeHat;
            }

            slimeColor.SetColor(actualSlimeColor);
        }

        private void InitializeMonster()
        {
            var monsterType = new List<string> { "boy", "girl" };
            var actualMonsterType = playerInfos.GetMonsterType();
            var actualMonsterColor = playerInfos.GetMonsterColor();

            if (monsterTypeDropdown.options.Count == 0)
            {
                monsterTypeDropdown.AddOptions(monsterType);
                monsterTypeDropdown.value = actualMonsterType;
            }

            monsterColor.SetColor(actualMonsterColor);
        }
    }
}