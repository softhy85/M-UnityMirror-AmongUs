using UnityEngine;
using Player.Controller;

namespace Player.Network
{
    public interface IPlayerNetwork
    {
        public APlayerController Controller { get; set; }
        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }
        public Camera Camera { get; set; }
    }
    
}