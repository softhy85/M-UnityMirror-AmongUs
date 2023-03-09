using System;
using UnityEngine;
using Player.Controller.Escapist;

namespace Player.Network.Escapist
{
    public class PhantomNetwork : APlayerNetwork
    {
        void Update()
        {
            if (!isLocalPlayer) { return; }
            PlayerMovement();
        }
    }
}