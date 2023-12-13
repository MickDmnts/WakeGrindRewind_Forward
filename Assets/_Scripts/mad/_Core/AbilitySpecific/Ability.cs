using System;
using UnityEngine;
using WGRF.Core;

namespace WGRF.Abilities
{
    /// <summary>
    /// Base class of every game Ability.
    /// </summary>
    [System.Serializable]
    public abstract class Ability
    {
        #region PUBLIC_VARIABLES
        ///<summary>Is the ability unlocked</summary>
        public bool IsUnlocked { get; set; }
        ///<summary>Can the ability be activated</summary>
        public bool CanActivate { get; protected set; }

        ///<summary>The ability UI sprite</summary>
        public Sprite AbilitySprite;
        ///<summary>The ability name</summary>
        public string AbilityName;
        ///<summary>The ability description</summary>
        public string AbilityDescription;

        ///<summary>The current ability tier</summary>
        public int AbilityTier;
        ///<summary>The max ability tier</summary>
        public int MaxAbilityTier;
        ///<summary>The active time of the ability</summary>
        public int ActiveTime;
        ///<summary>The ability uses per level</summary>
        public int UsesPerLevel;
        #endregion

        ///<summary>Current uses of the ability in the current level</summary>
        protected int cachedUses;
        ///<summary>Cache timer for each ability sequence</summary>
        protected float timer;

        #region STARTUP_BEHAVIOUR
        /// <summary>
        /// Call to fire off the ability initialization.
        /// </summary>
        /// <param name="callback">Method to call at the end of the initialization</param>
        public abstract void Start(Action callback);

        /// <summary>
        /// Sets IsUnlocked and CanActivate to true.
        /// </summary>
        protected abstract void EnableAbility();
        #endregion

        #region ON_ABILITY_USAGE
        /// <summary>
        /// Call to Enable the ability behaviour.
        /// </summary>
        /// <returns></returns>
        public abstract bool TryActivate();

        ///<summary>Plays the abilities SFX</summary>
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
        ///<summary>Returns the current cached ability uses.</summary>
        public abstract int GetCachedUses();

        /// <summary>
        /// Call to reset the ability uses back to maxAbilityUses.
        /// </summary>
        public abstract void ResetAbilityUses();

        public virtual bool Compare(Ability ability, Ability otherAbility)
        {return ability.AbilityName == otherAbility.AbilityName; }
        #endregion
    }
}