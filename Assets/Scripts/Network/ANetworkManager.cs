using System;
using System.Collections.Generic;
using Mirror;
using Player.Information;
using Player.Network;
using UnityEngine;

namespace Network {
    public abstract class ANetworkManager : NetworkManager, INetworkManager
    {
        #region Var Game Settings
        [field: Header("Game Settings")]
        #region imple max player
        [field: SerializeField] protected int _maxPlayer = 12;
        public int MaxPlayer
        { get => _maxPlayer; set => _maxPlayer = value; }
        #endregion
        #region imple private game
        [field: SerializeField] protected bool _privateGame = false;
        public bool PrivateGame
        { get => _privateGame; set => _privateGame = value; }
        #endregion
        #region imple uuid game
        protected string _uuidGame;
        public string UuidGame
        { get => _uuidGame; set => _uuidGame = value; }
        #endregion
        #endregion

        #region Var Lobby
        [field: Header("Lobby")]
        #region imple lobby scene
        [field: Scene] [field: SerializeField] protected string _lobbyScene;
        public string LobbyScene
        { get => _lobbyScene; set => _lobbyScene = value; }
        #endregion
        #region imple lobby player infos
        [field: SerializeField] protected APlayerInfos _playerInfos;
        public APlayerInfos PlayerInfos
        { get { return _playerInfos; } set { _playerInfos = value; } }
        #endregion
        protected List<NetworkConnectionToClient> _connections;
        #endregion

        #region Var Game
        [field: Header("Game")]
        #region imple game scene
        [field: Scene] [field: SerializeField] protected string _gameScene;
        public string GameScene
        { get => _gameScene;
            set => _gameScene = value;
        }
        #endregion
        #endregion

        public override void Awake()
        {
            base.Awake();
            _uuidGame = Guid.NewGuid().ToString();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            // _connections.Add(conn);
            var playerNetwork = conn.identity.GetComponent<PlayerNetwork>();
            Debug.Log("Test");
            if (playerNetwork) {
                Debug.Log("Test 2 ");
                // if (_connections.Count >= 1)
                    playerNetwork.AddPrefab(PlayerRole.Escapist);
                // else
                    // playerNetwork.AddPrefab(PlayerRole.Monster);
            }
        }
    }
}