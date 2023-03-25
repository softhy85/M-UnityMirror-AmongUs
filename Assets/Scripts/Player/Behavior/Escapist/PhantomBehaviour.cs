using Menu;
using Mirror;
using Player.Information;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Escapist
{
    public class PhantomBehaviour : APlayerBehaviour
    {
        [SyncVar] private int phantomHat;
        [SyncVar] private Color phantomColor = new Color(0, 0, 0, 0);

        private EscapistController escapistController;
        private static readonly int Speed = Animator.StringToHash("speed");

        #region Server

        #region Command

        [Command(requiresAuthority = false)]
        public void CmdReloadPhantom()
        {
            RpcSetPhantomSkin(phantomHat, phantomColor);
        }

        [Command]
        public void CmdSetPhantom(int newPhantomHat, Color newPhantomColor)
        {
            phantomHat = newPhantomHat;
            phantomColor = newPhantomColor;
            RpcSetPhantomSkin(phantomHat, phantomColor);
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

        [Client]
        public void SetPhantomSkin(Color color)
        {

            bodies[actualBody].animator.enabled = true;
            bodies[actualBody].networkAnimator.enabled = true;
            bodies[actualBody].gameObject.SetActive(true);
            defaultColor = bodies[actualBody].material.color;
            bodies[actualBody].material.color = color;
        }

        #endregion

        #endregion

        #endregion

        #region ClientRpc

        [ClientRpc]
        private void RpcSetPhantomSkin(int hat, Color color)
        {
            if (!bodies[actualBody].gameObject.activeSelf)
            {
                if (NetworkClient.connection.identity
                    .TryGetComponent<APlayerBehaviour>(out var playerBehaviour))
                {
                    if (playerBehaviour.GetRole() == PlayerRole.Monster)
                    {
                        return;
                    }
                }
                actualBody = hat;
                SetPhantomSkin(color);
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
                CmdSetRole(PlayerRole.Phantom);
                escapistController = new EscapistController();
                escapistController.Escapist.Enable();
                if (playerInfos) {
                    CmdSetPhantom(playerInfos.GetSlimeHat(), playerInfos.GetSlimeColor());
                }
            }
        }

        protected override void Update()
        {
            if (bodies[actualBody].gameObject.activeSelf) {
                base.Update();
                if (audioManager.GetActualMusic() != MusicType.EscapistCalmMusic)
                    audioManager.StartMusic(MusicType.EscapistCalmMusic);
                if (!isLocalPlayer) return;
                if (inputVector.magnitude != 0)
                    AskToMove(inputVector);
                else
                    CmdStopMoving();
            }
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
            if (isLocalPlayer)
                UnbindTriggers();
        }

        #endregion
    }
}