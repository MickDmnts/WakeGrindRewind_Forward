using System.Collections;
using UnityEngine;
using WGR.Core;
using WGR.Gameplay.BattleSystem;

namespace WGR.Gameplay.AI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Private variables: These values change throughout the game.
     * 
     * Base Type:  AIEntityWeapon
     * 
     * This class is responsible for the attack mechanisms of the BossEntity.
     * 
     * [Must know]
     * 1. The ShootSequence method gets passed as an Action callback in the attacking node creation of the Boss BT.
     */
    public class BossWeapon : AIEntityWeapon
    {
        //Boss controller.
        BossEntity bossEntity;

        private void Awake()
        {
            CacheComponents();
        }

        /// <summary>
        /// Call cache the needed components.
        /// </summary>
        void CacheComponents()
        {
            //Component caching
            bossEntity = transform.root.GetComponent<BossEntity>();
            enemyWeaponRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        private void Start()
        {
            SetStartDefaults();
            SetWeaponInfo(equipedWeapon);
        }

        /// <summary>
        /// Call to set the default attacking values.
        /// </summary>
        protected override void SetStartDefaults()
        {
            //Attacking setup.
            canShoot = true;
            shootInterval = 0.4f;

            //Attacking cache.
            shotBurstTimer = shotCooldownInterval;
            burstTimerCache = shotBurstTimer;
            projectilePerShotCache = projectilesPerShot;
        }

        //Base type summary
        public override void SetWeaponInfo(Weapon weapon, int cachedBulletCount = -1)
        {
            if (weapon == null) return;

            enemyWeaponRenderer.sprite = weapon.WeaponSprite;
            shootInterval = weapon.IntervalBetweenShots;
            maxBulletSpread = weapon.MaxBulletSpread;
            canShoot = true;
        }

        /// <summary>
        /// Call to initiate the Boss Shoot sequence.
        /// Acts as an update too when called from the enemyBT .Run()
        /// </summary>
        public override void ShootSequence()
        {
            if (bossEntity.IsDead) return;

            if (CanShoot())
            {
                TypeBasedAttack();
            }
        }

        /// <summary>
        /// Call to check if the boss can shoot his next bullet again.
        /// </summary>
        protected override bool CanShoot()
        {
            if (Time.time >= shootDoneTime)
            {
                return true;
            }

            return false;
        }

        //Base type summary
        protected override void TypeBasedAttack()
        {
            Shoot();
            OnShootReset();
        }

        public override void Shoot()
        {
            //Shoot on cooldown update
            if (onCooldown)
            {
                DecreaseSpread();

                shotBurstTimer -= Time.deltaTime;
                if (shotBurstTimer <= 0f)
                {
                    onCooldown = false;
                    shotBurstTimer = burstTimerCache;
                    projectilesPerShot = projectilePerShotCache;
                }
                return;
            }

            //Shooting behaviour
            SetIsShooting(true);
            CalculateBulletSpread();

            StartCoroutine(ShotgunShot());

            //Weapon SFX
            PlayWeaponSFX();
        }

        //Base type summary
        protected override void DecreaseSpread()
        {
            if (totalBulletSpread > 0)
            {
                totalBulletSpread -= accuracyIncreaseRate;
            }
        }

        /// <summary>
        /// Call to move 6 projectiles in front of the fire point.
        /// </summary>
        IEnumerator ShotgunShot()
        {
            GameObject[] pellets = new GameObject[6];

            for (int i = 0; i < 6; i++)
            {
                pellets[i] = GameManager.S.BulletPool.GetPooledBulletByType(BulletType.Enemy);
            }

            foreach (GameObject pellet in pellets)
            {
                float randomFloat = Random.Range(-equipedWeapon.MaxBulletSpread, equipedWeapon.MaxBulletSpread);

                if (pellet != null)
                {
                    Quaternion bulletRotation = Quaternion.Euler(0f, randomFloat, 0);

                    pellet.transform.position = firePoint.transform.position;
                    pellet.transform.rotation = firePoint.rotation * bulletRotation;
                    pellet.SetActive(true);
                }
                else
                {
                    Debug.Log("BulletPool returned null");
                }
            }

            yield return null;
        }

        /// <summary>
        /// Call to increase bulletSpread based on bulletSpreadRate
        /// </summary>
        void CalculateBulletSpread()
        {
            totalBulletSpread += bulletSpreadRate;
            if (totalBulletSpread >= maxBulletSpread)
            {
                totalBulletSpread = maxBulletSpread;
            }
        }

        #region UTILITIES
        //Base type summary
        public override AIEntity GetAIEntity()
        {
            return bossEntity;
        }

        //Base type summary
        public override Transform GetFirepointTransform() => firePoint.transform;

        //Base type summary
        public override float GetWeaponRange()
        {
            return equipedWeapon.MinShootDistance;
        }

        //Base type summary
        public override void SetIsShooting(bool value)
        {
            isShooting = value;
        }

        //Base type summary
        public override void ClearWeaponSprite()
        {
            enemyWeaponRenderer.sprite = null;
        }

        //Base type summary
        protected override void OnShootReset()
        {
            shootDoneTime = shootInterval + Time.time;
            canShoot = false;
        }

        /// <summary>
        /// Call to play the equiped weapon SFX.
        /// <para>If the weapon has more than one SFX, plays a random one.</para>
        /// </summary>
        void PlayWeaponSFX()
        {
            if (equipedWeapon.gunShootSound.Length > 1)
            {
                int rndSfx = Random.Range(0, equipedWeapon.gunShootSound.Length);
                GameManager.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[rndSfx]);
            }
            else
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[0]);
            }
        }
        #endregion
    }
}