using Mirror;
using Network;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class LobbyMenu : NetworkBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField] private Button disconnectButton;

        // [Header("Player")]
        // [SerializeField] private GameObject playerListCanva;

        private RoomManager roomManager;

        #region Trigger

        #region Add/Remove Trigger

        [Client]
        private void AddListener()
        {
            if (!isLocalPlayer) return;
            disconnectButton.onClick.AddListener(OnDisconnectButton);
        }

        [Client]
        private void RemoveListener()
        {
            if (!isLocalPlayer) return;
            disconnectButton.onClick.RemoveAllListeners();
        }

        #endregion

        [Client]
        private void OnDisconnectButton()
        {
            if (isClientOnly)
            {
                roomManager.StopClient();
            } else {
                roomManager.StopHost();
            }
        }

        #endregion

        private void Start()
        {
            roomManager = (RoomManager)NetworkManager.singleton;
        }

        [Client]
        private void OnDestroy()
        {
            if (!isLocalPlayer) return;
            RemoveListener();
        }

        // private void AddPlayer(GameObject playerRoom)
        // {
        //     playerRoom.transform.SetParent(playerListCanva.transform);
        // }
    }
}