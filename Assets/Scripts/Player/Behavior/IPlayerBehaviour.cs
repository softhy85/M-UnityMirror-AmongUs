using Mirror;
using UnityEngine;
using Player.Controller;
using Player.Network;

namespace Player.Behaviour
{
    public interface IPlayerBehaviour
    {
        public PlayerNetwork PlayerNetwork { get; set; }
        public GameObject Body { get; set; }
        public APlayerController Controller { get; set; }
        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }
        public Vector3 CameraRelative { get; set; }
        public Camera Camera { get; set; }
        public Camera MainCamera { get; set; }

        public virtual void SetBehavior(APlayerBehaviour lastBehavior){}

    }
}