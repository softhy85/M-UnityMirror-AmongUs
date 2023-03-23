using Mirror;
using Player.Behaviour;
using Player.Behaviour.Escapist;
using Player.Information;
using Player.Information.Structure;
using UnityEngine;

namespace Network {
    public abstract class ANetworkManager : NetworkManager
    {
        protected int connections = 0;

        [field: Header("Game")]
        [field: SerializeField] protected PlayerPrefab[] playerPrefabs;
        [field: Scene] [field: SerializeField] protected string _gameScene;


        public override void Awake()
        {
            base.Awake();
            // _uuidGame = Guid.NewGuid().ToString();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = null;
            PlayerRole role = PlayerRole.Escapist;

            if (connections == 0)
                role = PlayerRole.Monster;

            for (int i = 0; i < playerPrefabs.Length; i++)
            {
                if (playerPrefabs[i].role == role) {
                    player = Instantiate(playerPrefabs[i].prefab);
                }
            }

            player.name = $"{player.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);

            foreach(var (key, cliConn) in NetworkServer.connections) {
                if (cliConn.identity.TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                {
                    playerBehaviour.CmdActivateCamera();
                }
            }
            connections += 1;
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            Debug.Log("OnServerConnect");
        }

        public override void OnStartServer()
        {
            Debug.Log("OnStartServer");
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            Debug.Log("OnServerReady");
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
            foreach(var (key, conn) in NetworkServer.connections)
            {
                if (conn.identity.gameObject)
                {
                    if (conn.identity.gameObject
                        .TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                    {
                        if (playerBehaviour.GetRole() == PlayerRole.Escapist)
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