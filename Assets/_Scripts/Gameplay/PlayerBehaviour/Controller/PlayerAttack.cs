using System.Collections;
using UnityEngine;

using WGR.BattleSystem;
using WGR.Core.Managers;
using WGR.Entities.BattleSystem;
using WGR.Interactions;

namespace WGR.Entities.Player
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variable : Must be set from the inspector
     * Private variables: These values change in runtime.
     * 
     * Implements: IShooter
     * Base types: Shooter, MonoBehaviour.
     * 
     * [Must know]
     * 0. This script is responsible for the whole player shooting mechanism. (Unarmed, Melee and Ranged)
     * 1.Bullet rotations are calculated internally when transfered from the bullet pool.
     * 2.Each bullet handles its own re-caching.
     * 3. The weapon values and sprites are reseted in the Player hub and  Abilities test room.
     * 4. Events called:
     *      A. GameEventsHandler.OnPlayerShootStart everytime a bullet gets shot, one time for a shotgun shot.
     *      B. GameEventsHandler.OnPlayerShootEnd when isShooting is set to false on a FixedUpdate tick rate.
     * 5. User HUD gets updated through data passed to the UserHUDHandler.
     */
    public class PlayerAttack : Shooter
    {
        [Header("Player specific")]
        [SerializeField] Weapon defaultWeapon;
        [HideInInspector] public bool IsAttackActive = false;

        //Private variables
        SpriteRenderer playerWeaponRenderer;
        float totalBulletSpread;

        private void Awake()
        {
            playerWeaponRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange += SetIsAttacking;

                GameManager.S.GameEventHandler.onPlayerRewind += SetIsAttacking;

                GameManager.S.GameEventHandler.onSceneChanged += ClearEquipedWeaponOnIdleScenes;

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
        /// *Subscribed to the GameEventHandler.onSceneChanged event.*
        /// <para>Call to clear the player weapon and reset weapon values on specific scenes.</para>
        /// <para>List of scenes: PlayerHub, AbilitiesTutorial</para>
        /// </summary>
        /// <param name="scene"></param>
        void ClearEquipedWeaponOnIdleScenes(GameScenes scene)
        {
            switch (scene)
            {
                case GameScenes.PlayerHub:
                    if (GameManager.S != null)
                    {
                        GameManager.S.HUDHandler.ChangeBulletsLeft(System.String.Empty);
                        GameManager.S.HUDHandler.ChangeWeaponInfo(null);
                        GameManager.S.PlayerEntity.PlayerAnimations.SetRangedWeaponAnimation(false);
                    }
                    SetWeaponInfo(defaultWeapon);
                    break;

                case GameScenes.AbilitiesTutorial:
                    if (GameManager.S != null)
                    {
                        GameManager.S.HUDHandler.ChangeBulletsLeft(System.String.Empty);
                        GameManager.S.HUDHandler.ChangeWeaponInfo(null);
                    }
                    SetWeaponInfo(defaultWeapon);
                    break;
            }
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
                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.PlayerAnimations.SetRangedWeaponAnimation(true);
                }
            }
            else
            {
                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.PlayerAnimations.SetRangedWeaponAnimation(false);
                }
            }

            //Show the equiped weapon on the UI.
            UpdateWeaponsUI(weapon, weapon.weaponAmmoSprite);

            //For managing the starting weapon pickup
            if (GameManager.S.LevelManager.FocusedScene.Equals(GameScenes.PlayerHub))
            {
                SetStartingBulletCount(weapon);
            }
        }

        /// <summary>
        /// Call to update the ammo icon and the bullets left UI elements based to the passed weapon values.
        /// <para>Displays only ranged weaponCategory weapons.</para>
        /// </summary>
        /// <param name="weapon">The currently equiped weapon.</param>
        /// <param name="weaponAmmoSprite">The ammo sprite of the weapon.</param>
        void UpdateWeaponsUI(Weapon weapon, Sprite weaponAmmoSprite)
        {
            GameManager.S.HUDHandler.ChangeWeaponInfo(weaponAmmoSprite);
            GameManager.S.HUDHandler.ChangeBulletsLeft((weapon.WeaponCategory != WeaponCategory.Ranged)
                ? System.String.Empty : bulletsLeft.ToString());
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
        /// Call to transfer a weapon gameObject instance from the WeaponManager.GetWeaponByType(...) 
        /// based on the equiped player weapon and place it in front of the player.
        /// <para>Early exits if the equiped weapon is null OR is of Unarmed weaponCategory.</para>
        /// <para>Calls ClearWeaponsUI()</para>
        /// </summary>
        public void DropEquipedWeapon()
        {
            if (equipedWeapon == null) return;

            if (equipedWeapon.WeaponCategory != WeaponCategory.Unarmed)
            {
                GameObject weaponOnHand = GameManager.S.WeaponManager.GetWeaponByType(equipedWeapon.WeaponType, bulletsLeft);
                weaponOnHand.transform.position = firePoint.position;
                weaponOnHand.transform.rotation = firePoint.rotation;

                weaponOnHand.SetActive(true);

                //Update UI elements
                ClearWeaponsUI();
            }
        }

        /// <summary>
        /// Call to set the bullet and bulletsLeft UI elements to null and String.Empty respectivaly.
        /// </summary>
        void ClearWeaponsUI()
        {
            if (GameManager.S != null)
            {
                GameManager.S.HUDHandler.ChangeWeaponInfo(null);
                GameManager.S.HUDHandler.ChangeBulletsLeft(System.String.Empty);
            }
        }

        private void FixedUpdate()
        {
            if (!isShooting)
            {
                DecreaseSpread();

                if (GameManager.S != null)
                {
                    GameManager.S.GameEventHandler.OnPlayerShootEnd();
                }
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
                || GameManager.S.PlayerEntity.PlayerKick.IsKicking
                || GameManager.S.LevelManager.FocusedScene == GameScenes.PlayerHub) return;

            //Check if the player can shoot when the user presse the Fire button.
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
                ThrowWeapon();
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
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.PlayMeleeWeaponAnimation(equipedWeapon.WeaponType);
            }

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

                                if (GameManager.S != null)
                                {
                                    GameManager.S.WeaponManager.WeaponKillCount.AddKillToWeapon(equipedWeapon.WeaponType);

                                    GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
                                }
                            }
                        }
                    }
                }
            }

            //Play the equiped weapon random SFX
            int rndSfx = Random.Range(0, equipedWeapon.gunShootSound.Length);
            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(equipedWeapon.gunShootSound[rndSfx]);
            }
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
            if (GameManager.S != null)
            {
                GameManager.S.HUDHandler.ChangeBulletsLeft(bulletsLeft.ToString());
            }
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
                if (GameManager.S != null)
                {
                    GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.EmptyGunSound);
                }
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
            GameObject tempBullet = GameManager.S.BulletPool.GetPooledBulletByType(BulletType.Player);

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

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.OnPlayerShootStart();
            }

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
                pellets[i] = GameManager.S.BulletPool.GetPooledBulletByType(BulletType.Player);
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

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.OnPlayerShootStart();
            }

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

        #region WEAPON_THROWING
        /// <summary>
        /// Call to get a weapon from the WeaponManager.GetWeaponByType(...) and pass it the current bullet count the player has left.
        /// <para>If the weapon is a IThrowable then call its InitiateThrow() method to simulate a weapon throw.</para>
        /// <para>Then play the player throw animation and a weapon throw SFX.</para>
        /// <para>Calls ClearWeaponsUI() at the end.</para>
        /// </summary>
        void ThrowWeapon()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.SetRangedWeaponAnimation(false);
            }

            GameObject toBeThrown = GameManager.S.WeaponManager.GetWeaponByType(equipedWeapon.WeaponType, bulletsLeft);

            if (toBeThrown != null)
            {
                toBeThrown.transform.position = firePoint.position;
                toBeThrown.transform.rotation = firePoint.rotation;

                toBeThrown.SetActive(true);

                IThrowable throwable = toBeThrown.GetComponent<IThrowable>();

                if (throwable != null)
                {
                    throwable.InitiateThrow();

                    if (GameManager.S != null)
                    {
                        GameManager.S.PlayerEntity.PlayerAnimations.PlayThrowAnimation();
                        GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.WeaponThrow);
                    }

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

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange -= SetIsAttacking;

                GameManager.S.GameEventHandler.onPlayerRewind -= SetIsAttacking;

                GameManager.S.GameEventHandler.onSceneChanged -= ClearEquipedWeaponOnIdleScenes;
            }
        }
    }
}