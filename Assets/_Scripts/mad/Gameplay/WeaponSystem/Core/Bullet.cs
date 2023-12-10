using UnityEngine;
using WGRF.Core;
using WGRF.Core.Managers;
using WGRF.Gameplay.BattleSystem;
using WGRF.Interactions;

namespace WGRF.BattleSystem
{
    /// <summary>
    /// The layers every bullet gameObject can exist on.
    /// </summary>
    public enum ProjectileLayers
    {
        PlayerProjectile = 12,
        EnemyProjectile = 13,
    }

    public class Bullet : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] GameObject bloodImpactFX;

        #region PRIVATE_VARIABLES
        BulletType bulletType;
        WeaponType firedFrom;
        float bulletSpeed;

        bool isCached = true;
        #endregion

        #region STARTUP_BEHAVIOUR
        private void Awake()
        {
            if (ManagerHub.S != null)
            { ManagerHub.S.GameEventHandler.onBulletSpeedChange += ChangeSpeed; }
        }
        #endregion

        private void Update()
        {
            //Early exit if the bullet is inactive.
            if (isCached) return;

            //Move the bullet to its local forward. 
            transform.position += transform.forward * bulletSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            IInteractable interaction = other.GetComponent<IInteractable>();

            if (interaction != null)
            {
                interaction.AttackInteraction();

                //Spawn the blood impact FX when the bullet hits an entity
                GameObject spawnedFX = Instantiate(bloodImpactFX, other.transform.position, Quaternion.identity);
                spawnedFX.transform.rotation = transform.rotation * Quaternion.Euler(-90f, 0f, 0f);
            }

            //Cache the bullet after the impact
            CacheThisBullet();
        }

        #region UTILITIES
        /// <summary>
        /// Call to set THIS bullet instances' bullet type.
        /// <para>Bullet type of:</para>
        /// <para>Enemy: Collides with the player and not with AIEntities.</para>
        /// <para>Player: Collides with the AIEntities and not the player.</para>
        /// </summary>
        public void SetBulletType(BulletType type)
        {
            bulletType = type;

            if (bulletType == BulletType.Enemy)
            {
                gameObject.layer = (int)ProjectileLayers.EnemyProjectile;
            }
            else
            {
                gameObject.layer = (int)ProjectileLayers.PlayerProjectile;
            }
        }

        /// <summary>
        /// *Subsrcibed to the GameEventsHandler.onBulletSpeedChange event*
        /// <para>Call to set THIS bullets' current speed.</para>
        /// </summary>
        /// <param name="newSpeed"></param>
        public void ChangeSpeed(float newSpeed)
        {
            bulletSpeed = newSpeed;
        }

        /// <summary>
        /// Call to cache THIS bullet by passing it in the BulletPool.CacheBullet(...) method.
        /// </summary>
        void CacheThisBullet()
        {
            if (isCached) return;

            ManagerHub.S.BulletPool.CacheBullet(this, bulletType);
        }
        #endregion

        /// <summary>
        /// Resets bullet variables when the bullet gets enabled.
        /// </summary>
        private void OnEnable()
        {
            isCached = false;

            bulletSpeed = BulletStatics.CurrentSpeed;

            if (ManagerHub.S.PlayerController != null)
            {
                firedFrom = ManagerHub.S.PlayerController.Access<PlayerAttack>("pAttack").equipedWeapon.WeaponType;
            }

            //Auto-cache the bullet after a short time.
            Invoke("CacheThisBullet", 2f);
        }

        private void OnDisable()
        {
            isCached = true;
        }

        private void OnDestroy()
        {
            ManagerHub.S.GameEventHandler.onBulletSpeedChange -= ChangeSpeed;
        }
    }
}