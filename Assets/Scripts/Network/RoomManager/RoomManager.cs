using System;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Menu;
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
        [SerializeField] private PlayerPrefab[] playerPrefabs;

        private string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }

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
            PlayerRole role = roomPlayer.GetComponent<RoomPlayer>().GetRole();
            GameObject player = null;
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == role)
                {
                    Transform startPos = GetStartPosition();
                    player = startPos != null
                        ? Instantiate(playerPrefabs[i].prefab, startPos.position, startPos.rotation)
                        : Instantiate(playerPrefabs[i].prefab, Vector3.zero, Quaternion.identity);
                    player.name = $"{playerPrefabs[i].role.ToString()} [connId={conn.connectionId}]";
                }
            }
            return (player);
        }

        private void PlayerKilled(NetworkConnectionToClient conn, EscapistBehaviour escapistBehaviour)
        {
            GameObject phantomObj = null;
            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == PlayerRole.Phantom)
                {
                    phantomObj = Instantiate(playerPrefabs[i].prefab);
                    phantomObj.name = $"{PlayerRole.Phantom.ToString()} [connId={conn.connectionId}]";
                    NetworkServer.Spawn(phantomObj);
                }
            }

            if (phantomObj)
            {
                NetworkServer.RemovePlayerForConnection(conn, conn.identity.gameObject);
                NetworkServer.AddPlayerForConnection(conn, phantomObj);
                if (conn.identity.TryGetComponent<PhantomBehaviour>(out var phantomBehaviour))
                {
                    phantomBehaviour.CmdActivateCamera();
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
                                var escapistBehaviour =
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