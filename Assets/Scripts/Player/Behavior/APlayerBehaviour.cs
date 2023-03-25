using System;
using System.Collections.Generic;
using Menu;
using Mirror;
using Player.Behaviour.Escapist;
using Player.Behaviour.Monster;
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

        [SyncVar] protected PlayerRole actualRole = PlayerRole.Phantom;
        [SyncVar] protected Vector3 cameraRelative;

        protected Vector2 inputVector = new Vector2(0, 0);

        [SerializeField] protected List<body> bodies;

        protected int actualBody = 0;
        [SerializeField] protected float moveSpeed;

        [field: SerializeField] protected float rotateSpeed;

        [field: SerializeField] protected Camera actCamera;

        protected bool reloaded = false;

        protected Color? defaultColor = null;
        protected PlayerInfos playerInfos;

        protected AudioManager audioManager;

        #region Server


        [Server]
        protected virtual void OnDestroy()
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

        [Command]
        private void CmdReloadPlayers()
        {
            reloaded = true;
            foreach(var (key, cliConn) in NetworkServer.connections)
            {
                if (cliConn.identity.TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                {
                    // playerBehaviour.CmdActivateCamera();
                    var actRole = playerBehaviour.GetRole();
                    switch (actRole)
                    {
                        case PlayerRole.Escapist:
                            var escapistBehaviour =
                                (EscapistBehaviour)playerBehaviour;
                            escapistBehaviour.CmdReloadSlime();
                            break;
                        case PlayerRole.Phantom:
                            var phantomBehaviour =
                                (PhantomBehaviour)playerBehaviour;
                            phantomBehaviour.CmdReloadPhantom();
                            break;
                        case PlayerRole.Monster:
                            var monsterBehaviour =
                                (MonsterBehaviour)playerBehaviour;
                            monsterBehaviour.CmdReloadMonster();
                            break;
                        default:
                            break;
                    }
                }
            }
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
            if (isLocalPlayer) {
                ActivateCamera();
            } else
                DesactivateCamera();
        }

        #endregion


        #region Other


        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            if (isLocalPlayer)
            {
                var audioManagers =
                    GameObject.FindGameObjectsWithTag("AudioManager");
                if (audioManagers.Length == 1)
                {
                    if (audioManagers[0]
                        .TryGetComponent<AudioManager>(out var actAudioManager))
                    {
                        audioManager = actAudioManager;
                    }
                }
                if (actCamera != null) {
                    actCamera.gameObject.SetActive(false);
                    cameraRelative = actCamera.transform.position;
                }

                var playerInfosObj =
                    GameObject.FindGameObjectsWithTag("PlayerInfos");
                if (playerInfosObj.Length == 1)
                {
                    if (playerInfosObj[0]
                        .TryGetComponent<PlayerInfos>(out var actPlayerInfos))
                        playerInfos = actPlayerInfos;
                }

            }
        }

        protected virtual void Update()
        {
            if (!actCamera) return;
            if (isLocalPlayer && !reloaded)
                CmdReloadPlayers();
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