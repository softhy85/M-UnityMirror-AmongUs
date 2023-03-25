using System.Linq;
using System.Net;
using Mirror;
using Player.Behaviour;
using Player.Behaviour.Escapist;
using Player.Information;
using Player.Information.Structure;
using Player.Room;
using UI;
using UnityEngine;

namespace Network {
    public class RoomManager : NetworkRoomManager
    {
        private bool host = false;

        [Header("Game")]
        [SerializeField] private PlayerPrefab[] playerPrefabs;

        private Timer gameTimer;
        private WinWindow winWindow;
        private PlayerRole roleWin = PlayerRole.NoRole;
        private float renderWinTime = 0;

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
                escapistBehaviour.CmdDestroy();
            }
        }

        private void CheckPlayerKilled()
        {
            int nbEscapist = 0;
            int nbNoRole = 0;
            foreach (var (key, conn) in NetworkServer.connections)
            {
                if (conn.identity.gameObject)
                {
                    if (conn.identity.gameObject
                        .TryGetComponent<APlayerBehaviour>(
                            out var playerBehaviour))
                    {
                        var playerRole = playerBehaviour.GetRole();
                        if (playerRole ==
                            PlayerRole.Escapist)
                        {
                            var escapistBehaviour =
                                (EscapistBehaviour)playerBehaviour;
                            if (escapistBehaviour.IsKilled())
                                PlayerKilled(conn, escapistBehaviour);
                            else
                                nbEscapist += 1;
                        } else if (playerRole == PlayerRole.NoRole)
                            nbNoRole += 1;
                    } else
                        nbNoRole += 1;
                }
            }

            if (nbEscapist == 0 && nbNoRole == 0 && NetworkServer.connections.Count > 1 && winWindow)
            {
                roleWin = PlayerRole.Monster;
                winWindow.CmdActivateWinScreen(roleWin);
                renderWinTime = 10;
                gameTimer.gameObject.SetActive(false);
            }
        }

        private void CheckIfEscapistWin()
        {
            if (gameTimer && winWindow) {
                if (gameTimer.GetTimer() <= 0)
                {
                    roleWin = PlayerRole.Escapist;
                    winWindow.CmdActivateWinScreen(roleWin);
                    renderWinTime = 10;
                    gameTimer.gameObject.SetActive(false);
                }
            }

        }

        public override void Update()
        {
            base.Update();
            if (Utils.IsSceneActive(GameplayScene))
            {
                if (!gameTimer)
                {
                    var gameTimersObj =
                        GameObject.FindGameObjectsWithTag("Timer");
                    if (gameTimersObj.Length == 1)
                        if (gameTimersObj[0].TryGetComponent<Timer>(out var actGameTime))
                            gameTimer = actGameTime;
                }
                if (!winWindow)
                {
                    var winWindowsObj =
                        GameObject.FindGameObjectsWithTag("WinWindow");
                    if (winWindowsObj.Length == 1)
                        if (winWindowsObj[0].TryGetComponent<WinWindow>(out var actWinWindow))
                            winWindow = actWinWindow;
                }
                if (roleWin == PlayerRole.NoRole) {

                    CheckPlayerKilled();
                    CheckIfEscapistWin();
                }
                else
                {
                    if (renderWinTime >= 0)
                    {
                        renderWinTime -= Time.deltaTime;
                    }
                    else
                    {
                        foreach (var (key, conn) in NetworkServer.connections)
                        {
                            if (conn.identity.gameObject)
                            {
                                if (conn.identity.gameObject
                                    .TryGetComponent<APlayerBehaviour>(
                                        out var playerBehaviour))
                                {
                                    if (playerBehaviour.IsHost()) {
                                        playerBehaviour.CmdDisconnect();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}