using System;
using UnityEngine;
using Player.Controller.Escapist;

namespace Player.Network.Escapist
{
    public class EscapistNetwork : APlayerNetwork
    {
        void Update()
        {
            PlayerMovement();
        }
    }
}