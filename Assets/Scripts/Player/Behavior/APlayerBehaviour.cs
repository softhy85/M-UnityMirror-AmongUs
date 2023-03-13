using System;
using UnityEngine;
using Player.Controller;
using Player.Network;

namespace Player.Behaviour
{
    public class APlayerBehaviour : MonoBehaviour, IPlayerBehaviour
    {
        #region imple player network
         protected PlayerNetwork _playerNetwork;
         public PlayerNetwork PlayerNetwork
        { get => _playerNetwork; set => _playerNetwork = value; }
        #endregion
        #region imple body
        [field: SerializeField] protected GameObject _body;

        public GameObject Body
        { get => _body; set => _body = value; }
        #endregion
        #region imple controller

        [field: SerializeField] protected APlayerController _controller;
        public APlayerController Controller 
        { get => _controller; set => _controller = value; }
        #endregion
        #region imple moveSpeed

        [field: SerializeField] protected float _moveSpeed;
        public float MoveSpeed 
        { get => _moveSpeed; set => _moveSpeed = value; }
        #endregion
        #region imple rotateSpeed

        [field: SerializeField] protected float _rotateSpeed;
        public float RotateSpeed
        { get => _rotateSpeed; set => _rotateSpeed = value; }
        #endregion
        #region imple camera relative

        protected Vector3 _cameraRelative;
        public Vector3 CameraRelative
        { get => _cameraRelative; set => _cameraRelative = value; }
        #endregion
        #region imple camera

        [field: SerializeField] protected Camera _camera;
        public Camera Camera
        { get => _camera; set => _camera = value; }
        #endregion
        #region imple main camera

        protected Camera _mainCamera;
        public Camera MainCamera 
        { get => _mainCamera; set => _mainCamera = value; }
        #endregion

        protected void Awake()
        {
            _playerNetwork = this.gameObject.transform.parent
                .GetComponent<PlayerNetwork>();
            _mainCamera = Camera.main;
            if (_camera != null)
                _camera.gameObject.SetActive(false);
            _cameraRelative = _camera.transform.position;
        }

        protected virtual void Update()
        {
            if (_controller._inputVector.magnitude != 0)
            {
                AskToMove(_controller._inputVector);
            }
        }
        private void AskToMove(Vector3 movementVector) {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            var speed = _moveSpeed * Time.deltaTime;
            var targetPosition = _body.transform.position + movementVector * speed;
            _playerNetwork.CmdMove(eulerMovementVector);
        }

        public void ActivateCamera()
        {
            if (_camera != null)
                _camera.gameObject.SetActive(true);
            if (_mainCamera != null)
                _mainCamera.gameObject.SetActive(false);
        }

        public void DesactivateCamera()
        {
            if (_mainCamera != null)
                _mainCamera.gameObject.SetActive(true);
        }
        public void MoveTowardTarget(Vector3 movementVector)
        {
            var speed = _moveSpeed * Time.deltaTime;
            var targetPosition = _body.transform.position + movementVector * speed;
            _body.transform.position = targetPosition;
            _camera.transform.position = targetPosition + _cameraRelative;
        }

        public void RotateTowardMovementVector(Vector3 movementVector)
        {
            if (movementVector.magnitude == 0) { return; }
            var rotation = Quaternion.LookRotation(movementVector);
            _body.transform.rotation = Quaternion.RotateTowards(_body.transform.rotation,
                rotation, _rotateSpeed);
        }
    }
}