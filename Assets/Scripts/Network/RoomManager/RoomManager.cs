using JetBrains.Annotations;
using Mirror;
using Player.Behaviour;
using Player.Behaviour.Escapist;
using Player.Behaviour.Monster;
using Player.Information;
using Player.Information.Structure;
using Player.Room;
using UnityEngine;

namespace Network {
    public class RoomManager : NetworkRoomManager
    {
        private bool host = false;
        [Header("Game")]
        public PlayerPrefab[] playerPrefabs;


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
            foreach(var (key, cliConn) in NetworkServer.connections) {
                if (cliConn.identity.TryGetComponent<RoomPlayer>(out var roomPlayer))
                {
                    roomPlayer.CmdSetPseudo("");
                }
            }
        }

        public override GameObject OnRoomServerCreateGamePlayer(
            NetworkConnectionToClient conn, GameObject roomPlayer)
        {
            Debug.Log("OnRoomServerCreateGamePlayer");
            PlayerRole role = roomPlayer.GetComponent<RoomPlayer>().GetRole();
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == role)
                {
                    GameObject player = Instantiate(playerPrefabs[i].prefab);
                    return (player);
                }
            }
            foreach(var (key, cliConn) in NetworkServer.connections) {
                if (cliConn.identity.TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                {
                    playerBehaviour.CmdActivateCamera();
                }
            }

            return (null);
        }
        private void PlayerKilled(NetworkConnectionToClient conn, EscapistBehaviour escapistBehaviour)
        {
            GameObject phantomObj = null;
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == PlayerRole.Phantom)
                {
                    phantomObj = Instantiate(playerPrefabs[i].prefab);
                    phantomObj.name = $"{phantomObj.name} [connId={conn.connectionId}]";
                    NetworkServer.Spawn(phantomObj);
                }
            }

            if (phantomObj)
            {
                NetworkServer.RemovePlayerForConnection(conn, conn.identity.gameObject);
                NetworkServer.AddPlayerForConnection(conn, phantomObj);
                if (conn.identity.TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                {
                    playerBehaviour.CmdActivateCamera();
                }
                escapistBehaviour.CmdDestroy();
            }
        }

        public override void Update()
        {
            base.Update();
            if (!Utils.IsSceneActive(RoomScene))
            {
                foreach (var (key, conn) in NetworkServer.connections)
                {
                    if (conn.identity.gameObject)
                    {
                        if (conn.identity.gameObject
                            .TryGetComponent<APlayerBehaviour>(
                                out var playerBehaviour))
                        {
                            if (playerBehaviour.GetRole() ==
                                PlayerRole.Escapist)
                            {
                                EscapistBehaviour escapistBehaviour =
                                    (EscapistBehaviour)playerBehaviour;
                                if (escapistBehaviour.IsKilled())
                                {
                                    PlayerKilled(conn, escapistBehaviour);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}