using UnityEngine;

using WGRF.Core;

namespace WGRF.BattleSystem
{
    /// <summary>
    /// A base class for an entity that is capable of shooting
    /// </summary>
    public abstract class Shooter : CoreBehaviour, IShooter
    {
        ///<summary>The melee detection layers for attacking</summary>
        [Header("\tSet in inspector\n" +
            "Melee ray detection settings")]
        [SerializeField, Tooltip("The melee detection layers for attacking")]
        protected LayerMask meleeDetectionLayers;
        ///<summary>The melee detection layers for checking if the attack casts</summary>
        [SerializeField, Tooltip("The melee detection layers for checking if the attack casts")]
        protected LayerMask meleeLinecastLayers;

        ///<summary>The fire point transform of the shooter</summary>
        [Header("Shooting specifics")]
        [SerializeField, Tooltip("The fire point transform of the shooter")] protected Transform firePoint;
        ///<summary>At which rate the accuracy increases when attacking</summary>
        [SerializeField, Range(0f, 1f), Tooltip("At which rate the accuracy decreases when attacking")]
        protected float bulletSpreadRate;
        ///<summary>At which rate the accuracy increases when attacking</summary>
        [SerializeField, Range(0f, 1f), Tooltip("At which rate the accuracy increases when attacking")]
        protected float accuracyIncreaseRate;

        ///<summary>The AI entity equiped weapon</summary>
        [Header("Set ONLY if in AI entity")]
        [SerializeField, Tooltip("The AI entity equiped weapon")] protected Weapon equipedWeapon;

        ///<summary>The cached bullets per magazine</summary>
        protected int bulletsPerMag;
        ///<summary>The max bullet spread</summary>
        protected float maxBulletSpread;
        ///<summary>Can the entity shoot?</summary>
        protected bool canShoot;
        ///<summary>Is the entity shooting right now?</summary>
        protected bool isShooting;
        ///<summary>The cooldown between shooting bursts</summary>
        protected float shootInterval;
        ///<summary>The shoot timer</summary>
        protected float shootDoneTime;

        ///<summary>The equiped weapon of the entity</summary>
        public Weapon EquipedWeapon => equipedWeapon;

        public abstract void SetWeaponInfo(Weapon weapon);

        public abstract void Shoot();
    }
}