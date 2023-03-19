using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using Player.Controller;
using Player.Network;

namespace Player.Behaviour
{
    [Serializable]
    public struct body
    {
        public GameObject gameObject;
        public Material material;
    }
    public interface IPlayerBehaviour
    {
        public PlayerNetwork PlayerNetwork { get; set; }
        public List<body> Bodies { get; set; }
        public int ActualBody { get; set; }
        public APlayerController Controller { get; set; }
        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }
        public Vector3 CameraRelative { get; set; }
        public Camera Camera { get; set; }
        public Camera MainCamera { get; set; }
    }
}