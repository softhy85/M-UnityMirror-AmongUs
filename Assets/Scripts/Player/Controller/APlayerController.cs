using System;
using UnityEngine;

namespace Player.Controller {
    public abstract class APlayerController : MonoBehaviour, IPlayerController
    {
        #region implementation inputVector
        public Vector2 _inputVector;
        public Vector2 InputVector
        { get => _inputVector; set => _inputVector = value; }
        #endregion
        #region implementation mousePosition
        public Vector3 _mousePosition;
        public Vector3 MousePosition
        { get => _mousePosition; set => _mousePosition = value; }
        #endregion

        void Update()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            _inputVector = new Vector2(h, v);
            _mousePosition = Input.mousePosition;
        }
    }
}