using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Player.Information;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Player.Room
{
    public class RoomPlayer : NetworkRoomPlayer
    {
        [SyncVar] private PlayerRole role = PlayerRole.Escapist;
        [SyncVar] public bool isReady = false;
        [SyncVar] private bool isHost = false;

        private GameObject playerListCanvas;
        private bool initialize = false;
        [Header("UI Canvas")]
        [SerializeField] private GameObject parentPlayer;
        [SerializeField] private GameObject localPlayer;
        [SerializeField] private GameObject otherPlayer;

        [Header("UI Host")]
        [SerializeField] private TMP_Text HostPlayer;
        
        [Header("UI Local Player")]
        [SerializeField] private Button lButtonReady;
        [SerializeField] private TMP_Dropdown lRoleOption;
        [SerializeField] private TMP_Text lRoleSelected;
        
        [Header("UI Other Player")]
        [SerializeField] private Image oReady;
        [SerializeField] private TMP_Text oRoleSelected;

        #region Server

        [Server]
        public bool isHosting()
        {
            return isHost;
        }

        [Server]
        public void SetHosting(bool host)
        {
            isHost = host;
        }

        [Server]
        public PlayerRole GetRole()
        {
            return role;
        }

        [Server]
        public void SetRole(PlayerRole newRole)
        {
            role = newRole;
            RpcRole(role);
        }

        [Server]
        public void SetReady(bool newReady)
        {
            isReady = newReady;
            if (lButtonReady != null)
                if (lButtonReady.TryGetComponent<Image>(out var button) && button != null)
                    button.color = newReady ? Color.green : Color.red;
            RpcReady(isReady);
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
            CmdRole(Enum.GetValues(typeof(PlayerRole)).Cast<PlayerRole>().ToArray()[lRoleOption.value]);
        }

        [Client]
        public void ClientReady()
        {
            if (!isOwned) return;
            var newReady = !isReady;
            CmdReady(newReady);
            CmdChangeReadyState(newReady);
        }
        #endregion

        #region ClientRpc

        [ClientRpc]
        void RpcRole(PlayerRole newRole)
        {
            if (isLocalPlayer)
            {
                int i = 0;

                while (i < lRoleOption.options.Count &&
                       newRole.ToString() != lRoleOption.options[i].text)
                    i++;
                if (i > lRoleOption.options.Count) return;
                lRoleSelected.text = newRole.ToString();
            }
            else
            {
                oRoleSelected.text = newRole.ToString();
            }
        }

        [ClientRpc]
        void RpcReady(bool ready)
        {
            if (isLocalPlayer)
            {
                if (lButtonReady != null)
                    if (lButtonReady.TryGetComponent<Image>(out var button) && button != null)
                        button.color = ready ? Color.green : Color.red;
            }
            else
            {
                if (oReady != null)
                    oReady.color = ready ? Color.green : Color.red;
            }
        }
        #endregion

        #region Other

        public void Initiate()
        {
            parentPlayer.transform.SetParent(playerListCanvas.transform);
            if (isLocalPlayer) {
                localPlayer.SetActive(true);
                List<string> playerRoles = Enum.GetNames(typeof(PlayerRole)).ToList();
                lRoleOption.ClearOptions();
                lRoleOption.AddOptions(playerRoles);
                lRoleOption.SetValueWithoutNotify(0);
                if (lButtonReady != null)
                    if (lButtonReady.TryGetComponent<Image>(out var button))
                        button.color = isReady ? Color.green : Color.red;
            }
            else
            {
                otherPlayer.SetActive(true);
                oRoleSelected.text = role.ToString();
                if (oReady != null)
                    oReady.color = isReady ? Color.green : Color.red;

            }
            if (isHost)
            {
                HostPlayer.gameObject.SetActive(true);
            }
        }

        public override void OnClientEnterRoom()
        {
            if (!initialize)
            {
                var temp = GameObject.FindGameObjectsWithTag("RoomListCanvas");
                if (temp.Length == 1)
                {
                    playerListCanvas = temp[0];
                    Initiate();
                    initialize = true;
                }
            }
        }

        private void OnDestroy()
        {
            Destroy(parentPlayer);
        }

        #endregion
    }
}