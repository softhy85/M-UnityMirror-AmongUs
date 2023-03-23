using System;
using Mirror;
using UnityEngine;
using Network;
using Player.Behaviour;
using Player.Information;

namespace Player.Network
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SyncVar]
        protected PlayerRole role = PlayerRole.Escapist;

        protected APlayerBehaviour playerBehaviour;

        [field: SerializeField] protected PlayerPrefab[] playerPrefabs;

        protected GameObject actualPlayer = null;

        #region Server

        [Command]
        public void CmdAddPrefab(PlayerRole newRole)
        {
            if (!isOwned) return;
            if (actualPlayer == null) {
                for (int i = 0; i < playerPrefabs.Length; i++)
                {
                    if (playerPrefabs[i].role == role) {
                        actualPlayer = Instantiate(playerPrefabs[i].prefab, gameObject.transform);
                        NetworkServer.Spawn(actualPlayer, NetworkClient.connection);
                        playerBehaviour = actualPlayer.GetComponent<APlayerBehaviour>();
                    }
                }
            }
        }

        [Server]

            role = newRole;
        }

        #endregion
    }
}