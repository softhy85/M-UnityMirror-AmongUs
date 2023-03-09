using UnityEngine;

namespace Player.Controller
{
    public interface IPlayerController
    {
        public Vector2 InputVector { get; set; }
        public Vector3 MousePosition { get; set; }
    }
}