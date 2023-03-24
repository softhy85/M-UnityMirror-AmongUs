using System;
using System.Collections.Generic;
using Mirror;
using Player.Information;
using UnityEngine;

namespace Player.Behaviour
{
    [Serializable]
    public struct body
    {
        public GameObject gameObject;
        public Material material;
        public NetworkAnimator networkAnimator;
        public Animator animator;
    }

    public class APlayerBehaviour : NetworkBehaviour
    {

        [SyncVar] protected PlayerRole actualRole;
        [SyncVar] protected Vector3 cameraRelative;

        protected Vector2 inputVector = new Vector2(0, 0);

        [SerializeField] protected List<body> bodies;

        protected int actualBody = 0;
        [SerializeField] protected float moveSpeed;

        [field: SerializeField] protected float rotateSpeed;

        [field: SerializeField] protected Camera actCamera;

        protected Camera mainCamera;

        protected Color? defaultColor = null;


        #region Server


        [Server]
        void OnDestroy()
        {
            DesactivateCamera();
        }

        #region Command

        [Command]
        protected void CmdSetRole(PlayerRole newRole) {
            actualRole = newRole;
        }

        [Command]
        public void CmdMove(Vector3 movementVector, Vector3 targetVector, float actualSpeed)
        {
            RpcMove(movementVector, targetVector, actualSpeed);
        }

        [Command]
        public void CmdStopMoving()
        {
            RpcStopMoving();
        }

        [Command]
        public void CmdActivateCamera()
        {
            RpcActivateCamera();
        }

        #endregion

        #endregion

        #region Client

        [Client]
        protected virtual void AskToMove(Vector3 movementVector)
        {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, actCamera.gameObject.transform.eulerAngles.y, 0) * targetVector;
            var speed = moveSpeed;
            var actualVecSpeed = movementVector * speed;
            var actualSpeed = Mathf.Sqrt(actualVecSpeed.x * actualVecSpeed.x +
                                         actualVecSpeed.y * actualVecSpeed.y);
            var targetPosition = bodies[actualBody].gameObject.transform.position + eulerMovementVector * speed * Time.deltaTime;
            CmdMove(eulerMovementVector, targetPosition, actualSpeed);
        }

        [Client]
        private void MoveTowardTarget(Vector3 targetPosition, float actualSpeed)
        {
            if (bodies[actualBody].animator.enabled)
                bodies[actualBody].animator.SetFloat("speed", actualSpeed);
            bodies[actualBody].gameObject.transform.position = targetPosition;
            actCamera.transform.position = targetPosition + cameraRelative;
        }

        [Client]
        private void RotateTowardMovementVector(Vector3 movementVector)
        {
            if (movementVector.magnitude == 0) { return; }
            var rotation = Quaternion.LookRotation(movementVector);
            bodies[actualBody].gameObject.transform.rotation = Quaternion.RotateTowards(bodies[actualBody].gameObject.transform.rotation,
                rotation, rotateSpeed);
        }

        [Client]
        public void ActivateCamera()
        {
            if (isLocalPlayer) {
                if (actCamera != null) {
                    actCamera.gameObject.SetActive(true);
                    // cameraRelative = actCamera.transform.position;
                }
                if (mainCamera != null)
                    mainCamera.gameObject.SetActive(false);
            }
        }

        [Client]
        private void DesactivateCamera()
        {
            if (actCamera != null)
                actCamera.gameObject.SetActive(false);
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        protected void RpcMove(Vector3 movementVector, Vector3 targetVector, float actualSpeed)
        {
            // Debug.Log("Test Moving");
            // Debug.Log("targetVector - " + targetVector);
            // Debug.Log("actualSpeed - " + actualSpeed);
            MoveTowardTarget(targetVector, actualSpeed);
            RotateTowardMovementVector(movementVector);
        }

        [ClientRpc]
        protected void RpcStopMoving()
        {
            if (bodies[actualBody].animator.enabled)
                bodies[actualBody].animator.SetFloat("speed", 0);
        }

        [ClientRpc]
        protected void RpcActivateCamera()
        {
            Debug.Log("Camara - LocalPlayer - " + isLocalPlayer);
            if (isLocalPlayer) {
                ActivateCamera();
            } else
                DesactivateCamera();
        }

        #endregion


        #region Other

        public virtual void Start()
        {
            if (isLocalPlayer)
            {
                mainCamera = Camera.main;
                if (actCamera != null)
                    actCamera.gameObject.SetActive(false);
                cameraRelative = actCamera.transform.position;
            }
        }

        protected virtual void Update()
        {
            if (!actCamera) return;
            if (isLocalPlayer && !actCamera.gameObject.activeSelf)
                ActivateCamera();
            else if (!isLocalPlayer && actCamera.gameObject.activeSelf)
                DesactivateCamera();
        }


        public PlayerRole GetRole()
        {
            return actualRole;
        }

        #endregion
    }
}