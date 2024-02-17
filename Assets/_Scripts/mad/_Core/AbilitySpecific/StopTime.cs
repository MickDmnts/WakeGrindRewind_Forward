using System;
using UnityEngine;
using WGRF.AI;
using WGRF.Core;

namespace WGRF.Abilities
{
    /// <summary>
    /// The time stop ability
    /// </summary>
    public class StopTime : Ability
    {
        ///<summary>Cached method called when the ability finishes execution</summary>
        Action onAbilityFinishCallback;

        /// <summary>
        /// Creates a StopTime instance.
        /// </summary>
        /// <param name="name">The name of the ability</param>
        /// <param name="tier">The starting tier of the ability</param>
        /// <param name="abilitySprite">The ability sprite</param>
        /// <param name="isUnlocked">Is the ability unlocked initialy?</param>
        public StopTime(string name, int tier, Sprite abilitySprite, bool isUnlocked)
        {
            this.AbilityName = name;
            this.AbilityDescription = "UPDATE TEXT PER TIER";

            this.AbilityTier = tier;

            this.MaxAbilityTier = 3;
            this.ActiveTime = 0;

            this.AbilitySprite = abilitySprite;

            this.IsUnlocked = isUnlocked;
        }

        /// <summary>
        /// Call to startup any basic ability behaviour.
        /// </summary>
        /// <param name="onAbilityFinishCallback">The method to execute when the ability ends</param>
        public override void Start(Action onAbilityFinishCallback)
        {
            this.onAbilityFinishCallback = onAbilityFinishCallback;

            UpdateStatsPerTier();

            this.CanActivate = true;
        }

        ///<summary>Disable the ability on any scene change.</summary>
        public void DisableBehaviourOnSceneChange()
        {
            OnAbilityFinished();
        }

        ///<summary>Fully enables the ability</summary>
        protected override void EnableAbility()
        {
            IsUnlocked = true;
            CanActivate = true;
        }

        /// <summary>
        /// Call to Activate the ability behaviour.
        /// </summary>
        public override bool TryActivate()
        {
            if (ManagerHub.S.AbilityManager.AbilitiesPerRoom <= 0 || !IsUnlocked)
                return false;

            timer = ActiveTime;

            //Change the bullet current speed to simulate a stop time feeling.
            ManagerHub.S.InternalTime.ChangeTimeScale(0.0f, ActiveTime);

            //Decrease THIS runs remaining ability uses.
            ManagerHub.S.AbilityManager.DecreaseAbilityUses();
            //Update remaining uses UI
            ManagerHub.S.HUDHandler.SetAbilityUses(ManagerHub.S.AbilityManager.AbilitiesPerRoom);

            AbilitySpecificActions();

            PlayAbilitySound();

            return true;
        }

        ///<summary>Play this ability's SFX</summary>
        protected override void PlayAbilitySound()
        {
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.StopTime);
        }

        /// <summary>
        /// Call to make changes and actions based on the ability behaviour
        /// </summary>
        void AbilitySpecificActions()
        {
            //CHANGE AFTER TIME MANAGER CREATION
            int ar = ManagerHub.S.ActiveRoom;
            foreach (AIEntity enemy in ManagerHub.S.AIHandler.GetRoomAgents(ar))
            {
                if (enemy == null) continue;

                enemy.Agent.speed = 0;
                enemy.Agent.angularSpeed = 0;

                // enemy.Controller.Access<EnemyAnimations>("eAnimations").SetAnimatorPlaybackSpeed(0f);

                //enemy.DisableShootingBehaviour();
            }
        }

        /// <summary>
        /// Call to update the tick of the ability script, just like running an Update() from MonoBehaviour.
        /// </summary>
        public override void UpdateAbilityTick()
        {
            //the timer will be used in the UI timer reference
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                //Call to revert back to normal gameplay.
                OnAbilityFinished();
            }
        }

        /// <summary>
        /// Called when the ability timer has reached zero to revert back any changes the abiltiy has made.
        /// </summary>
        protected override void OnAbilityFinished()
        {
            CanActivate = true;

            //reset the timer
            timer = ActiveTime;

            //call the external method on ability finish.
            onAbilityFinishCallback();

            int ar = ManagerHub.S.ActiveRoom;
            foreach (EnemyEntity enemy in ManagerHub.S.AIHandler.GetRoomAgents(ar))
            {
                if (enemy == null) continue;

                enemy.OnPlayerAbilityFinish();
            }

            ManagerHub.S.GameEventHandler.OnAbilityEnd();
        }

        /// <summary>
        /// Call to increase the AbilityTier by 1 level only if the current AbilityTier is smaller than MaxAbilityTier.
        /// </summary>
        public override void UpgradeAbility()
        {
            //Enable the ability if it is still locked.
            if (!IsUnlocked)
            {
                EnableAbility();
            }

            AbilityTier++;

            //Refresh the ability stats to the new tier stats.
            UpdateStatsPerTier();
        }

        /// <summary>
        /// Call after the construction of the Ability to set 
        /// the ActiveTime and UsesPerLevel based on the ability tier.
        /// </summary>
        public override void UpdateStatsPerTier()
        {
            switch (AbilityTier)
            {
                case 1:
                    ActiveTime = 2;
                    break;

                case 2:
                    ActiveTime = 3;
                    break;

                case 3:
                    ActiveTime = 5;
                    break;
            }

            string pointToNext = AbilityTier < MaxAbilityTier ? 1.ToString() : "Maxed out!";
            string activeTime = ActiveTime > 0 ? ActiveTime.ToString() : "2";

            AbilityDescription = $"Points needed for next tier {pointToNext}\nEnemies and projectiles freeze in time for {activeTime}.";
        }

        /// <summary>
        /// Call to Reset the ability uses per level.
        /// <para>Called on PlayerHub return.</para>
        /// </summary>
        public override void ResetAbilityUses()
        {
            ManagerHub.S.HUDHandler.SetAbilityUses(ManagerHub.S.AbilityManager.TotalAbilitiesPerRoom);
        }
    }
}