using System;
using System.Collections.Generic;
using Mirror;
using Player.Network.Escapist;
using Player.Information;
using Player.Network;
using UnityEngine;

namespace Network {
    [Serializable]
    public struct PlayerPrefab {
        public APlayerNetwork prefab;
        public PlayerRole role;
    }

    public interface INetworkManager
    {
        [field: Header("Game Settings")]
        public int MaxPlayer { get; set; }

        [field: Header("Lobby")]
        public string LobbyScene { get; set; }
        public APlayerInfos PlayerInfos { get; set; }

        [field: Header("Game")]
        public string GameScene { get; set; }
        public PlayerPrefab[] PlayerPrefabs { get; set; }

        public void OnServerAddPlayer(NetworkConnectionToClient conn);
    }
}