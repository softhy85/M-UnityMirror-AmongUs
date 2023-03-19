using System;
using System.Collections.Generic;
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
        #region imple bodies
        [field: SerializeField] protected List<body> _bodies;

        public List<body> Bodies
        { get => _bodies; set => _bodies = value; }
        #endregion
        #region imple actual body
        protected int _actualBody = 0;
        public int ActualBody
        { get => _actualBody; set => _actualBody = value; }
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

        protected Color? _defaultColor = null;

        public int test_slime;
        public int test_hat;
        public Color test_color = Color.black;

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
            else
            {
                var animator = _bodies[_actualBody].gameObject.GetComponent<Animator>();
                animator.SetFloat("speed", 0);
            }
        }
        private void AskToMove(Vector3 movementVector) {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            var speed = _moveSpeed * Time.deltaTime;
            var actualSpeed = movementVector * speed;
            var animator = _bodies[_actualBody].gameObject.GetComponent<Animator>();
            animator.SetFloat("speed", Mathf.Sqrt(actualSpeed.x * actualSpeed.x +
                                                  actualSpeed.y * actualSpeed.y));
            var targetPosition = _bodies[_actualBody].gameObject.transform.position + movementVector * speed;
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
            var targetPosition = _bodies[_actualBody].gameObject.transform.position + movementVector * speed;
            _bodies[_actualBody].gameObject.transform.position = targetPosition;
            _camera.transform.position = targetPosition + _cameraRelative;
        }

        public void RotateTowardMovementVector(Vector3 movementVector)
        {
            if (movementVector.magnitude == 0) { return; }
            var rotation = Quaternion.LookRotation(movementVector);
            _bodies[_actualBody].gameObject.transform.rotation = Quaternion.RotateTowards(_bodies[_actualBody].gameObject.transform.rotation,
                rotation, _rotateSpeed);
        }

        [ContextMenu("SetSkinTest")]
        void SetSkinTest()
        {
            Color defaultColor = new Color(0, 0, 0, 0);
            SetSkin(test_slime, test_hat, test_color == defaultColor ? null : test_color);
        }
        public void SetSkin(int slime, int hat = 0, Color? color = null)
        {
            int[] slimeType = new int[3] { 0, 6, 7 };
            
            var temp = _bodies[_actualBody].gameObject.transform.position;
            _bodies[_actualBody].gameObject.SetActive(false);
            _bodies[_actualBody].material.color = _defaultColor ??
                                                  _bodies[_actualBody].material
                                                      .color;
            var animator = _bodies[_actualBody].gameObject.GetComponent<Animator>();
            var speed = animator.GetFloat("speed");
            animator.SetFloat("speed", 0);
            animator.enabled = false;
            if (slime == 1)
            {
                _actualBody = slimeType[slime];
            }
            else if (slime is >= 0 and <= 2)
            {
                _actualBody = slimeType[slime] + hat;
            }
            _bodies[_actualBody].gameObject.transform.position = temp;
            animator = _bodies[_actualBody].gameObject.GetComponent<Animator>();
            animator.SetFloat("speed", speed);
            animator.enabled = true;
            _bodies[_actualBody].gameObject.SetActive(true);
            _defaultColor = _bodies[_actualBody].material.color;
            _bodies[_actualBody].material.color = color ?? _defaultColor ?? _bodies[_actualBody].material.color;
        }
    }
}