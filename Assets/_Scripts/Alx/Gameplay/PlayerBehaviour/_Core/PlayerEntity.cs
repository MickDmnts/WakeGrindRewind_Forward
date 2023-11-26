using System;
using System.Collections;
using UnityEngine;
//using WGRF.Abilities;
using WGRF.Core;

namespace WGRF.Entities.Player
{

    [DefaultExecutionOrder(150)]
    public class PlayerEntity : Entity //IRewindable
    {
        ///<summary>The blood decal addressable path of the player</summary>
        [Header("Decal on death")]
        [SerializeField, Tooltip("The blood decal addressable path of the player")]
        string bloodDecalPath;

        ///<summary>Is the player dead?</summary>
        bool isDead;
        ///<summary>Returns the is dead state of the player</summary>
        public bool IsDead => isDead;

        ///<summary>Is the player active?</summary>
        bool isActive = false;
        ///<summary>Returns the is active state of the player</summary>
        public bool IsActive => isActive;

        //REPLACE THIS WITH THE CONTROLLER AND COREBEHAVIOUR SYSTEM -  SEE EXAMPLES FOLDER FOR MCV
        #region BEHAVIOUR_CACHING
        /*        //Cache player controller script
                private PlayerController playerController;
                public PlayerController PlayerController
                {
                    get { return playerController; }
                    set { playerController = value; }
                }

                //Cache player attacking script
                private PlayerAttack playerShooting;
                public PlayerAttack PlayerShooting
                {
                    get { return playerShooting; }
                    set { playerShooting = value; }
                }

                //Cache player animations script
                private PlayerAnimations playerAnimations;
                public PlayerAnimations PlayerAnimations
                {
                    get { return playerAnimations; }
                    set { playerAnimations = value; }
                }

                //Cache player kicking script
                private PlayerKick playerKick;
                public PlayerKick PlayerKick
                {
                    get { return playerKick; }
                    set { playerKick = value; }
                }*/
        #endregion

        protected override void PreAwake()
        {
            if (ManagerHub.S != null)
            {
                //UNCOMMENT WHEN YOY'VE IMPLEMENTED THE CORE BEHAVIOUR AND CONTROLLER SYSTEM ON THE PLAYER,
                //THEN PASS THE CONTROLLER AS AN ARGUMENT
                //ManagerHub.S.AttachPlayerController(this);

                //Subscribe to the needed events.
                ManagerHub.S.GameEventHandler.onPlayerSpawn += MoveToSpawnPoint;
            }
        }

        private void Start()
        { isActive = true; }

        /// <summary>
        /// Call to initiate the attack interaction of the player.
        /// <para>If the player is marked dead then return.</para>
        /// <para>If there are rewind ability uses and the player dies then a rewind sequence gets initiated.</para>
        /// <para>If there are not, then the player dies and gets back to the player hub.</para>
        /// </summary>
        public override void AttackInteraction()
        {
            //Prevents the player from dying from multiple shots when he's already dead.
            if (entityLife <= 0) return;

            float tempLife = entityLife;

            SetHealth(--tempLife);

            //Should check for rewind availability here
            if (CheckIfDead())
            {
                isActive = false;

                //This should use the still WIP addressables system.
                //Instantiate(bloodDecalPath, transform.position, bloodDecalPath.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));

                //Ability system still WIP
                /*if (ManagerHub.S.AbilityManager.CanRewind(true))
                {
                    StartCoroutine(RewindOnDeath());
                    return;
                }*/

                isDead = true;
                StartCoroutine(DeathSequence());
            }
        }

        /// <summary>
        /// Called when the player dies and there are rewind ability uses left.
        /// <para>The routine waits for 2 seconds and revives the player.</para>
        /// <para>If inside the boss room, the boss fov detectors get re-activated from inside AIEntityManager.</para>
        /// <para>Player isActive is set to true at the end.</para>
        /// </summary>
        IEnumerator RewindOnDeath()
        {
            //playerAnimations.GetAnimator().speed = 1f;
            //playerAnimations.PlayDeathAnimation();
            yield return new WaitForSecondsRealtime(1f);

            //playerAnimations.PlayWakeUpAnimation();

            yield return new WaitForSecondsRealtime(1f);

            //playerAnimations.SetWalkAnimationState(true);

            //AI Manager is WIP
            //ManagerHub.S.AIEntityManager.ActivateDetectors();

            isActive = true;
            SetHealth(1);
        }

        /// <summary>
        /// Called when the player really dies and moves him back into the player hub after 2 seconds.
        /// </summary>
        /// <returns></returns>
        IEnumerator DeathSequence()
        {
            //PlayerAnimations.PlayDeathAnimation();

            //UI System stil WIP
            //ManagerHub.S.HUDHandler.ChangeWeaponInfo(null);

            yield return new WaitForSeconds(2f);

            transform.rotation = Quaternion.Euler(Vector3.up);

            yield return null;
        }

        /// <summary>
        /// Call to set the entity life value.
        /// </summary>
        /// <param name="value">The life value</param>
        void SetHealth(float value)
        { entityLife = value; }

        /// <summary>
        /// Call to check if the entity life value is below zero.
        /// <para>If yes, the GameEventHandler.OnPlayerDeath() method gets called.</para>
        /// </summary>
        /// <returns>True if the value is below zero,false otherwise</returns>
        bool CheckIfDead()
        {
            if (entityLife <= 0)
            {
                ManagerHub.S.GameEventHandler.OnPlayerDeath();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call to initiate the stun interaction of the entity.
        /// </summary>
        /// <param name="hitDirection"></param>
        public override void StunInteraction()
        { Debug.Log($"{entityName} got stuned"); }

        #region UTILS
        /// <summary>
        /// Subscribed to the onPlayerSpawn event to move the player on the currently loaded scenes' spawn point
        /// when it loads.
        /// </summary>
        void MoveToSpawnPoint(Vector3 spawnPoint)
        {
            transform.position = spawnPoint;
        }

        /// <summary>
        /// Call to get the current position of the player.
        /// </summary>
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Call to set the current position of the player.
        /// <para>If the player is marked dead, then return.</para>
        /// </summary>
        /// <param name="newPos"></param>
        public void SetPosition(Vector3 newPos)
        {
            if (IsDead) return;

            transform.position = newPos;
        }

        /// <summary>
        /// *ANIMATION EVENT*
        /// <para>Called when the wake up animation finishes playing to reactivate the player controls.</para>
        /// </summary>
        public void EnableUserControlOfPlayer()
        {
            //Change after implemented the CoreBehaviour and Controller
            /*ManagerHub.S.PlayerEntity.IsDead = false;
            ManagerHub.S.PlayerEntity.IsActive = true;

            playerAnimations.GetAnimator().SetBool("isDead", false);
            playerAnimations.GetAnimator().SetBool("isWakingUp", false);*/
        }
        #endregion

        protected override void PreDestroy()
        {
            if (ManagerHub.S != null)
            { ManagerHub.S.GameEventHandler.onPlayerSpawn -= MoveToSpawnPoint; }
        }
    }
}