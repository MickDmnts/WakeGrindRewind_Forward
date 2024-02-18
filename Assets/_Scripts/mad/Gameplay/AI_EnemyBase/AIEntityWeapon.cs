using UnityEngine;
using WGRF.BattleSystem;

namespace WGRF.AI
{
    /// <summary>
    /// A base class for ai entity attacking mechanisms
    /// </summary>
    public abstract class AIEntityWeapon : Shooter
    {
        ///<summary>The projectiles this agent will shoot per shoot sequence</summary>
        [Header("Set in inspector\nEnemy Specific")]
        [SerializeField, Tooltip("The projectiles this agent will shoot per shoot sequence")] protected int projectilesPerShot;
        ///<summary>Shooting cooldown</summary>
        [SerializeField, Tooltip("Shooting cooldownk")] protected float shotCooldownInterval;

        ///<summary>The ai entity of this weapon handler</summary>
        protected AIEntity aIEntity;
        ///<summary>The sprite renderer for the weapon</summary>
        protected SpriteRenderer enemyWeaponRenderer;
        ///<summary>Is the shooting on cooldown</summary>
        protected bool onCooldown = false;
        ///<summary>Bursting default timer</summary>
        protected float shotBurstTimer;
        ///<summary>Bursting timer cache</summary>
        protected float burstTimerCache;
        ///<summary>Projectile per shot cache</summary>
        protected int projectilePerShotCache;
        ///<summary>The total bullet spread</summary>
        protected float totalBulletSpread;

        ///<summary>Returns the ai entity of this weapon handler</summary>
        public AIEntity AIEntity => aIEntity;

        /// <summary>
        /// Called in start to set weapon management defaults.
        /// </summary>
        protected abstract void SetStartDefaults();

        /// <summary>
        /// Call to decrease totalBulletSpread by accuracyIncreaseRate
        /// </summary>
        protected abstract void DecreaseSpread();

        /// <summary>
        /// Call to initiate an attack sequence based on the equiped weapon
        /// WeaponCategory
        /// </summary>
        protected abstract void TypeBasedAttack();

        /// <summary>
        /// Call to reset the shooting specific variables
        /// <para>(shootDoneTime + canShoot)</para>
        /// </summary>
        protected abstract void OnShootReset();

        protected abstract bool CanShoot();

        /// <summary>
        /// Call to get the firePoint transform of THIS enemy.
        /// </summary>
        public abstract Transform GetFirepointTransform();

        public abstract override void SetWeaponInfo(Weapon weapon);

        /// <summary>
        /// Call to get the equipedWeapon minimum shoot distance.
        /// </summary>
        public abstract float GetWeaponRange();

        /// <summary>
        /// Call to set isShooting to the passed value
        /// </summary>
        /// <param name="value">bool state</param>
        public abstract void SetIsShooting(bool value);

        public abstract void ShootSequence();
        public abstract override void Shoot();

        /// <summary>
        /// Call to set the weaponSprite to null
        /// </summary>
        public abstract void ClearWeaponSprite();
    }
}