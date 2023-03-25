using System.Collections.Generic;
using Mirror;
using Player.Information;
using Player.Information.Structure;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Escapist
{
    public class EscapistBehaviour : APlayerBehaviour
    {
        [SyncVar] public bool isKilled = false;
        [SyncVar] private int slimeType = 0;
        [SyncVar] private int slimeHat = 0;
        [SyncVar] private Color slimeColor = new Color(0, 0, 0, 0);
        private EscapistController escapistController;
        private static readonly int Speed = Animator.StringToHash("speed");


        #region Server

        #region Command

        [Command(requiresAuthority = false)]
        public void CmdReloadSlime()
        {
            RpcSetSlimeSkin(slimeType, slimeHat, slimeColor);
        }

        [Command]
        private void CmdSetSlime(int newSlimeType, int newSlimeHat, Color newSlimeColor)
        {
            slimeType = newSlimeType;
            slimeHat = newSlimeHat;
            slimeColor = newSlimeColor;
            RpcSetSlimeSkin(slimeType, slimeHat, slimeColor);
        }

        [Command(requiresAuthority = false)]
        public void CmdKilled(GameObject killer)
        {
            isKilled = true;
        }

        [Command]
        public void CmdDestroy()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #endregion

        #region Client

        #region triggers

        #region  triggers bind / unbind

        [Client]
        private void BindTriggers()
        {
            escapistController.Escapist.Movement.performed += OnTriggerMove;
            escapistController.Escapist.Movement.canceled += OnTriggerMove;
        }

        [Client]
        private void UnbindTriggers()
        {
            escapistController.Escapist.Movement.performed -= OnTriggerMove;
            escapistController.Escapist.Movement.canceled -= OnTriggerMove;
        }

        #endregion

        #region triggers move

        [Client]
        private void OnTriggerMove(InputAction.CallbackContext ctx)
        {
            inputVector = ctx.ReadValue<Vector2>();
        }
        #endregion

        #endregion

        [Client]
        public void SetSlimeSkin(Color color)
        {
            bodies[actualBody].animator.enabled = true;
            bodies[actualBody].networkAnimator.enabled = true;
            bodies[actualBody].gameObject.SetActive(true);
            defaultColor = bodies[actualBody].material.color;
            bodies[actualBody].material.color = color;
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        private void RpcSetSlimeSkin(int slime, int hat, Color color)
        {
            if (!bodies[actualBody].gameObject.activeSelf)
            {
                var slimeTypes = new List<int> { 0, 6, 7 };

                if (slime == 1)
                {
                    actualBody = slimeTypes[slime];
                }
                else if (slime is >= 0 and <= 2)
                {
                    actualBody = slimeTypes[slime] + hat;
                }
                SetSlimeSkin(color);
            }
        }

        #endregion

        #region Other

        private void Start()
        {
            if (isLocalPlayer || isClient)
            {
                escapistController = new EscapistController();
                escapistController.Escapist.Enable();
                BindTriggers();
            }
        }
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            if (isLocalPlayer || isClient)
            {
                CmdSetRole(PlayerRole.Escapist);
                if (playerInfos)
                {
                    CmdSetSlime(playerInfos.GetSlimeType(), playerInfos.GetSlimeHat(), playerInfos.GetSlimeColor());
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!isLocalPlayer) return;
            if (inputVector.magnitude != 0)
                AskToMove(inputVector);
            else
                CmdStopMoving();
        }

        public bool IsKilled()
        {
            return isKilled;
        }
        private void OnEnable()
        {
            if (isLocalPlayer)
                escapistController.Escapist.Enable();
        }

        private void OnDisable()
        {
            if (isLocalPlayer)
                escapistController.Escapist.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (isLocalPlayer) {
                UnbindTriggers();
                bodies[actualBody].gameObject.SetActive(false);
                bodies[actualBody].animator.enabled = false;
                bodies[actualBody].networkAnimator.enabled = false;
            }
        }

        #endregion
    }
}