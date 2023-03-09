using System;
using UnityEngine;
using Player.Controller.Escapist;

namespace Player.Network.Escapist
{
    public class MonsterNetwork : APlayerNetwork
    {
        void Update()
        {
            if (!isLocalPlayer) { return; }
            PlayerMovement();
        }
    }
}