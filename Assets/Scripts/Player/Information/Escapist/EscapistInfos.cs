using Player.Information;
using UnityEngine;

namespace Player.Information.Escapist
{
    public class EscapistInfos : APlayerInfos
    {
        #region imple role

        [field: SerializeField] public PlayerRole _role = PlayerRole.Escapist;

        public PlayerRole Role
        { get { return _role; } set { _role = value; } }
        #endregion
        
    }
}