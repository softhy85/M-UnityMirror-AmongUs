using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class MyPlayerNetwork : NetworkBehaviour
{
    // Server variables

    // Only client variables

    public override void OnStartLocalPlayer()
    {
    }

    #region Server
    #endregion

    #region Client
    #endregion
}