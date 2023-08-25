using System;
using UnityEngine;

using WGR.AI.Entities.Hostile.Generic;
using WGR.BattleSystem;
using WGR.Core.Managers;

namespace WGR.Abilities
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Dynamically used: Every class variable is changed throughout the game.
     * 
     * [Class Flow]
     * 1. They class entry point is the Start() method called from the Ability manager that sets any required script fields.
     * 2. The ability behavioural flow is enabled from the TryActivate() called from player input.
     * 3. The ability behavioural flow is disabled from the ability timer.
     * 
     * [Must Know]
     * 
     * X => An ability specific value.
     * 
     * 1. When activated the ability:
     *      1a. Changes the CurrentBulletSpeed to X values.
     *      1b. Changes the Enemy agent (angular, acceleration and speed) to X values.
     * 2. Both actions are set back to normal when the ability finishes.
     * 3. When the ability tier is updated it decreases the player skill points by X value.
     */

    public class SlowDownTime : Ability
    {
        //Dynamically changed
        float slowDownPercent = 0f;
        Action externalCallback;

        public SlowDownTime(string name, int tier, Sprite abilitySprite, bool isUnlocked)
        {
            this.AbilityName = name;
            this.AbilityDescription = "UPDATE TEXT PER TIER";

            //Set from json
            this.AbilityTier = tier;

            this.MaxAbilityTier = 3;
            this.ActiveTime = 0;
            this.UsesPerLevel = 0;

            this.AbilitySprite = abilitySprite;

            //Set from json
            this.IsUnlocked = isUnlocked;
        }

        /// <summary>
        /// Call to startup any basic ability behaviour
        /// <para>Sets the stats based on the abiltiy tier, caches the uses per level and sets canActivate to true.</para>
        /// </summary>
        public override void Start(Action callback)
        {
            GameManager.S.GameEventHandler.onSceneChanged += DisableBehaviourOnSceneChange;

            externalCallback = callback;

            UpdateStatsPerTier();

            this.CanActivate = true;
        }

        public override void DisableBehaviourOnSceneChange(GameScenes scene)
        {
            OnAbilityFinished();
        }

        /// <summary>
        /// Call to set IsUnlocked and CanActivate to true.
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
            if (UsesPerLevel <= 0 || !IsUnlocked)
                return false;

            timer = ActiveTime;


            //Change the bullet current speed to simulate a slow down feeling.
            BulletStatics.CurrentSpeed = BulletStatics.SlowDownSpeed;

            //Decrease THIS runs remaining ability uses.
            UsesPerLevel--;

            //Update remaining uses UI
            GameManager.S.HUDHandler.UpdateRemainingUsesIcon(UsesPerLevel, cachedUses);

            AbilitySpecificActions();

            PlayAbilitySound();

            return true;
        }

        protected override void PlayAbilitySound()
        {
            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PressPlay);
        }

        /// <summary>
        /// Call to make changes and actions based on the ability behaviour.
        /// </summary>
        void AbilitySpecificActions()
        {
            foreach (EnemyEntity enemy in GameManager.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.GetAgent().angularSpeed -= (slowDownPercent / 100) * enemy.GetAgent().angularSpeed;
                enemy.GetAgent().acceleration -= (slowDownPercent / 100) * enemy.GetAgent().acceleration;
                enemy.GetAgent().speed -= (slowDownPercent / 100) * enemy.GetAgent().speed;

                enemy.OnPlayerAbilityStart(0.3f);
            }

            GameManager.S.GameSoundsHandler.ChangeSoundPitch(0.5f);
        }

        /// <summary>
        /// Call to update the tick of the ability script, just like running an Update() from MonoBehaviour.
        /// </summary>
        /// <param name="callback">The external method to call when the ability finishes</param>
        public override void UpdateAbilityTick()
        {
            //the timer will be used in the UI timer reference
            timer -= Time.deltaTime;
            GameManager.S.HUDHandler.UpdateRemainingTimeIcon(timer, ActiveTime);

            //Notify any subscriber of onAbilityUse
            GameManager.S.GameEventHandler.OnAbilityUse(ThrowableSpeeds.SlowDownSpeed, ThrowableSpeeds.SlowDownRotation, false);

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
            externalCallback();

            BulletStatics.CurrentSpeed = BulletStatics.StartingSpeed;

            foreach (EnemyEntity enemy in GameManager.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (enemy == null) continue;

                enemy.OnPlayerAbilityFinish();
            }

            GameManager.S.GameEventHandler.OnAbilityEnd();

            GameManager.S.GameSoundsHandler.ChangeSoundPitch(1f);
        }

        /// <summary>
        /// Call to increase the AbilityTier by 1 level only if the current AbilityTier is smaller than MaxAbilityTier.
        /// </summary>
        public override void UpgradeAbility()
        {
            //Get the current player skill points.
            int playerPoints = GameManager.S.SkillPointHandle.RemainingSkillPoints();

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
                GameManager.S.SkillPointHandle.DecreaseSkillPoints(neededPoints);

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
                    slowDownPercent = 30f;
                    BulletStatics.SlowDownSpeed = 30f;
                    ActiveTime = 2;
                    UsesPerLevel = 1;
                    break;

                case 2:
                    slowDownPercent = 30f;
                    BulletStatics.SlowDownSpeed = 20f;
                    ActiveTime = 5;
                    UsesPerLevel = 2;
                    break;

                case 3:
                    slowDownPercent = 60f;
                    BulletStatics.SlowDownSpeed = 10f;
                    ActiveTime = 5;
                    UsesPerLevel = 3;
                    break;
            }

            string pointToNext = AbilityTier < MaxAbilityTier ? GetPointsForUpgrade().ToString() : "Maxed out!";
            string uses = UsesPerLevel > 0 ? UsesPerLevel.ToString() + " use(s) per floor." : "Not yet unlocked";

            AbilityDescription = $"Points needed for next tier {pointToNext}\nReduce enemy and projectile movement speed by {slowDownPercent}% for {ActiveTime} seconds.\n{uses}";

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
                //The slow down time ability is free so we need 0 points to update to tier 1.
                case 0:
                    points = 0;
                    break;
                case 1:
                    points = 4;
                    break;
                case 2:
                    points = 14;
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
            GameManager.S.HUDHandler.UpdateRemainingUsesIcon(0, cachedUses);
        }

        public override void AbilityInfoFullReset()
        {
            //Set from json
            this.AbilityTier = 0;

            this.MaxAbilityTier = 3;
            this.ActiveTime = 0;
            this.UsesPerLevel = 0;

            //Set from json
            this.IsUnlocked = false;
        }

        public override int GetCachedUses()
        {
            return cachedUses;
        }
    }
}