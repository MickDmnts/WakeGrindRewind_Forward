using System;
using UnityEngine;
using WGR.Core.Managers;

namespace WGR.Abilities
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * PUBLIC_VARIABLE values: These values must be set from the class constructor for the script to work correctly.
     * Internally changed: These variables are changed throughout the game.
     * 
     * [Must Know]
     * 1. This class is used for the ability construction, thus the abstraction.
     */
    [System.Serializable]
    public abstract class Ability
    {
        #region PUBLIC_VARIABLES
        public bool IsUnlocked { get; set; }
        public bool CanActivate { get; protected set; }

        public Sprite AbilitySprite;
        public string AbilityName;
        public string AbilityDescription;

        public int AbilityTier;
        public int MaxAbilityTier;
        public int ActiveTime;
        public int UsesPerLevel;
        #endregion

        //Internally used variables
        protected int cachedUses;
        protected float timer;

        #region STARTUP_BEHAVIOUR
        /// <summary>
        /// Call to fire off the ability initiation.
        /// </summary>
        public abstract void Start(Action callback);

        /// <summary>
        /// Call to set IsUnlocked and CanActivate to true.
        /// </summary>
        protected abstract void EnableAbility();
        #endregion

        #region ON_ABILITY_USAGE
        /// <summary>
        /// Call to Enable the ability behaviour.
        /// </summary>
        /// <returns></returns>
        public abstract bool TryActivate();

        protected abstract void PlayAbilitySound();

        /// <summary>
        /// Call to update the ability info per frame 
        /// (Used in MonoBehaviour Update()).
        /// <para>The passed method gets called when the ability pre-requisites are met.</para>
        /// </summary>
        public abstract void UpdateAbilityTick();

        /// <summary>
        /// Call when the ability behaviour is finished to revert back any changes.
        /// </summary>
        protected abstract void OnAbilityFinished();
        #endregion

        #region ABILITY_TIER_UPDATE
        /// <summary>
        /// Call to increase the ability tier by 1.
        /// </summary>
        public abstract void UpgradeAbility();

        /// <summary>
        /// Call to update the current ability stats based on its current tier.
        /// </summary>
        public abstract void UpdateStatsPerTier();
        #endregion

        #region UTILITIES
        public abstract int GetCachedUses();

        /// <summary>
        /// Returns the SkillPointsPerTier() value.
        /// </summary>
        public abstract int GetPointsForUpgrade();

        /// <summary>
        /// Returns the skill points needed for ability tier up.
        /// </summary>
        protected abstract int SkillPointsPerTier();

        /// <summary>
        /// Call to reset the ability uses back to maxAbilityUses.
        /// </summary>
        public abstract void ResetAbilityUses();

        public abstract void DisableBehaviourOnSceneChange(GameScenes scene);

        public abstract void AbilityInfoFullReset();

        public virtual bool Compare(Ability ability, Ability otherAbility)
        {
            if (ability.AbilityName == otherAbility.AbilityName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}