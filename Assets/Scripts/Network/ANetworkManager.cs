using System.Collections.Generic;
using Mirror;
using Player.Information;
using Player.Network.Escapist;
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
        #region imple player prefabs

        [field: SerializeField] protected PlayerPrefab[] _playerPrefabs;
        public PlayerPrefab[] PlayerPrefabs
        { get => _playerPrefabs; set => _playerPrefabs = value; }
        #endregion
        #endregion

        protected virtual void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
        }
    }
}