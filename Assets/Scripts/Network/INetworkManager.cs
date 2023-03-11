using System;
using System.Collections.Generic;
using Mirror;
using Player.Information;
using Player.Network;
using UnityEngine;

namespace Network {
    [Serializable]
    public struct PlayerPrefab {
        public GameObject prefab;
        public PlayerRole role;
    }

    public interface INetworkManager
    {
        public int MaxPlayer { get; set; }
        public bool PrivateGame { get; set; }
        public string UuidGame { get; set; }

        public string LobbyScene { get; set; }
        public APlayerInfos PlayerInfos { get; set; }

        public string GameScene { get; set; }
        public PlayerPrefab[] PlayerPrefabs { get; set; }

        public void OnServerAddPlayer(NetworkConnectionToClient conn);
    }
}