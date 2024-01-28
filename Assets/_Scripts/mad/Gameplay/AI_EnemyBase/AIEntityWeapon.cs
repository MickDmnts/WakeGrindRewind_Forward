using UnityEngine;
using WGRF.BattleSystem;
using WGRF.Entities.BattleSystem;

namespace WGRF.AI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These variables must be set from the inspector
     * Protected variables: Cached and changed throughout the game
     * 
     *  These class acts as a base attacking handler for each AI Entity in the game.
     */
    public abstract class AIEntityWeapon : Shooter
    {
        [Header("Set in inspector\nEnemy Specific")]
        [SerializeField] protected int projectilesPerShot;
        [SerializeField] protected float shotCooldownInterval;

        #region Private_Variables
        protected SpriteRenderer enemyWeaponRenderer;

        protected bool onCooldown = false;
        protected float shotBurstTimer;
        protected float burstTimerCache;

        protected int projectilePerShotCache;
        protected float totalBulletSpread;
        #endregion

        #region PROTECTED_METHODS
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
        #endregion

        #region PUBLIC_METHODS
        /// <summary>
        /// Call to get the AIEntity of THIS EnemyWeapon script.
        /// </summary>
        public abstract AIEntity GetAIEntity();

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
        #endregion
    }
}