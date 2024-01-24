using System;
using UnityEngine;
using WGRF.Core;

namespace WGRF.Abilities
{
    /// <summary>
    /// The slow down time ability
    /// </summary>
    public class SlowDownTime : Ability
    {
        ///<summary>Percentage of slow down based on ability tier.</summary>
        float slowDownPercent = 0f;
        ///<summary>Cached method called when the ability finishes execution</summary>
        Action onAbilityFinishCallback;

        /// <summary>
        /// Creates a SlowDownTime instance.
        /// </summary>
        /// <param name="name">The name of the ability</param>
        /// <param name="tier">The starting tier of the ability</param>
        /// <param name="abilitySprite">The ability sprite</param>
        /// <param name="isUnlocked">Is the ability unlocked initialy?</param>
        public SlowDownTime(string name, int tier, Sprite abilitySprite, bool isUnlocked)
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

        /// <summary>
        /// Fully enables the ability
        /// </summary>
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

            //Change the bullet current speed to simulate a slow down feeling.
            ManagerHub.S.InternalTime.ChangeTimeScale(0.5f, ActiveTime);

            //Decrease THIS rooms remaining ability uses.
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
            //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PressPlay);
        }

        /// <summary>
        /// Call to make changes and actions based on the ability behaviour.
        /// </summary>
        void AbilitySpecificActions()
        {
            //CHANGE AFTER TIME MANAGER CREATION
            /* foreach (EnemyEntity enemy in ManagerHub.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.GetAgent().angularSpeed -= (slowDownPercent / 100) * enemy.GetAgent().angularSpeed;
                enemy.GetAgent().acceleration -= (slowDownPercent / 100) * enemy.GetAgent().acceleration;
                enemy.GetAgent().speed -= (slowDownPercent / 100) * enemy.GetAgent().speed;

                enemy.OnPlayerAbilityStart(0.3f);
            } */

            //ManagerHub.S.GameSoundsHandler.ChangeSoundPitch(0.5f);
        }

        /// <summary>
        /// Call to update the tick of the ability script, just like running an Update() from MonoBehaviour.
        /// </summary>
        /// <param name="callback">The external method to call when the ability finishes</param>
        public override void UpdateAbilityTick()
        {
            //the timer will be used in the UI timer reference
            timer -= Time.deltaTime;

            //Notify any subscriber of onAbilityUse
            //ManagerHub.S.GameEventHandler.OnAbilityUse(ThrowableSpeeds.SlowDownSpeed, ThrowableSpeeds.SlowDownRotation, false);

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

            /* foreach (EnemyEntity enemy in ManagerHub.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.OnPlayerAbilityFinish();
            } */

            ManagerHub.S.GameEventHandler.OnAbilityEnd();

            //ManagerHub.S.GameSoundsHandler.ChangeSoundPitch(1f);
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
                    slowDownPercent = 30f;
                    ActiveTime = 2;
                    break;

                case 2:
                    slowDownPercent = 30f;
                    ActiveTime = 5;
                    break;

                case 3:
                    slowDownPercent = 60f;
                    ActiveTime = 5;
                    break;
            }

            string pointToNext = AbilityTier < MaxAbilityTier ? 1.ToString() : "Maxed out!";

            AbilityDescription = $"Points needed for next tier {pointToNext}\nReduce enemy and projectile movement speed by {slowDownPercent}% for {ActiveTime} seconds.";
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