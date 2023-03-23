using Mirror;
using Player.Information;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Escapist
{
    public class PhantomBehaviour : APlayerBehaviour
    {
        protected EscapistController escapistController;

        [Header("Test")]
        public int testHat;
        public Color testColor = new Color(0, 0, 0, 0);

        #region Server

        #region Command

        

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
        void SetPhantomSkinTest()
        {
            Color defaultColor = new Color(0, 0, 0, 0);
            SetPhantomSkin(testHat, testColor == defaultColor ? null : testColor);
        }

        [Client]
        public void SetPhantomSkin(int hat = 0, Color? color = null)
        {
            var temp = bodies[actualBody].gameObject.transform.position;
            bodies[actualBody].gameObject.SetActive(false);
            bodies[actualBody].material.color = defaultColor ??
                                                bodies[actualBody].material
                                                    .color;
            if (bodies[actualBody].animator && bodies[actualBody].animator.enabled)
                bodies[actualBody].animator.SetFloat("speed", 0);
            bodies[actualBody].animator.enabled = false;
            bodies[actualBody].networkAnimator.enabled = false;
            actualBody = hat;
            bodies[actualBody].gameObject.transform.position = temp;

            bodies[actualBody].animator.enabled = true;
            bodies[actualBody].networkAnimator.enabled = true;
            bodies[actualBody].gameObject.SetActive(true);
            defaultColor = bodies[actualBody].material.color;
            bodies[actualBody].material.color = color ?? defaultColor ?? bodies[actualBody].material.color;
        }

        #endregion

        #region ClientRpc

        #endregion

        #region Other

        public override void Start()
        {
            base.Start();
            if (isLocalPlayer || isClient)
            {
                CmdSetRole(PlayerRole.Phantom);
                escapistController = new EscapistController();
                escapistController.Escapist.Enable();
                BindTriggers();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!isLocalPlayer) return;
            if (!bodies[actualBody].gameObject.activeSelf)
                SetPhantomSkinTest();
            if (inputVector.magnitude != 0)
                AskToMove(inputVector);
            else
                CmdStopMoving();
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

        private void OnDestroy()
        {
            if (isLocalPlayer)
                UnbindTriggers();
        }

        #endregion
    }
}