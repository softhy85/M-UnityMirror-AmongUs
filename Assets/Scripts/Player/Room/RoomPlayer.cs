using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Player.Information;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Player.Room
{
    public class RoomPlayer : NetworkRoomPlayer, IRoomPlayer
    {
        [SyncVar] private PlayerRole _role = PlayerRole.Escapist;
        [SyncVar] private bool _ready = false;
        [SyncVar] private bool _isHost = false;

        private GameObject _playerListCanvas;
        private bool _initialize = false;
        [Header("UI Canvas")]
        [SerializeField] private GameObject _parentPlayer;
        [SerializeField] private GameObject _localPlayer;
        [SerializeField] private GameObject _otherPlayer;

        [Header("UI Host")]
        [SerializeField] private TMP_Text _HostPlayer;
        
        [Header("UI Local Player")]
        [SerializeField] private Button _lButtonReady;
        [SerializeField] private TMP_Dropdown _lRoleOption;
        [SerializeField] private TMP_Text _lRoleSelected;
        
        [Header("UI Other Player")]
        [SerializeField] private Image _oReady;
        [SerializeField] private TMP_Text _oRoleSelected;

        public void Initiate()
        {
            _parentPlayer.transform.SetParent(_playerListCanvas.transform);
            if (isLocalPlayer) {
                _localPlayer.SetActive(true);
                List<string> playerRoles = Enum.GetNames(typeof(PlayerRole)).ToList();
                _lRoleOption.ClearOptions();
                _lRoleOption.AddOptions(playerRoles);
                _lRoleOption.SetValueWithoutNotify(0);
                if (_lButtonReady != null)
                    if (_lButtonReady.TryGetComponent<Image>(out var button))
                        button.color = _ready ? Color.green : Color.red;
            }
            else
            {
                _otherPlayer.SetActive(true);
                _oRoleSelected.text = _role.ToString();
                if (_oReady != null)
                    _oReady.color = _ready ? Color.green : Color.red;

            }
            if (_isHost)
            {
                _HostPlayer.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (!_initialize)
            {
                var temp = GameObject.FindGameObjectsWithTag("RoomListCanvas");
                if (temp.Length == 1)
                {
                    _playerListCanvas = temp[0];
                    Initiate();
                    _initialize = true;
                }
            }
        }

        private void OnDestroy()
        {
            Destroy(_parentPlayer);
        }

        #region Server

        [Server]
        public bool isHosting()
        {
            return _isHost;
        }

        [Server]
        public void SetHosting(bool host)
        {
            _isHost = host;
        }

        [Server]
        public PlayerRole GetRole()
        {
            return _role;
        }

        [Server]
        public void SetRole(PlayerRole role)
        {
            _role = role;
            RpcRole(_role);
        }

        [Server]
        public void SetReady(bool newReady)
        {
            _ready = newReady;
            if (_lButtonReady != null)
                if (_lButtonReady.TryGetComponent<Image>(out var button) && button != null)
                    button.color = newReady ? Color.green : Color.red;
            RpcReady(_ready);
        }
        #endregion

        #region Command

        
        [Command(requiresAuthority = false)]
        private void CmdRole(PlayerRole newRole)
        {
            SetRole(newRole);
        }

        [Command(requiresAuthority = false)]
        private void CmdReady(bool newReady)
        {
            SetReady(newReady);
        }

        #endregion

        #region Client

        [Client]
        public void ClientRoleSelected()
        {
            if (!isOwned) return;
            CmdRole(Enum.GetValues(typeof(PlayerRole)).Cast<PlayerRole>().ToArray()[_lRoleOption.value]);
        }

        [Client]
        public void ClientReady()
        {
            if (!isOwned) return;
            var newReady = !_ready;
            CmdReady(newReady);
            CmdChangeReadyState(newReady);
        }
        #endregion

        #region ClientRpc

        [ClientRpc]
        void RpcRole(PlayerRole role)
        {
            if (isLocalPlayer)
            {
                int i = 0;

                while (i < _lRoleOption.options.Count &&
                       role.ToString() != _lRoleOption.options[i].text)
                    i++;
                if (i > _lRoleOption.options.Count) return;
                _lRoleSelected.text = role.ToString();
            }
            else
            {
                _oRoleSelected.text = role.ToString();
            }
        }

        [ClientRpc]
        void RpcReady(bool ready)
        {
            if (isLocalPlayer)
            {
                if (_lButtonReady != null)
                    if (_lButtonReady.TryGetComponent<Image>(out var button) && button != null)
                        button.color = ready ? Color.green : Color.red;
            }
            else
            {
                if (_oReady != null)
                    _oReady.color = ready ? Color.green : Color.red;
            }
        }
        #endregion
    }
}