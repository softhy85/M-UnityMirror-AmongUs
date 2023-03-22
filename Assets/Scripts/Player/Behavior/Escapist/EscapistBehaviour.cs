using Mirror;
using Player.Information;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Escapist
{
    public class EscapistBehaviour : APlayerBehaviour
    {
        [SyncVar] public bool isKilled = false;
        protected EscapistController escapistController;

        [Header("Test")]
        public int testSlime;
        public int testHat;
        public Color testColor = new Color(0, 0, 0, 0);

        #region Server

        #region Command

        public void CmdKilled(GameObject killer)
        {
            isKilled = true;
        }

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
        void SetSlimeSkinTest()
        {
            Color blankColor = new Color(0, 0, 0, 0);
            SetSlimeSkin(testSlime, testHat, testColor == blankColor ? null : testColor);
        }

        [Client]
        private void SetSlimeSkin(int slime, int hat = 0, Color? color = null)
        {
            int[] slimeType = new int[3] { 0, 6, 7 };
            
            var temp = bodies[actualBody].gameObject.transform.position;
            bodies[actualBody].gameObject.SetActive(false);
            bodies[actualBody].material.color = defaultColor ??
                                                bodies[actualBody].material
                                                    .color;
            if (bodies[actualBody].animator && bodies[actualBody].animator.enabled)
                bodies[actualBody].animator.SetFloat("speed", 0);
            bodies[actualBody].animator.enabled = false;
            bodies[actualBody].networkAnimator.enabled = false;
            if (slime == 1)
            {
                actualBody = slimeType[slime];
            }
            else if (slime is >= 0 and <= 2)
            {
                actualBody = slimeType[slime] + hat;
            }
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
                actualRole = PlayerRole.Escapist;
                escapistController = new EscapistController();
                escapistController.Escapist.Enable();
                BindTriggers();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!bodies[actualBody].gameObject.activeSelf)
                SetSlimeSkinTest();
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

        private void OnDestroy()
        {
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