using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player.Information;
using UnityEngine;

public class WinWindow : NetworkBehaviour
{
    [SerializeField] private GameObject monsterPanel;
    [SerializeField] private GameObject escapistPanel;

    public void activateWinScreen(PlayerRole role)
    {
        if (role == PlayerRole.Escapist)
        {
            escapistPanel.SetActive(true);
        }
        else if (role == PlayerRole.Monster)
        {
            monsterPanel.SetActive(true);
        }
    }

    private void Awake()
    {
        escapistPanel.SetActive(false);
        monsterPanel.SetActive(false);
    }
}
