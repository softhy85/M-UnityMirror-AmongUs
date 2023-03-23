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
        [SyncVar] private string pseudo = "";

        private GameObject playerListCanvas;
        private bool initialize = false;
        [Header("UI Canvas")]
        [SerializeField] private GameObject parentPlayer;
        [SerializeField] private GameObject localPlayer;
        [SerializeField] private GameObject otherPlayer;

        [SerializeField] private TMP_Text pseudoPlayer;

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
        public bool IsHosting()
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
            if (!isReady)
                role = newRole;
            RpcRole(role);
        }

        [Server]
        public bool IsReady()
        {
            return isReady;
        }

        [Server]
        public void SetReady(bool newReady)
        {
            isReady = newReady;
            if (lButtonReady != null)
                if (lButtonReady.TryGetComponent<Image>(out var button) && button != null)
                    button.color = newReady ? Color.green : Color.red;
            readyToBegin = isReady;
            CmdChangeReadyState(isReady);
            RpcReady(isReady);
        }

        [Server]
        private bool checkOtherClientReady()
        {
            int nbMonster = 0;
            int nbEscapist = 0;
            if (NetworkServer.connections.Count == 1)
                return false;
            foreach (var (key, conn) in NetworkServer.connections)
            {
                if (!conn.identity.isLocalPlayer && conn.identity.gameObject.TryGetComponent<RoomPlayer>(out var roomPlayer))
                {
                    if (!roomPlayer.IsReady())
                    {
                        return false;
                    }

                    var actRole = roomPlayer.GetRole();
                    if (actRole == PlayerRole.Escapist)
                        nbEscapist += 1;
                    else if (actRole == PlayerRole.Monster)
                        nbMonster += 1;
                }
            }
            if (GetRole() == PlayerRole.Escapist)
                nbEscapist += 1;
            else if (GetRole() == PlayerRole.Monster)
                nbMonster += 1;

            if (nbMonster >= 1 && nbEscapist >= 1)
                return true;
            return false;
        }

        #region Command


        [Command]
        private void CmdRole(PlayerRole newRole)
        {
            SetRole(newRole);
        }

        [Command]
        private void CmdReady(bool newReady)
        {
            if (isHost)
            {
                if (checkOtherClientReady())
                    SetReady(newReady);
            } else {
                SetReady(newReady);
            }
        }

        [Command]
        public void CmdSetPseudo(string newPseudo)
        {
            if (pseudo != "")
                RpcSetPseudo(pseudo);
            else if (newPseudo != "") {
                pseudo = newPseudo;
                RpcSetPseudo(newPseudo);
            } else
                RpcSetPseudo("Pseudo");
        }
        #endregion
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
            if (!isLocalPlayer) return;
            CmdReady(!isReady);
        }
        #endregion

        #region ClientRpc

        [ClientRpc]
        void RpcSetPseudo(string newPseudo)
        {
            pseudoPlayer.text = newPseudo;
        }
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
                var listAvailableStrings = lRoleOption.options.Select(option => option.text).ToList();
                lRoleOption.value = listAvailableStrings.IndexOf(newRole.ToString());
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
            var playersInfos = GameObject.FindGameObjectsWithTag("PlayerInfos");
            if (playersInfos.Length == 1)
            {
                if (isLocalPlayer && playersInfos[0]
                    .TryGetComponent<PlayerInfos>(out var playerInfos))
                {
                    var newPseudo = playerInfos.GetPseudo();
                    if (newPseudo != "")
                        CmdSetPseudo(newPseudo);
                    else
                        CmdSetPseudo("Pseudo");
                }
            }
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