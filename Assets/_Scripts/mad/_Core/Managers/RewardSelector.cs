using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using WGRF.BattleSystem;
using WGRF.Player;

namespace WGRF.Core
{
    /// <summary>
    /// Th rewards handler responsible for throwing rewards.
    /// </summary>
    public class RewardSelector : CoreBehaviour
    {
        ///<summary>The available weapon rewards</summary>
        [SerializeField, Tooltip("The available weapon rewards")]
        List<Weapon> weaponRewards;
        [SerializeField, Tooltip("The min max range of health the player's health update")]
        [Range(10, 50)] int maxHealthUpdate;

        ///<summary>Weapon rewards cache for reseting</summary>
        List<Weapon> weaponRewardsCache;

        protected override void PreAwake()
        {
            weaponRewardsCache = new List<Weapon>();
            weaponRewardsCache = weaponRewards.ToList<Weapon>();
        }

        ///<summary>Call to increase the ability uses per room as a reward</summary>
        public int AbilityUsesReward()
        { return ManagerHub.S.AbilityManager.IncreaseTotalAbilityUsesPerRoom(); }

        ///<summary>Call to increase the player health by a random value set from the inspector</summary>
        public int PlayerHealthUpdateReward()
        {
            int healthIncrease = Random.Range(0, maxHealthUpdate);
            ManagerHub.S.PlayerController.Access<PlayerEntity>("pEntity").IncreaseHealthBy(healthIncrease);
            return healthIncrease;
        }

        /// <summary>
        /// Returns the next weapon reward from the weapon pool.
        /// Returns null if the pool is empty
        /// </summary>
        /// <returns>The next weapon, else null.</returns>
        public Weapon GetWeaponUpdate()
        {
            if (weaponRewards.Count <= 0)
            { return null; }

            Weapon temp = weaponRewards[0];
            weaponRewards.RemoveAt(0);
            return temp;
        }

        /// <summary>
        /// Returns the next weapon sprite
        /// </summary>
        public Sprite GetNextWeaponSprite()
        {
            if (weaponRewards.Count <= 0)
            { return null; }

            return weaponRewards[0].WeaponSprite;
        }

        /// <summary>
        /// Returns true if there are other weapon rewards left. false otherwise
        /// </summary>
        public bool HasWeaponReward()
        { return weaponRewards.Count > 0; }

        /// <summary>
        /// Resets the rewards handler
        /// </summary>
        public void ResetRewards()
        {
            weaponRewards = new List<Weapon>();
            weaponRewards = weaponRewardsCache;
        }
    }
}