using System;
using System.Collections;
using UnityEngine;
using WGR.Abilities;
using WGR.Core.Managers;

namespace WGR.Entities.Player
{
    /* [CLASS DOCUMENTATION]
     * 
     * This is the main skeleton script for the player behaviour.
     * 
     * Implements: IInteractable, IRewindable
     * BASE TYPES: Entity, MonoBehaviour
     * 
     * [Variables]
     * Inpesctor variables: These values must be set from the inspector.
     * Private variables: Mainly behaviour script caching.
     * 
     * [Must know]
     * 1. This script acts as central behaviour manager for the PlayerEntity.
     * 2. The IsActive boolean calles the OnPlayerDeactivate everytime it gets set with a value.
     * 3. Player values are reseted every time he enters the player hub.
     */
    [DefaultExecutionOrder(150)]
    public class PlayerEntity : Entity, IRewindable
    {
        #region PUBLIC_VARIABLES
        [Header("Decal on death")]
        [SerializeField] GameObject bloodDecal;

        bool isDead;
        /// <summary>
        /// If true adds +1 to the playe death value in the GameManager.
        /// </summary>
        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                isDead = value;

                if (value)
                {
                    if (GameManager.S != null)
                    {
                        GameManager.S.PlayerDeaths += 1;
                    }
                }
            }
        }

        bool isActive = false;
        /// <summary>
        /// Everytime the value is set the OnPlayerDeactivate event gets invoked and passes the passed value.
        /// </summary>
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;

                OnPlayerStateChange(IsActive);
            }
        }
        #endregion

        /// <summary>
        /// *PLAYER SYSTEM USE ONLY*
        /// <para>Subscribe to this event to get notified with the current player 
        /// entity active state when it changes.</para>
        /// </summary>
        public event Action<bool> onPlayerStateChange;
        public void OnPlayerStateChange(bool value)
        {
            if (onPlayerStateChange != null)
            {
                onPlayerStateChange(value);
            }
        }

        #region BEHAVIOUR_CACHING
        //Cache player controller script
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
        }
        #endregion

        private void Awake()
        {
            if (GameManager.S != null)
            {
                //Set player script reference to the GameManager.
                GameManager.S.SetPlayerEntity(this);

                //Subscribe to the needed events.
                GameManager.S.GameEventHandler.onSceneChanged += PlayerHubEntry;
                GameManager.S.GameEventHandler.onPlayerSpawn += MoveToSpawnPoint;

                //Cache the necessary player scripts.
                CacheComponents();
            }
        }

        /// <summary>
        /// Method subscribed to the onSceneChanged event.
        /// <para>When called and the focused scene is the PlayerHub, the player active state gets set to false,
        /// the animator speed is reseted to 1, playerLife is set to 1 and the the PlayerAnimations.PlayWakeUpAnimation gets called.</para>
        /// </summary>
        /// <param name="scenes"></param>
        void PlayerHubEntry(GameScenes scenes)
        {
            if (GameManager.S.LevelManager.FocusedScene == GameScenes.PlayerHub)
            {
                IsActive = false;

                PlayerAnimations.GetAnimator().speed = 1f;
                PlayerAnimations.PlayWakeUpAnimation();

                entityLife.SetValue(1);
            }
        }

        /// <summary>
        /// Call to cache every needed player behaviour script.
        /// </summary>
        void CacheComponents()
        {
            PlayerController = GetComponent<PlayerController>();
            PlayerShooting = GetComponentInChildren<PlayerAttack>();
            PlayerAnimations = GetComponent<PlayerAnimations>();
            PlayerKick = GetComponentInChildren<PlayerKick>();
        }

        private void Start()
        {
            IsActive = true;
        }

        /// <summary>
        /// Call to initiate the attack interaction of the player.
        /// <para>If the player is marked dead then return.</para>
        /// <para>If there are rewind ability uses and the player dies then a rewind sequence gets initiated.</para>
        /// <para>If there are not, then the player dies and gets back to the player hub.</para>
        /// </summary>
        public override void AttackInteraction()
        {
            //Prevents the player from dying from multiple shots when he's already dead.
            if (entityLife.GetValue() <= 0) return;

            float tempLife = entityLife.GetValue();

            SetHealth(--tempLife);

            //Should check for rewind availability here
            if (CheckIfDead())
            {
                IsActive = false;

                Instantiate(bloodDecal, transform.position, bloodDecal.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));

                if (GameManager.S != null)
                {
                    if (GameManager.S.AbilityManager.CanRewind(true))
                    {
                        StartCoroutine(RewindOnDeath());
                        return;
                    }
                }
                else Utils.MissingComponent("GameManager", this);

                IsDead = true;
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
            playerAnimations.GetAnimator().speed = 1f;
            playerAnimations.PlayDeathAnimation();
            yield return new WaitForSecondsRealtime(1f);

            playerAnimations.PlayWakeUpAnimation();

            yield return new WaitForSecondsRealtime(1f);

            playerAnimations.SetWalkAnimationState(true);

            if (GameManager.S != null)
            {
                GameManager.S.AIEntityManager.ActivateDetectors();

                if (GameManager.S.LevelManager.FocusedScene.Equals(GameScenes.Level5))
                {

                    GameManager.S.AIEntityManager.ActivateBossPlayerDetectors();
                }
            }
            else Utils.MissingComponent("GameManager", this);

            IsActive = true;
            SetHealth(1);
        }

        /// <summary>
        /// Called when the player really dies and moves him back into the player hub after 2 seconds.
        /// </summary>
        /// <returns></returns>
        IEnumerator DeathSequence()
        {
            PlayerAnimations.PlayDeathAnimation();

            if (GameManager.S != null)
            {
                GameManager.S.HUDHandler.ChangeWeaponInfo(null);
            }
            else Utils.MissingComponent("GameManager", this);

            yield return new WaitForSeconds(2f);

            if (GameManager.S != null)
            {
                GameManager.S.LevelManager.TransitToPlayerHub();
            }
            else Utils.MissingComponent("GameManager", this);

            transform.rotation = Quaternion.Euler(Vector3.up);

            yield return null;
        }

        /// <summary>
        /// Call to set the entity life value.
        /// </summary>
        /// <param name="value">The life value</param>
        void SetHealth(float value)
        {
            entityLife.SetValue(value);
        }

        /// <summary>
        /// Call to check if the entity life value is below zero.
        /// <para>If yes, the GameEventHandler.OnPlayerDeath() method gets called.</para>
        /// </summary>
        /// <returns>True if the value is below zero,false otherwise</returns>
        bool CheckIfDead()
        {
            if (entityLife.GetValue() <= 0)
            {
                if (GameManager.S != null)
                {
                    GameManager.S.GameEventHandler.OnPlayerDeath();
                }
                else Utils.MissingComponent("GameManager", this);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call to initiate the stun interaction of the entity.
        /// </summary>
        /// <param name="hitDirection"></param>
        public override void StunInteraction()
        {
            Debug.Log($"{entityName} got stuned");
        }

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
            GameManager.S.PlayerEntity.IsDead = false;
            GameManager.S.PlayerEntity.IsActive = true;

            playerAnimations.GetAnimator().SetBool("isDead", false);
            playerAnimations.GetAnimator().SetBool("isWakingUp", false);
        }
        #endregion

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged -= PlayerHubEntry;
                GameManager.S.GameEventHandler.onPlayerSpawn -= MoveToSpawnPoint;
            }
        }
    }
}