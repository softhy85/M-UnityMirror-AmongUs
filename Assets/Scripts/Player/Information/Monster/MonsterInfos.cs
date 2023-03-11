using Player.Information;
using UnityEngine;

namespace Player.Information.Monster
{
    public class MonsterInfos : APlayerInfos
    {
        #region imple role

        [field: SerializeField] public PlayerRole _role = PlayerRole.Monster;
        public PlayerRole Role
        { get { return _role; } set { _role = value; } }
        #endregion
    }
}