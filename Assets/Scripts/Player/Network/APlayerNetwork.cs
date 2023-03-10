using Mirror;
using UnityEngine;
using Player.Controller;
using Unity.VisualScripting;

namespace Player.Network
{
    public abstract class APlayerNetwork : NetworkBehaviour, IPlayerNetwork
    {
        #region imple body
        [field: SerializeField] public GameObject _body { get; set; }
        public GameObject Body
        { get { return _body; } set { _body = value; } }
        #endregion
        #region imple controller
        [field: SerializeField] public APlayerController _controller { get; set; }
        public APlayerController Controller
        { get { return _controller; } set { _controller = value; } }
        #endregion
        #region imple moveSpeed
        [field: SerializeField] public float _moveSpeed { get; set; }
        public float MoveSpeed 
        { get { return _moveSpeed; } set { _moveSpeed = value; } }
        #endregion
        #region imple rotateSpeed
        [field: SerializeField] public float _rotateSpeed { get; set; }
        public float RotateSpeed
        { get { return _rotateSpeed; } set { _rotateSpeed = value; } }
        #endregion
        #region imple camera relative
        protected Vector3 _cameraRelative { get; set; }
        public Vector3 CameraRelative
        { get { return _cameraRelative; } set { _cameraRelative = value; } }
        #endregion
        #region imple camera
        [field: SerializeField] public Camera _camera { get; set; }
        public Camera Camera
        { get { return _camera; } set { _camera = value; } }
        #endregion
        #region imple main camera
        protected Camera _mainCamera { get; set; }
        public Camera MainCamera
        { get { return _mainCamera; } set { _mainCamera = value; } }
        #endregion

        #region Server
        [Command]
        protected void CmdMove(Vector3 movementVector)
        {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            var speed = _moveSpeed * Time.deltaTime;
            var targetPosition = _body.transform.position + movementVector * speed;

            RpcMove(eulerMovementVector);
        }
        #endregion

        #region Client Rpc
        [ClientRpc]
        protected void RpcMove(Vector3 movementVector)
        {
            MoveTowardTarget(movementVector);
            RotateTowardMovementVector(movementVector);
        }
        #endregion

        #region Client
        [Client]
        protected virtual void Start()
        {
            if (!isLocalPlayer)
            {
                _camera.gameObject.SetActive(false);
            } else {
                _mainCamera = Camera.main;
                if (_mainCamera != null)
                    _mainCamera.gameObject.SetActive(false);
                _camera.gameObject.SetActive(true);
                _cameraRelative = _camera.transform.position;
            }
        }

        [ClientCallback]
        protected virtual void Update()
        {
            if (!isOwned) return;
            _camera.transform.position = _body.transform.position + _cameraRelative;
            if (_controller._inputVector.magnitude != 0)
            {
                CmdMove(_controller._inputVector);
            }
        }

        [Client]
        void OnDestroy()
        {
            _camera.gameObject.SetActive(false);
            if (_mainCamera != null)
                _mainCamera.gameObject.SetActive(true);
        }

        [Client]
        private void MoveTowardTarget(Vector3 movementVector)
        {
            var speed = _moveSpeed * Time.deltaTime;
            var targetPosition = _body.transform.position + movementVector * speed;
            _body.transform.position = targetPosition;
        }

        [Client]
        private void RotateTowardMovementVector(Vector3 movementVector)
        {
            if (movementVector.magnitude == 0) { return; }
            var rotation = Quaternion.LookRotation(movementVector);
            _body.transform.rotation = Quaternion.RotateTowards(_body.transform.rotation,
                rotation, _rotateSpeed);
        }
        #endregion
    }
}