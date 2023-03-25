using Menu;
using Mirror;
using Player.Behaviour.Escapist;
using Player.Information;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Behaviour.Monster
{
    public class MonsterBehaviour : APlayerBehaviour
    {
        [SyncVar] private int monsterType = 0;
        [SyncVar] private Color monsterColor = new Color(0, 0, 0, 0);
        [SerializeField] private float sprintSpeed;

        private MonsterController monsterController;
        private bool[] inputAttack = new bool[3] { false, false, false };
        private bool inputSprint = false;

        private bool isAttacking;
        private float timerAttack = 0;
        private static readonly int Attacking = Animator.StringToHash("attacking");
        private static readonly int Speed = Animator.StringToHash("speed");

        #region Server

        [Server]
        private bool CheckAttack()
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

        [Command(requiresAuthority = false)]
        public void CmdReloadMonster()
        {
            RpcSetMonsterSkin(monsterType, monsterColor);
        }

        [Command]
        private void CmdSetMonster(int newMonsterType, Color newMonsterColor)
        {
            monsterType = newMonsterType;
            monsterColor = newMonsterColor;
            RpcSetMonsterSkin(monsterType, monsterColor);
        }

        [Command]
        private void CmdAttack(int attackType)
        {
            RpcAttack(attackType);
        }

        [Command]
        private void CmdStopAttacking()
        {
            RpcStopAttacking();
        }

        [Command]
        private void CmdAttackPlayer(GameObject player)
        {
            if (!isAttacking) return;
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

        #endregion

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
            if (!isLocalPlayer) return;
            inputVector = ctx.ReadValue<Vector2>();
        }

        #endregion

        #region triggers sprint
        [Client]
        private void OnTriggerSprintOn(InputAction.CallbackContext ctx)
        {
            if (!isLocalPlayer) return;
            inputSprint = true;
        }

        [Client]
        private void OnTriggerSprintOff(InputAction.CallbackContext ctx)
        {
            if (!isLocalPlayer) return;
            inputSprint = false;
        }

        #endregion

        #region triggers attacks

        [Client]
        private void OnTriggerAttack(InputAction.CallbackContext ctx)
        {
            if (!isLocalPlayer) return;
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

        [Client]
        public GameObject getActualBody()
        {
            return bodies[actualBody].gameObject;
        }

        [Client]
        protected override void AskToMove(Vector3 movementVector)
        {
            var targetVector = new Vector3(movementVector.x, 0,
                movementVector.y);
            var eulerMovementVector = Quaternion.Euler(0, actCamera.transform.eulerAngles.y, 0) * targetVector;
            var speed = inputSprint ? sprintSpeed : moveSpeed;
            var actualVecSpeed = movementVector * speed;
            var actualSpeed = Mathf.Sqrt(actualVecSpeed.x * actualVecSpeed.x +
                                         actualVecSpeed.y * actualVecSpeed.y);
            var targetPosition = bodies[actualBody].gameObject.transform.position + eulerMovementVector * (speed * Time.deltaTime);

            CmdMove(eulerMovementVector, targetPosition, actualSpeed);
        }

        [Client]
        private void AskToAttack()
        {
            for (int it = 0; it < 2; it++) {
                if (inputAttack[it])
                    CmdAttack(it);
                inputAttack[it] = false;
            }
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
        [Client]
        public void SetMonsterSkin(Color color)
        {
            bodies[actualBody].animator.enabled = true;
            bodies[actualBody].networkAnimator.enabled = true;
            bodies[actualBody].gameObject.SetActive(true);
            defaultColor = bodies[actualBody].material.color;
            bodies[actualBody].material.color = color;
        }

        [Client]
        protected override void MoveTowardTarget(Vector3 targetPosition, float actualSpeed)
        {
            base.MoveTowardTarget(targetPosition, actualSpeed);
            if (!isLocalPlayer) return;
            if (actualSpeed < sprintSpeed)
                audioManager.StartSound(SoundType.MonsterWalk);
            else
                audioManager.StartSound(SoundType.MonsterRun);
        }

        #endregion

        #region ClientRpc

        [ClientRpc]
        private void RpcSetMonsterSkin(int monster, Color color)
        {
            if (!bodies[actualBody].gameObject.activeSelf)
            {
                actualBody = monster;
                SetMonsterSkin(color);
            }
        }


        [ClientRpc]
        private void RpcAttack(int attackType)
        {
            timerAttack = 4;
            isAttacking = true;
            var attackName = "attack" + attackType.ToString();
            if (bodies[actualBody].animator.enabled) {
                if (isLocalPlayer)
                {
                    audioManager.StartSound(SoundType.MonsterAttack);
                }
                bodies[actualBody].animator
                    .SetBool(Attacking, isAttacking);
                bodies[actualBody].animator.SetTrigger(attackName);
            }

        }

        [ClientRpc]
        private void RpcStopAttacking()
        {
            if (isLocalPlayer) {
                isAttacking = false;
                if (bodies[actualBody].animator.enabled)
                    bodies[actualBody].animator
                        .SetBool(Attacking, false);
            }
        }

        #endregion

        #region Others

        private void Start()
        {
            if (isLocalPlayer || isClient)
            {
                monsterController = new MonsterController();
                monsterController.Monster.Enable();
                BindTriggers();
            }
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            actualRole = PlayerRole.Monster;
            if (isLocalPlayer || isClient) {
                CmdSetRole(PlayerRole.Monster);
                if (playerInfos)
                {
                    CmdSetMonster(playerInfos.GetMonsterType(),
                        playerInfos.GetMonsterColor());
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            if (!isLocalPlayer) return;
            if (pauseMenu.IsOpen()) return;
            if (audioManager?.GetActualMusic() != MusicType.MonsterMusic)
                audioManager.StartMusic(MusicType.MonsterMusic);
            if (timerAttack > 0)
            {
                timerAttack -= Time.deltaTime;
                if (timerAttack <= 0)
                    CmdStopAttacking();
            }

            if (!bodies[actualBody].gameObject.activeSelf) return;
            if (CheckAttack())
                AskToAttack();
            else if (inputVector.magnitude != 0)
                AskToMove(inputVector);
            else
                CmdStopMoving();
        }

        private void OnEnable()
        {
            if (!isLocalPlayer) return;
            monsterController?.Monster.Enable();
        }

        private void OnDisable()
        {
            if (!isLocalPlayer) return;
            monsterController?.Monster.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!isLocalPlayer) return;
            monsterController?.Monster.Disable();
            UnbindTriggers();
        }

        #endregion
    }

}