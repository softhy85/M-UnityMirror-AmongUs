using Mirror;
using Player.Information;
using UnityEngine;

namespace UI
{
    public class WinWindow : NetworkBehaviour
    {
        [SerializeField] private GameObject monsterPanel;
        [SerializeField] private GameObject escapistPanel;

        [Command(requiresAuthority = false)]
        public void CmdActivateWinScreen(PlayerRole role)
        {
            RpcActivateWinScreen(role);
        }

        [ClientRpc]
        private void RpcActivateWinScreen(PlayerRole role)
        {
            if (role == PlayerRole.Escapist)
            {
                escapistPanel.SetActive(true);
            }
            else if (role == PlayerRole.Monster)
            {
                monsterPanel.SetActive(true);
            }
        }

        private void Awake()
        {
            escapistPanel.SetActive(false);
            monsterPanel.SetActive(false);
        }
    }
}