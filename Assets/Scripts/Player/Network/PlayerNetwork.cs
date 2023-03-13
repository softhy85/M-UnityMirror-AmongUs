using Mirror;
using UnityEngine;
using Network;
using Player.Behaviour;
using Player.Information;
using Player.Information.Escapist;
using Player.Information.Monster;

namespace Player.Network
{
    public class PlayerNetwork : NetworkBehaviour, IPlayerNetwork
    {
        #region var sync
        #region imple player infos

        [SyncVar]
        protected APlayerInfos _playerInfos = null;
        public APlayerInfos PlayerInfos
        { get => _playerInfos; set => _playerInfos = value; }
        #endregion
        #endregion

        #region non var sync
        #region imple player behaviour
        protected APlayerBehaviour _playerBehaviour;
        public APlayerBehaviour PlayerBehaviour
        { get => _playerBehaviour; set => _playerBehaviour = value; }
        #endregion
        #region imple player prefabs
        [field: SerializeField] protected PlayerPrefab[] _playerPrefabs;
        public PlayerPrefab[] PlayerPrefabs
        { get => _playerPrefabs; set => _playerPrefabs = value; }

        #endregion
        #region imple actual player
        protected GameObject _actualPlayer = null;
        public GameObject ActualPlayer
        { get => _actualPlayer; set => _actualPlayer = value; }
        #endregion
        #endregion

        #region Server

        [Server]
        protected void AddPrefab(PlayerRole role)
        {
            if (_actualPlayer == null) {
                for (int i = 0; i < _playerPrefabs.Length; i++)
                {
                    if (_playerPrefabs[i].role == role) {
                        _actualPlayer = Instantiate(_playerPrefabs[i].prefab, this.gameObject.transform);
                        _playerBehaviour = _actualPlayer.GetComponent<APlayerBehaviour>();
                        _playerBehaviour.ActivateCamera();
                    }
                }
            }

            if (_playerInfos == null)
            {
                switch (role)
                {
                    case PlayerRole.Escapist:
                        _playerInfos = gameObject.AddComponent<EscapistInfos>();
                        break;
                    case PlayerRole.Phantom:
                        _playerInfos = gameObject.AddComponent<EscapistInfos>();
                        break;
                    case PlayerRole.Monster:
                        _playerInfos = gameObject.AddComponent<MonsterInfos>();
                        break;
                }
            }
        }

        [Server]
        void OnDestroy()
        {
            if (!isLocalPlayer) return;
            _playerBehaviour.DesactivateCamera();
        }

        [Command]
        public void CmdMove(Vector3 movementVector)
        {
            RpcMove(movementVector);
        }
        #endregion

        #region Client Rpc
        [ClientRpc]
        protected void RpcMove(Vector3 movementVector)
        {
            _playerBehaviour.MoveTowardTarget(movementVector);
            _playerBehaviour.RotateTowardMovementVector(movementVector);
        }
        #endregion

    }
}