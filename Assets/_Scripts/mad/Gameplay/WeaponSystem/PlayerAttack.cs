using System.Collections;
using UnityEngine;
using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Core.Managers;
using WGRF.Entities.BattleSystem;
using WGRF.Entities.Player;
using WGRF.Interactions;

namespace WGRF.Gameplay.BattleSystem
{
    public class PlayerAttack : Shooter
    {
        [Header("Player specific")]
        [SerializeField] Weapon defaultWeapon;
        [HideInInspector] public bool IsAttackActive = false;

        //Private variables
        SpriteRenderer playerWeaponRenderer;
        float totalBulletSpread;
        GameObject objOnHand;

        protected override void PreAwake()
        {
            playerWeaponRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        private void Start()
        {
            if (ManagerHub.S != null)
            {
                //ManagerHub.S.PlayerEntity.onPlayerStateChange += SetIsAttacking;

                ManagerHub.S.GameEventHandler.onPlayerRewind += SetIsAttacking;

                canShoot = true;

                SetWeaponInfo(defaultWeapon);
            }
        }

        /// <summary>
        /// *Subscribed to the PlayerEntity.onPlayerStateChange and GameEventHandler.onPlayerRewind events.*
        /// <para>Call to set the state of the player shooting mechanics to the passed value.</para>
        /// </summary>
        void SetIsAttacking(bool isActive)
        {
            IsAttackActive = isActive;
        }

        /// <summary>
        /// Call to set the player weapon to the passed weapon type and values.
        /// <para>Calls UpdateWeaponsUI(...)</para>
        /// </summary>
        /// <param name="cachedBulletCount">Leave to default when passing a non ranged weapon so the bullet count does not get printed in the UI.
        /// <para>Used to transfer the bullets left between the equiped weapon and the picked up weapon.</para></param>
        public override void SetWeaponInfo(Weapon weapon, int cachedBulletCount = -1)
        {
            //Update equiped weapon values
            equipedWeapon = weapon;
            playerWeaponRenderer.sprite = weapon.weaponEquipedSprite;
            shootInterval = weapon.IntervalBetweenShots;
            maxBulletSpread = weapon.MaxBulletSpread;
            canShoot = true;

            bulletsLeft = cachedBulletCount;

            //Enable the weapon holding animation.
            if (weapon.WeaponCategory.Equals(WeaponCategory.Ranged))
            {
                ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").SetRangedWeaponAnimation(true);
            }
            else
            {
                ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").SetRangedWeaponAnimation(false);
            }

            //Show the equiped weapon on the UI.
            UpdateWeaponsUI(weapon, weapon.weaponAmmoSprite);
        }

        /// <summary>
        /// Call to update the ammo icon and the bullets left UI elements based to the passed weapon values.
        /// <para>Displays only ranged weaponCategory weapons.</para>
        /// </summary>
        /// <param name="weapon">The currently equiped weapon.</param>
        /// <param name="weaponAmmoSprite">The ammo sprite of the weapon.</param>
        void UpdateWeaponsUI(Weapon weapon, Sprite weaponAmmoSprite)
        {
            /* GameManager.S.HUDHandler.ChangeWeaponInfo(weaponAmmoSprite);
            GameManager.S.HUDHandler.ChangeBulletsLeft((weapon.WeaponCategory != WeaponCategory.Ranged)
                ? System.String.Empty : bulletsLeft.ToString()); */
        }

        /// <summary>
        /// Call to set the player starting weapon bullet count when in the player hub to the passed weapon value.
        /// <para>Calls UpdateWeaponsUI(...)</para>
        /// </summary>
        /// <param name="weapon"></param>
        void SetStartingBulletCount(Weapon weapon)
        {
            bulletsLeft = weapon.DefaultMagazine;

            UpdateWeaponsUI(weapon, weapon.weaponAmmoSprite);
        }

        /// <summary>
        /// Call to set the bullet and bulletsLeft UI elements to null and String.Empty respectivaly.
        /// </summary>
        void ClearWeaponsUI()
        {
            /* ManagerHub.S.HUDHandler.ChangeWeaponInfo(null);
            ManagerHub.S.HUDHandler.ChangeBulletsLeft(System.String.Empty); */
        }

        private void FixedUpdate()
        {
            if (!isShooting)
            {
                DecreaseSpread();

                //ManagerHub.S.GameEventHandler.OnPlayerShootEnd();
            }
        }

        /// <summary>
        /// Call to decrease totalBulletSpread by accuracyIncreaseRate if the totalBulletSpread is greater than 0.
        /// </summary>
        void DecreaseSpread()
        {
            if (totalBulletSpread > 0)
            {
                totalBulletSpread -= accuracyIncreaseRate;
            }
        }

        void Update()
        {
            //Prevents shooting when the mechanic is deactivate, the player is Kicking or when he's in the PlayerHub.
            if (!IsAttackActive
                || ManagerHub.S.PlayerController.Access<PlayerKick>("pKick").IsKicking) return;

            //Check if the player can shoot when the user presses the Fire button.
            if (Input.GetButton("Fire1") && CanShoot())
            {
                TypeBasedAttack();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                SetIsShooting(false);
            }

            if (Input.GetButtonDown("Fire2") && equipedWeapon.WeaponType != WeaponType.Punch)
            {
                ThrowObject();
                SetWeaponInfo(defaultWeapon);
            }
        }

        /// <summary>
        /// Call to check if the player can shoot again based on time passed since the last shot.
        /// <para>Sets canShoot to true if the player can shoot.</para>
        /// </summary>
        bool CanShoot()
        {
            if (Time.time >= shootDoneTime)
            {
                canShoot = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call to initiate an attacking sequence based on the equiped weapon category.
        /// <para>Calls OnAttackReset() after each attack end.</para>
        /// </summary>
        void TypeBasedAttack()
        {
            switch (equipedWeapon.WeaponCategory)
            {
                case WeaponCategory.Unarmed:
                    MeleeAttack();

                    OnAttackReset();
                    break;

                case WeaponCategory.Melee:
                    MeleeAttack();

                    OnAttackReset();
                    break;

                case WeaponCategory.Ranged:
                    Shoot();

                    OnAttackReset();
                    break;
            }
        }

        #region UNARMED_MELEE_ATTACK
        /// <summary>
        /// Call to play the melee animation bassed on the equipedWeapon.WeaponType.
        /// <para>If the hit was a kill then call WeaponKillCount.AddKillToWeapon(passing the equiped weapon type)</para>
        /// <para>Plays a punch sound to notify the user of the hit.</para>
        /// <para>Plays an SFX based on the equiped weapon.</para>
        /// </summary>
        void MeleeAttack()
        {
            ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").PlayMeleeWeaponAnimation(equipedWeapon.WeaponType);

            //Shoot a ray forward
            Ray meleeRay = new Ray(transform.position, transform.forward);
            RaycastHit[] hits = new RaycastHit[10];
            int numOfHits = Physics.RaycastNonAlloc(meleeRay, hits, equipedWeapon.MinShootDistance, meleeDetectionLayers);

            //Check for AI hits
            if (numOfHits > 0)
            {
                for (int i = 0; i < numOfHits; i++)
                {
                    if (hits[i].transform.CompareTag("Enemy"))
                    {
                        //If we hit an AI directly, not through meleeLinecastLayers.
                        if (!Physics.Linecast(transform.position, hits[i].transform.position, meleeLinecastLayers))
                        {
                            IInteractable interaction = hits[i].transform.GetComponent<IInteractable>();

                            if (interaction != null)
                            {
                                //Call the Entity AttackInteraction().
                                interaction.AttackInteraction();

                                //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
                            }
                        }
                    }
                }
            }

            //Play the equiped weapon random SFX
            /* int rndSfx = Random.Range(0, equipedWeapon.gunShootSound.Length);
            ManagerHub.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[rndSfx]); */
        }
        #endregion

        #region SHOOTING_SPECIFIC
        /// <summary>
        /// Call to initiate the player specific attacking sequence.
        /// <para>Sets isShooting to true.</para>
        /// <para>Changes the bullets left UI element.</para>
        /// </summary>
        public override void Shoot()
        {
            SetIsShooting(true);

            CalculateBulletSpread();

            if (!equipedWeapon.WeaponType.Equals(WeaponType.Shotgun))
            {
                EnableBullet(false);
            }
            else
            {
                EnableBullet(true);
            }

            bulletsLeft = bulletsLeft > 0 ? --bulletsLeft : 0;

            //Update Bullets ui
            //ManagerHub.S.HUDHandler.ChangeBulletsLeft(bulletsLeft.ToString());
        }

        /// <summary>
        /// Call to transfer a bullet gameObject from the bullet pool in front of the firePoint and activate it.
        /// <para>If the bulletsLeft count is smaller or equal to 0 then play an empty gun SFX and early return.</para>
        /// <para>If the shot is a shotgun shot then initiates the shotgun shoot coroutine and returns.</para>
        /// <para>Calls the GameEventHandler.OnPlayerShootStart() event 
        /// and sets isShooting to false at the end</para>
        /// </summary>
        /// <param name="shotgunShot">Is the user shooting a shotgun?</param>
        void EnableBullet(bool shotgunShot)
        {
            //Check if the gun is empty.
            if (bulletsLeft <= 0)
            {
                //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.EmptyGunSound);
                return;
            }

            //Play the equiped weapon shoot sound.
            PlayWeaponShootSound();

            if (shotgunShot && bulletsLeft != 0)
            {
                StartCoroutine(ShotgunShot());
                return;
            }

            //Transfer the bullet.
            GameObject tempBullet = ManagerHub.S.BulletPool.GetPooledBulletByType(BulletType.Player);

            float randomFloat = Random.Range(-totalBulletSpread, totalBulletSpread);

            if (tempBullet != null)
            {
                //Rotate the bullet based on the randomFloat value.
                Quaternion bulletRotation = Quaternion.Euler(0f, randomFloat, 0);

                tempBullet.transform.position = firePoint.transform.position;
                tempBullet.transform.rotation = firePoint.rotation * bulletRotation;
                tempBullet.SetActive(true);
            }
            else
            {
                Debug.Log("BulletPool returned null");
            }

            ManagerHub.S.GameEventHandler.OnPlayerShootStart();

            SetIsShooting(false);
        }

        /// <summary>
        /// Call to transfer 6 bullets in front of the player.
        /// <para>Calls the GameEventHandler.OnPlayerShootStart() event 
        /// and sets isShooting to false at the end</para>
        /// </summary>
        IEnumerator ShotgunShot()
        {
            GameObject[] pellets = new GameObject[6];

            for (int i = 0; i < 6; i++)
            {
                pellets[i] = ManagerHub.S.BulletPool.GetPooledBulletByType(BulletType.Player);
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

            ManagerHub.S.GameEventHandler.OnPlayerShootStart();

            SetIsShooting(false);

            yield return null;
        }

        /// <summary>
        /// Call to increase the bullet spread variable by bulletSpreadRate.
        /// </summary>
        void CalculateBulletSpread()
        {
            totalBulletSpread += bulletSpreadRate;
            if (totalBulletSpread >= maxBulletSpread)
            {
                totalBulletSpread = maxBulletSpread;
            }
        }

        /// <summary>
        /// Call to play a random weapon shoot sound from the equiped weapon scriptable object.
        /// </summary>
        void PlayWeaponShootSound()
        {
            /* if (equipedWeapon.gunShootSound.Length > 1)
            {
                int rndSfx = Random.Range(0, equipedWeapon.gunShootSound.Length);
                GameManager.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[rndSfx]);
            }
            else
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[0]);
            } */
        }
        #endregion

        #region WEAPON_THROWING
        /// <summary>
        /// Call to get a weapon from the WeaponManager.GetWeaponByType(...) and pass it the current bullet count the player has left.
        /// <para>If the weapon is a IThrowable then call its InitiateThrow() method to simulate a weapon throw.</para>
        /// <para>Then play the player throw animation and a weapon throw SFX.</para>
        /// <para>Calls ClearWeaponsUI() at the end.</para>
        /// </summary>
        void ThrowObject()
        {
            ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").SetRangedWeaponAnimation(false);

            if (objOnHand != null)
            {
                objOnHand.transform.position = firePoint.position;
                objOnHand.transform.rotation = firePoint.rotation;

                objOnHand.SetActive(true);

                IThrowable throwable = objOnHand.GetComponent<IThrowable>();

                if (throwable != null)
                {
                    throwable.InitiateThrow();

                    ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").PlayThrowAnimation();
                    //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.WeaponThrow);

                    ClearWeaponsUI();
                }
            }
        }
        #endregion

        #region UTILITIES
        /// <summary>
        /// Call to set isShooting to the passed value
        /// </summary>
        void SetIsShooting(bool value)
        {
            isShooting = value;
        }

        /// <summary>
        /// Call to reset the shooting specific variables.
        /// <para>shootDoneTime is equal to shootInterval + Time.time</para>
        /// <para>casShoot is set to false.</para>
        /// </summary>
        void OnAttackReset()
        {
            shootDoneTime = shootInterval + Time.time;
            canShoot = false;
        }
        #endregion

        protected override void PreDestroy()
        {
            ManagerHub.S.GameEventHandler.onPlayerRewind -= SetIsAttacking;
        }
    }
}