using System.Collections.Generic;
using Menu;
using Mirror;
using Player.Behaviour.Monster;
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

        [Client]
        private int GetNearbyDistanceMonster()
        {
            var distanceMonster = -1;
            var actPos = new Vector2(bodies[actualBody].gameObject.transform.position.x,
                bodies[actualBody].gameObject.transform.position.y);
            var monstersObj =
                GameObject.FindGameObjectsWithTag("PlayerMonster");
            foreach (var monsterObj in monstersObj)
            {
                if (!monsterObj.TryGetComponent<MonsterBehaviour>(
                        out var monsterBehaviour)) continue;
                var monsterBody = monsterBehaviour.getActualBody();
                var monsterPos = new Vector2(monsterBody.transform.position.x, monsterBody.transform.position.y);
                var actDistanceVec =
                    new Vector2(Mathf.Abs(monsterPos.x - actPos.x),
                        Mathf.Abs(monsterPos.y - actPos.y));
                var actDistance =
                    Mathf.Sqrt(actDistanceVec.x * actDistanceVec.x +
                               actDistanceVec.y * actDistanceVec.y);
                if (distanceMonster > actDistance || distanceMonster == -1)
                    distanceMonster = (int)Mathf.Round(actDistance);
            }
            return distanceMonster;
        }

        [Client]
        private void SetMusicMonster()
        {
            var distanceMonster = GetNearbyDistanceMonster();
            var actualMusic = audioManager.GetActualMusic();

            if (distanceMonster == -1) return;
            if (distanceMonster < 5 && actualMusic != MusicType.EscapistMonsterChasingMusic)
            {
                audioManager.StartMusic(MusicType.EscapistMonsterChasingMusic);
            } else if (distanceMonster is >= 5 and < 10 &&
                       actualMusic != MusicType.EscapistMonsterNearbyMusic)
            {
                audioManager.StartMusic(MusicType.EscapistMonsterNearbyMusic);
            } else if (distanceMonster is >= 10 and < 20  &&
                       actualMusic != MusicType.EscapistMonsterMusic)
            {
                audioManager.StartMusic(MusicType.EscapistMonsterMusic);
            } else if (distanceMonster >= 20 && actualMusic != MusicType.EscapistCalmMusic)
            {
                audioManager.StartMusic(MusicType.EscapistCalmMusic);
            }
        }

        [Client]
        protected override void MoveTowardTarget(Vector3 targetPosition, float actualSpeed)
        {
            base.MoveTowardTarget(targetPosition, actualSpeed);
            if (!isLocalPlayer) return;
            audioManager.StartSound(SoundType.SlimeJump);
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
            SetMusicMonster();
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