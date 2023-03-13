using Player.Information;
using UnityEngine;

namespace Player.Information.Escapist
{
    public class EscapistInfos : APlayerInfos
    {
        protected override void Awake()
        {
            base.Awake();
            _role = PlayerRole.Escapist;
        }
    }
}