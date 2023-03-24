using Mirror;
using Player.Behaviour.Escapist;
using Player.Information;
using UDP;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Monster
{
    public class MonsterBehaviour : APlayerBehaviour
    {
        [SerializeField] protected float sprintSpeed;

        protected MonsterController monsterController;
        protected bool[] inputAttack = new bool[3] { false, false, false };
        protected bool inputSprint = false;

        protected bool isAttacking;
        protected float timerAttack = 0;

        [Header("Test")]
        public int testMonster;
        public Color testColor = new Color(0, 0, 0, 0);

        #region Server

        [Server]
        private bool checkAttack()
        {
            var attack = inputAttack[0] || inputAttack[1] ||
                         inputAttack[2];
            if ((timerAttack > 0 || inputSprint) && attack) {
                for (int it = 0; it < 2; it++)
                    inputAttack[it] = false;
                return false;
            } else if (timerAttack <= 0 && attack)
                return true;
            else
                return false;
        }

        #region Command

        [Command]
        public void CmdAttack(int attackType)
        {
            RpcAttack(attackType);
        }

        [Command]
        public void CmdStopAttacking()
        {
            RpcStopAttacking();
        }

        [Command]
        public void CmdAttackPlayer(GameObject player)
        {
            if (isAttacking)
            {
                if (player.TryGetComponent<APlayerBehaviour>(
                        out var playerBehaviour))
                {
                    if (playerBehaviour.GetRole() == PlayerRole.Escapist)
                    {
                        EscapistBehaviour escapistBehaviour = (EscapistBehaviour)playerBehaviour;
                        escapistBehaviour.CmdKilled(gameObject);
                    }
                }
            }
        }

        #endregion

        #region Client

        
        #region triggers

        #region triggers bind / unbind

        [Client]
        private void BindTriggers()
        {
            monsterController.Monster.Movement.performed += OnTriggerMove;
            monsterController.Monster.Movement.canceled += OnTriggerMove;
            monsterController.Monster.Sprint.performed += OnTriggerSprintOn;
            monsterController.Monster.Sprint.canceled += OnTriggerSprintOff;
            monsterController.Monster.Attack0.performed += OnTriggerAttack;
            monsterController.Monster.Attack0.canceled += OnTriggerAttack;
            monsterController.Monster.Attack1.performed += OnTriggerAttack;
            monsterController.Monster.Attack1.canceled += OnTriggerAttack;
            monsterController.Monster.Attack2.performed += OnTriggerAttack;
        }
        [Client]
        private void UnbindTriggers()
        {
            monsterController.Monster.Movement.performed -= OnTriggerMove;
            monsterController.Monster.Movement.canceled -= OnTriggerMove;
            monsterController.Monster.Sprint.performed -= OnTriggerSprintOn;
            monsterController.Monster.Sprint.canceled -= OnTriggerSprintOff;
            monsterController.Monster.Attack0.performed -= OnTriggerAttack;
            monsterController.Monster.Attack0.canceled -= OnTriggerAttack;
            monsterController.Monster.Attack1.performed -= OnTriggerAttack;
            monsterController.Monster.Attack1.canceled -= OnTriggerAttack;
            monsterController.Monster.Attack2.performed -= OnTriggerAttack;
        }

        #endregion

        #region triggers move
        [Client]
        private void OnTriggerMove(InputAction.CallbackContext ctx)
        {
            inputVector = ctx.ReadValue<Vector2>();
        }

        #endregion

        #region triggers sprint
        [Client]
        private void OnTriggerSprintOn(InputAction.CallbackContext ctx)
        {
            inputSprint = true;
        }

        [Client]
        private void OnTriggerSprintOff(InputAction.CallbackContext ctx)
        {
            inputSprint = false;
        }

        #endregion

        #region triggers attacks

        [Client]
        private void OnTriggerAttack(InputAction.CallbackContext ctx)
        {
            switch (ctx.action.name)
            {
                case "Attack 0":
                    inputAttack[0] = true;
                    break;
                case "Attack 1":
                    inputAttack[1] = true;
                    break;
                case "Attack 2":
                    inputAttack[2] = true;
                    break;
            }
        }

        #endregion

        #endregion

        #endregion

        #endregion

        #region Client

        [Client]
        protected override void AskToMove(Vector3 movementVector)
        {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, GetComponent<Camera>().gameObject.transform.eulerAngles.y, 0) * targetVector;
            float speed = 0;
            if (inputSprint)
                speed = sprintSpeed;
            else
                speed = moveSpeed;
            var actualVecSpeed = movementVector * speed;
            var actualSpeed = Mathf.Sqrt(actualVecSpeed.x * actualVecSpeed.x +
                                         actualVecSpeed.y * actualVecSpeed.y);
            var targetPosition = bodies[actualBody].gameObject.transform.position + eulerMovementVector * speed * Time.deltaTime;

            CmdMove(eulerMovementVector, targetPosition, actualSpeed);
        }

        [Client]
        protected void AskToAttack()
        {
            for (int it = 0; it < 2; it++) {
                if (inputAttack[it])
                    CmdAttack(it);
                inputAttack[it] = false;
            }
        }

        [Client]
        void SetMonsterSkinTest()
        {
            Color defaultColor = new Color(0, 0, 0, 0);
            SetMonsterSkin(testMonster, testColor == defaultColor ? null : testColor);
        }

        [Client]
        public void SetMonsterSkin(int monster, Color? color = null)
        {
            var temp = bodies[actualBody].gameObject.transform.position;
            bodies[actualBody].gameObject.SetActive(false);
            bodies[actualBody].material.color = defaultColor ??
                                                bodies[actualBody].material
                                                    .color;
            if (bodies[actualBody].animator.enabled)
                bodies[actualBody].animator.SetFloat("speed", 0);
            bodies[actualBody].animator.enabled = false;
            bodies[actualBody].networkAnimator.enabled = false;
            if (monster == 0)
            {
                actualBody = monster;
            }
            else if (monster == 1)
            {
                actualBody = monster;
            }
            bodies[actualBody].gameObject.transform.position = temp;
            bodies[actualBody].animator.enabled = true;
            bodies[actualBody].networkAnimator.enabled = true;
            bodies[actualBody].gameObject.SetActive(true);
            defaultColor = bodies[actualBody].material.color;
            bodies[actualBody].material.color = color ?? defaultColor ?? bodies[actualBody].material.color;
        }

        [Client]
        public void OnSwordCollider(GameObject objectPlayer)
        {
            if (isAttacking && isLocalPlayer)
            {
                if (objectPlayer.transform.parent.gameObject.TryGetComponent<APlayerBehaviour>(
                        out var playerBehaviour))
                {
                    if (playerBehaviour.GetRole() == PlayerRole.Escapist)
                    {
                        CmdAttackPlayer(objectPlayer.transform.parent.gameObject);
                    }
                }
            }
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        protected void RpcAttack(int attackType)
        {
            timerAttack = 4;
            isAttacking = true;
            var attackName = "attack" + attackType.ToString();
            if (bodies[actualBody].animator.enabled) {
                bodies[actualBody].animator
                    .SetBool("attacking", isAttacking);
                bodies[actualBody].animator.SetTrigger(attackName);
            }
        }

        [ClientRpc]
        protected void RpcStopAttacking()
        {
            if (isLocalPlayer) {
                isAttacking = false;
                if (bodies[actualBody].animator.enabled)
                    bodies[actualBody].animator
                        .SetBool("attacking", false);
            }
        }

        #endregion

        #region Others

        public override void Start()
        {
            base.Start();
            if (isLocalPlayer || isClient) {
                actualRole = PlayerRole.Monster;
                monsterController = new MonsterController();
                monsterController.Monster.Enable();
                BindTriggers();
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!bodies[actualBody].gameObject.activeSelf)
                SetMonsterSkinTest();
            if (!isLocalPlayer) return;
            if (timerAttack > 0)
            {
                timerAttack -= Time.deltaTime;
                if (timerAttack <= 0)
                    CmdStopAttacking();
            }

            if (!bodies[actualBody].gameObject.activeSelf) return;
            if (checkAttack())
                AskToAttack();
            else if (inputVector.magnitude != 0)
                AskToMove(inputVector);
            else
                CmdStopMoving();
        }

        private void OnEnable()
        {
            if (isLocalPlayer)
                monsterController.Monster.Enable();
        }

        private void OnDisable()
        {
            if (isLocalPlayer)
                monsterController.Monster.Disable();
        }

        private void OnDestroy()
        {
            if (isLocalPlayer)
                UnbindTriggers();
        }

        #endregion
    }

}