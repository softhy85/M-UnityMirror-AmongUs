using System;
using Mirror;
using UnityEngine;

namespace Player.Information
{

    public class APlayerInfos : NetworkBehaviour
    {
        protected string _uuid;

        [field: SerializeField] protected string _pseudo = "Player Name...";

        [field: SerializeField] protected PlayerRole _role = PlayerRole.Escapist;

        protected virtual void Awake()
        {
            _uuid = Guid.NewGuid().ToString();
        }
    }
}