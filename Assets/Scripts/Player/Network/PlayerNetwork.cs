using Mirror;
using UnityEngine;
using Network;
using Player.Behaviour;

namespace Player.Network
{
    public class PlayerNetwork : NetworkBehaviour, IPlayerNetwork
    {
        #region imple player behaviour
        protected APlayerBehaviour _playerBehaviour;
        public APlayerBehaviour PlayerBehaviour
        { get => _playerBehaviour; set => _playerBehaviour = value; }
        #endregion
        [field: SerializeField] protected PlayerPrefab[] _playerPrefabs;
        protected GameObject _actualPlayer = null;

        #region Server

        [Server]
        [ContextMenu("Add Escapist")]
        void AddPrefabEscapist()
        {
            if (_actualPlayer == null) {
                _actualPlayer = Instantiate(_playerPrefabs[0].prefab, this.gameObject.transform);
                _playerBehaviour = _actualPlayer.GetComponent<APlayerBehaviour>();
                _playerBehaviour.ActivateCamera();
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