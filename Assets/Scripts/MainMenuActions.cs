using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{

	public Button hostNavButton;

	private enum nav
	{
		HOST, JOIN, CURRENT_GAME, OPTIONS, QUIT
	};

	void Start()
	{
		Button btn = hostNavButton.GetComponent<Button>();
		btn.onClick.AddListener(onMainNavigation);
	}

	void onMainNavigation()
	{
		Debug.Log("You have clicked the button!");
	}

}
