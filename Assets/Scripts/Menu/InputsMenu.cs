using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputsMenu : MonoBehaviour
{
    [Header("UI Inputs")]
    [SerializeField] private Button closeButton;

    [Header("Panel")] [SerializeField] private GameObject settingsPanel;

    #region Trigger

    #region Add/Remove Trigger

    private void AddListener()
    {
        closeButton.onClick.AddListener(OncloseButton);
    }
    private void RemoveListener()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    #endregion

    private void OncloseButton()
    {
        settingsPanel.SetActive(false);
    }

    #endregion

    private void OnEnable()
    {
        AddListener();
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
