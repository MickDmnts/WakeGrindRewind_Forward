using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WGRF.BattleSystem;

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

        ///<summary>Weapon rewards cache for reseting</summary>
        Weapon[] weaponRewardsCache;

        protected override void PreAwake()
        {
            weaponRewardsCache = new Weapon[weaponRewards.Count];
            weaponRewards.CopyTo(weaponRewardsCache);
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
        /// Resets the rewards handler
        /// </summary>
        public void ResetRewards()
        { weaponRewards = weaponRewardsCache.ToList<Weapon>(); }
    }
}