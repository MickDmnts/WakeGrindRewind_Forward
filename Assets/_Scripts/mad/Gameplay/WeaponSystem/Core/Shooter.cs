using UnityEngine;

using WGRF.Core;

namespace WGRF.BattleSystem
{
    public abstract class Shooter : CoreBehaviour, IShooter
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
        protected int bulletsPerMag;
        protected float maxBulletSpread;
        protected bool canShoot;
        protected bool isShooting;
        protected float shootInterval;
        protected float shootDoneTime;
        #endregion

        public abstract void SetWeaponInfo(Weapon weapon);

        public abstract void Shoot();
    }
}