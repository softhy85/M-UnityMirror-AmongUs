using System;
using System.Collections.Generic;
using Mirror;
using Player.Information;
using Player.Network;
using Player.Room;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.PlayerLoop;

namespace Network {
    public class RoomManager : NetworkRoomManager, IRoomManager
    {
        private bool host = false;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            if (conn.identity.TryGetComponent<RoomPlayer>(out var player))
            {
                if (!host)
                {
                    host = true;
                    player.SetHosting(true);
                }
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            // GameObject[] roomPlayers = GameObject.FindGameObjectsWithTag("RoomPlayer");
            //
            // for (int i = 0; i < roomPlayers.Length; i++)
            // {
            //     RoomPlayer roomPlayer = roomPlayers[i].GetComponent<RoomPlayer>();
            // }
        }

        // public override void OnServerSceneChanged(string sceneName)
        // {
        //     if (sceneName == GameplayScene)
        //     {
        //         // call SceneLoadedForPlayer on any players that become ready while we were loading the scene.
        //         int it = 0;
        //         foreach (PendingPlayer pending in pendingPlayers) {
        //             SceneLoadedForPlayer(pending.conn, pending.roomPlayer);
        //             it++;
        //         }
        //
        //         pendingPlayers.Clear();
        //     }
        //
        //     OnRoomServerSceneChanged(sceneName);
        // }

        // public override void OnRoomServerSceneChanged(string sceneName)
        // {
        // }

        // public virtual void OnRoomServerAddPlayer(
        //     NetworkConnectionToClient conn)
        // {
        //     if (conn.identity.TryGetComponent<PlayerNetwork>(out var player) && player != null)
        //     {
        //         player.SetRole(role);
        //         // TODO Get pseudo
        //     }
        // }

        public virtual GameObject OnRoomServerCreateGamePlayer(
            NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            PlayerRole role = roomPlayer.GetComponent<RoomPlayer>().GetRole();
            GameObject player = null;
            Transform startPos = GetStartPosition();
            for (int it = 0; it < listPrefab.Lenght; it++)
            {
                if (listPrefab.role == role)
                {
                    player = Instantiate(listPrefab.prefab, startPos.position,
                        startPos.rotation);
                }
            }

            if (conn.identity.TryGetComponent<PlayerNetwork>(out var playerNet))
            {
                playerNet.SetRole(role);
                // TODO Get pseudo
            }
        }

        // protected override void SceneLoadedForPlayer(NetworkConnectionToClient conn, GameObject roomPlayer)
        // {
        //     Debug.Log($"NetworkRoom SceneLoadedForPlayer scene: {SceneManager.GetActiveScene().path} {conn}");
        //
        //     if (Utils.IsSceneActive(RoomScene))
        //     {
        //         // cant be ready in room, add to ready list
        //         PendingPlayer pending;
        //         pending.conn = conn;
        //         pending.roomPlayer = roomPlayer;
        //         pendingPlayers.Add(pending);
        //         return;
        //     }
        //
        //     GameObject gamePlayer = OnRoomServerCreateGamePlayer(conn, roomPlayer);
        //     if (gamePlayer == null)
        //     {
        //         // get start position from base class
        //         Transform startPos = GetStartPosition();
        //         gamePlayer = startPos != null
        //             ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
        //             : Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        //     }
        //
        //     PlayerRole role = roomPlayer.GetComponent<RoomPlayer>().GetRole();
        //     if (!OnRoomServerSceneLoadedForPlayer(conn, roomPlayer, gamePlayer))
        //         return;
        //     // replace room player with game player
        //     NetworkServer.ReplacePlayerForConnection(conn, gamePlayer, true);
        //     if (conn.identity.TryGetComponent<PlayerNetwork>(out var player))
        //     {
        //         player.SetRole(role);
        //         // TODO Get pseudo
        //     }
        // }
    }
}