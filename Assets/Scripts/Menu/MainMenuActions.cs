using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{
	private enum nav
	{
		HOST, JOIN, CURRENT_GAME, OPTIONS, QUIT
	};

	[Header("Navigation Button")]
	[SerializeField] private Button PlayNavButton;
	[SerializeField] private Button OptionNavButton;
	[SerializeField] private Button QuitNavButton;

	[Header("Pannels")]
	[SerializeField] private GameObject multiplayerPannel;
	[SerializeField] private GameObject settingsPannel;

	#region Trigger

	#region Add/Remove Trigger

	private void AddListener()
	{
		PlayNavButton.onClick.AddListener(OnPlayNavButton);
		OptionNavButton.onClick.AddListener(OnOptionNavButton);
		QuitNavButton.onClick.AddListener(OnQuitNavButton);
	}
	private void RemoveListener()
	{
		PlayNavButton.onClick.RemoveAllListeners();
		OptionNavButton.onClick.RemoveAllListeners();
		QuitNavButton.onClick.RemoveAllListeners();
	}

	#endregion

	private void OnPlayNavButton()
	{
		if (!multiplayerPannel.activeSelf)
			multiplayerPannel.SetActive(true);
		else
		{
			settingsPannel.SetActive(false);
			multiplayerPannel.SetActive(true);
		}
	}

	private void OnOptionNavButton()
	{
		if (!multiplayerPannel.activeSelf)
			settingsPannel.SetActive(true);
		else
		{
			multiplayerPannel.SetActive(false);
			settingsPannel.SetActive(true);
		}
	}

	private void OnQuitNavButton()
	{
		Application.Quit();
	}

	#endregion
	private void Start()
	{
		AddListener();
	}

	private void OnDestroy()
	{
		RemoveListener();
	}
}
