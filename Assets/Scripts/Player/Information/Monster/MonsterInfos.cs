using Player.Information;
using UnityEngine;

namespace Player.Information.Monster
{
    public class MonsterInfos : APlayerInfos
    {
        protected override void Awake()
        {
            base.Awake();
            _role = PlayerRole.Monster;
        }
    }
}