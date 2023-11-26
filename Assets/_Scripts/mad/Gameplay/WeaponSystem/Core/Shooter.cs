using UnityEngine;

using WGRF.BattleSystem;

namespace WGRF.Entities.BattleSystem
{

    /* [CLASS DOCUMENTATIONS]
     * 
     * Inspector variable : Must be set from the inspector
     * Protected variables: These values change in runtime.
     * 
     *  Impelements: IShooter interface.
     * 
     * [Must know]
     * 1. The below class is the default template for every entity that can shoot ie. the player and the enemy types.
     *      An abstract class was used so each type can use its own attacking style.
     * 2. The equipedWeapon field must be set from the inspector ONLY in the enemies if we want them to hold a weapon before-hand.
     */
    public abstract class Shooter : MonoBehaviour, IShooter
    {
        #region INSPECTOR_VALUES
        [Header("\tSet in inspector\n" +
            "Melee ray detection settings")]
        [SerializeField] protected LayerMask meleeDetectionLayers;
        [SerializeField] protected LayerMask meleeLinecastLayers;

        [Header("Shooting specifics")]
        [SerializeField] protected Transform firePoint;
        [SerializeField, Range(0f, 1f)] protected float bulletSpreadRate;
        [SerializeField, Range(0f, 1f)] protected float accuracyIncreaseRate;

        [Header("Set ONLY if in AI entity")]
        public Weapon equipedWeapon;
        #endregion

        #region PROTECTED_VARIABLES
        protected int bulletsLeft;
        protected float maxBulletSpread;
        protected bool canShoot;
        protected bool isShooting;
        protected float shootInterval;
        protected float shootDoneTime;
        #endregion

        public abstract void SetWeaponInfo(Weapon weapon, int cachedBulletCount = -1);

        public abstract void Shoot();
    }
}