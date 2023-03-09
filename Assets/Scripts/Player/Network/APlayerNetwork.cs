using Mirror;
using UnityEngine;
using Player.Controller;

namespace Player.Network
{
    public abstract class APlayerNetwork : NetworkBehaviour, IPlayerNetwork
    {
        #region implementation controller
        [field: SerializeField] public APlayerController _controller { get; set; }
        public APlayerController Controller
        { get { return _controller; } set { _controller = value; } }
        #endregion
        #region implementation moveSpeed
        [field: SerializeField] public float _moveSpeed { get; set; }
        public float MoveSpeed 
        { get { return _moveSpeed; } set { _moveSpeed = value; } }
        #endregion
        #region implementation rotateSpeed
        [field: SerializeField] public float _rotateSpeed { get; set; }
        public float RotateSpeed
        { get { return _rotateSpeed; } set { _rotateSpeed = value; } }
        #endregion
        #region implementation camera
        protected Camera _camera;
        public Camera Camera
        { get { return _camera; } set { _camera = value; } }
        #endregion

        protected virtual void Awake()
        {
            _camera = GameObject.FindObjectOfType<Camera>();
        }

        protected void PlayerMovement()
        {
            var targetVector = new Vector3(_controller._inputVector.x, 0,
                _controller._inputVector.y);
            var movementVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
    
            MoveTowardTarget(movementVector);

            RotateTowardMovementVecor(movementVector);
        }

        private void MoveTowardTarget(Vector3 movementVector)
        {
            var speed = _moveSpeed * Time.deltaTime;
            var targetPosition = transform.position + movementVector * speed;
            transform.position = targetPosition;
            var cameraPosition = _camera.transform.position + movementVector * speed;
            _camera.transform.position = cameraPosition;
        }

        private void RotateTowardMovementVecor(Vector3 movementVector)
        {
            if (movementVector.magnitude == 0) { return; }
            var rotation = Quaternion.LookRotation(movementVector);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                rotation, _rotateSpeed);
        }
    }
}