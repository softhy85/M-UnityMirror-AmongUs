using UnityEngine;
using Network;
using Player.Behaviour;
using Player.Information;

namespace Player.Network
{
    public interface IPlayerNetwork
    {

        #region var sync

        public APlayerInfos PlayerInfos { get; set; }

        #endregion

        #region non sync var
        public APlayerBehaviour PlayerBehaviour { get; set; }
        public PlayerPrefab[] PlayerPrefabs { get; set; }
        public GameObject ActualPlayer { get; set; }

        #endregion

    }
}