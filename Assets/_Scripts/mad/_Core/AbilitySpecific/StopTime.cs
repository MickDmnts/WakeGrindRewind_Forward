using System;
using UnityEngine;

//using WGRF.AI.Entities.Hostile.Generic;
//using WGRF.BattleSystem;
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
            this.UsesPerLevel = 0;

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
            if (UsesPerLevel <= 0 || !IsUnlocked)
                return false;

            timer = ActiveTime;

            //REWORK INTO TIME MANAGER
            //Change the bullet current speed to simulate a stop time feeling.
            //BulletStatics.CurrentSpeed = BulletStatics.StopTimeSpeed;

            //Decrease THIS runs remaining ability uses.
            UsesPerLevel--;
            //Update remaining uses UI
            //ManagerHub.S.HUDHandler.UpdateRemainingUsesIcon(UsesPerLevel, cachedUses);

            AbilitySpecificActions();

            PlayAbilitySound();

            return true;
        }

        ///<summary>Play this ability's SFX</summary>
        protected override void PlayAbilitySound()
        {
            //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PressPlay);
            //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.StopActivate);
        }

        /// <summary>
        /// Call to make changes and actions based on the ability behaviour
        /// </summary>
        void AbilitySpecificActions()
        {   
            //CHANGE AFTER TIME MANAGER CREATION
            /* foreach (EnemyEntity enemy in GameManager.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.GetAgent().speed = 0;
                enemy.GetAgent().angularSpeed = 0;

                enemy.EnemyAnimation.SetAnimatorPlaybackSpeed(0f);

                enemy.DisableShootingBehaviour();
            } */

            //ManagerHub.S.GameSoundsHandler.SetGameWideSoundtrackState(true);
        }

        /// <summary>
        /// Call to update the tick of the ability script, just like running an Update() from MonoBehaviour.
        /// </summary>
        public override void UpdateAbilityTick()
        {
            //the timer will be used in the UI timer reference
            timer -= Time.deltaTime;
            //ManagerHub.S.HUDHandler.UpdateRemainingTimeIcon(timer, ActiveTime);

            //Notify any subscriber of onAbilityUse
            //ManagerHub.S.GameEventHandler.OnAbilityUse(ThrowableSpeeds.StopTimeSpeed, ThrowableSpeeds.StopTimeRotation, true);

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

            //CHANGE AFTER TIME MANAGER CREATION
            //BulletStatics.CurrentSpeed = BulletStatics.StartingSpeed;

            /* foreach (EnemyEntity enemy in GameManager.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.OnPlayerAbilityFinish();
            } */

            ManagerHub.S.GameEventHandler.OnAbilityEnd();

            //ManagerHub.S.GameSoundsHandler.SetGameWideSoundtrackState(false);
        }

        /// <summary>
        /// Call to increase the AbilityTier by 1 level only if the current AbilityTier is smaller than MaxAbilityTier.
        /// </summary>
        public override void UpgradeAbility()
        {
            //Get the current player skill points.
            int playerPoints = 0;//ManagerHub.S.SkillPointHandle.RemainingSkillPoints();

            //Get the next tier update points needed.
            int neededPoints = SkillPointsPerTier();

            if (playerPoints >= neededPoints && AbilityTier < MaxAbilityTier)
            {
                //Enable the ability if it is still locked.
                if (!IsUnlocked)
                {
                    EnableAbility();
                }

                AbilityTier++;

                //Decrease player current skill points by the ability needed points.
                //ManagerHub.S.SkillPointHandle.DecreaseSkillPoints(neededPoints);

                //Refresh the ability stats to the new tier stats.
                UpdateStatsPerTier();
            }
            else
            {
                //We can fire off an event to notify the UI to throw an error
                //?Maybe build an event that passes strings to a UI message system?
            }
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
                    UsesPerLevel = 1;
                    break;

                case 2:
                    ActiveTime = 3;
                    UsesPerLevel = 2;
                    break;

                case 3:
                    ActiveTime = 5;
                    UsesPerLevel = 3;
                    break;
            }

            string pointToNext = AbilityTier < MaxAbilityTier ? GetPointsForUpgrade().ToString() : "Maxed out!";
            string uses = UsesPerLevel > 0 ? UsesPerLevel.ToString() + " use(s) per floor." : "Not yet unlocked";
            string activeTime = ActiveTime > 0 ? ActiveTime.ToString() : "2";

            AbilityDescription = $"Points needed for next tier {pointToNext}\nEnemies and projectiles freeze in time for {activeTime}.\n{uses}";
            cachedUses = UsesPerLevel;
        }

        //This is needed for UI display purposes!
        public override int GetPointsForUpgrade()
        {
            return SkillPointsPerTier();
        }

        /// <summary>
        /// Call to get the needed points to update the skill to the next tier.
        /// </summary>
        /// <returns>An int representing the skill points needed to upgrade.</returns>
        protected override int SkillPointsPerTier()
        {
            int points = 0;

            switch (AbilityTier)
            {
                case 0:
                    points = 6;
                    break;
                case 1:
                    points = 12;
                    break;
                case 2:
                    points = 22;
                    break;
            }

            return points;
        }

        /// <summary>
        /// Call to Reset the ability uses per level.
        /// <para>Called on PlayerHub return.</para>
        /// </summary>
        public override void ResetAbilityUses()
        {
            UsesPerLevel = cachedUses;
            //ManagerHub.S.HUDHandler.UpdateRemainingUsesIcon(0, cachedUses);
        }

        public override int GetCachedUses()
        {return cachedUses;}
    }
}