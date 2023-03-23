using System;
using UnityEngine;

namespace Player
{
    public class LobbyBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject _playerListCanva;

        private void AddPlayer(GameObject playerRoom)
        {
            playerRoom.transform.SetParent(_playerListCanva.transform);
        }
    }
}