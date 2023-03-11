using System;
using Mirror;
using UnityEngine;

namespace Player.Information
{

    public class APlayerInfos : NetworkBehaviour, IPlayerInfos
    {
        #region imple uuid

        protected string _uuid;
        public string Uuid
        { get => _uuid; set => _uuid = value; }
        #endregion
        #region imple pseudo

        [field: SerializeField] protected string _pseudo = "Player Name...";
        public string Pseudo 
        { get => _pseudo; set => _pseudo = value; }
        #endregion
        #region imple role

        [field: SerializeField] protected PlayerRole _role = PlayerRole.Escapist;
        public PlayerRole Role
        { get => _role; set => _role = value; }
        #endregion

        protected virtual void Awake()
        {
            _uuid = Guid.NewGuid().ToString();
        }
    }
}