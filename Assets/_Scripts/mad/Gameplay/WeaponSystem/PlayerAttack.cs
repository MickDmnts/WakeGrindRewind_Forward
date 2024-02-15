using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using WGRF.BattleSystem;
using WGRF.Core;
using WGRF.Interactions;

namespace WGRF.Player
{
    public class PlayerAttack : Shooter
    {
        [Header("Player specific")]
        [SerializeField] Weapon defaultWeapon;
        [SerializeField] Transform objThrowPos;
        [SerializeField] int damage = 5;

        //Private variables
        Weapon currentRoomWeapon;
        bool isAttackActive = false;
        bool isReloading = false;
        SpriteRenderer playerWeaponRenderer;
        float totalBulletSpread;
        GameObject objOnHand;

        protected override void PreAwake()
        {
            SetController(transform.root.GetComponent<Controller>());

            playerWeaponRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }

        private void Start()
        {
            //ManagerHub.S.PlayerEntity.onPlayerStateChange += SetIsAttacking;

            ManagerHub.S.GameEventHandler.onPlayerRewind += SetIsAttacking;

            canShoot = true;
            SetIsAttacking(canShoot);

            SetWeaponInfo(defaultWeapon);
        }

        /// <summary>
        /// *Subscribed to the PlayerEntity.onPlayerStateChange and GameEventHandler.onPlayerRewind events.*
        /// <para>Call to set the state of the player shooting mechanics to the passed value.</para>
        /// </summary>
        void SetIsAttacking(bool isActive)
        {
            isAttackActive = isActive;
        }

        /// <summary>
        /// Sets the current room's weapon type to the passed weapon
        /// </summary>
        /// <param name="weapon">The current room weapon</param>
        public void SetCurrentRoomWeapon(Weapon weapon)
        { currentRoomWeapon = weapon; }

        /// <summary>
        /// Call to set the player weapon to the passed weapon type and values.
        /// <para>Calls UpdateWeaponsUI(...)</para>
        /// </summary>
        /// <param name="cachedBulletCount">Leave to default when passing a non ranged weapon so the bullet count does not get printed in the UI.
        /// <para>Used to transfer the bullets left between the equiped weapon and the picked up weapon.</para></param>
        public override void SetWeaponInfo(Weapon weapon)
        {
            //Update equiped weapon values
            equipedWeapon = weapon;
            playerWeaponRenderer.sprite = weapon.weaponEquipedSprite;
            shootInterval = weapon.IntervalBetweenShots;
            maxBulletSpread = weapon.MaxBulletSpread;
            canShoot = true;

            bulletsPerMag = weapon.DefaultMagazine;

            //Enables the weapon holding animation.
            if (weapon.WeaponCategory.Equals(WeaponCategory.Ranged))
            { ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").SetRangedWeaponAnimation(true); }
            else
            { ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").SetRangedWeaponAnimation(false); }

            //Show the equiped weapon on the UI.
            UpdateWeaponsUI(weapon);
        }

        /// <summary>
        /// Call to update the bullets left UI elements based to the passed weapon values.
        /// <para>Displays only ranged weaponCategory weapons.</para>
        /// </summary>
        /// <param name="weapon">The currently equiped weapon.</param>
        /// <param name="weaponAmmoSprite">The ammo sprite of the weapon.</param>
        void UpdateWeaponsUI(Weapon weapon)
        {
            ManagerHub.S.HUDHandler.SetWeaponSliderInfo(weapon);
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
            if (!isAttackActive
                || ManagerHub.S.PlayerController.Access<PlayerKick>("pKick").IsKicking) return;

            //Check if the player can shoot when the user presses the Fire button.
            if (Mouse.current.leftButton.isPressed && CanShoot())
            {
                TypeBasedAttack();
            }

            if (!Mouse.current.leftButton.isPressed)
            {
                SetIsShooting(false);
            }

            if (Mouse.current.rightButton.isPressed && equipedWeapon.WeaponType == WeaponType.Throwable)
            {
                ThrowObject();
                SetWeaponInfo(currentRoomWeapon);
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
            if (isReloading)
            { return; }

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
                                interaction.AttackInteraction(damage);

                                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.ForcePunch);
                            }
                            else
                            {ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Punch);}
                        }
                    }
                }
            }

            GameAudioClip sfx = (GameAudioClip)(-1);
            switch (equipedWeapon.WeaponType)
            {
                case WeaponType.Knife:
                    sfx = GameAudioClip.KnifeCut;
                    break;
                case WeaponType.BaseballBat:
                    sfx = GameAudioClip.Punch;
                    break;
                case WeaponType.Punch:
                    sfx = GameAudioClip.Punch;
                    break;
            }

            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(sfx);
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

            bulletsPerMag = bulletsPerMag > 0 ? --bulletsPerMag : 0;

            if (bulletsPerMag <= 0 && !isReloading)
            {
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.EmptyGun);
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Reloading);
                isReloading = true;
                StartCoroutine(ReloadAfter(1.5f));
                return;
            }

            //Update Bullets ui
            ManagerHub.S.HUDHandler.ChangeBulletsLeft(bulletsPerMag);
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
            if (bulletsPerMag <= 0)
            {
                ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.EmptyGun);
                return;
            }

            //Play the equiped weapon shoot sound.
            GameAudioClip sfx = (GameAudioClip)(-1);
            switch (equipedWeapon.WeaponType)
            {
                case WeaponType.Pistol:
                    sfx = GameAudioClip.Pistol;
                    break;
                case WeaponType.SemiAutomatic:
                    sfx = GameAudioClip.Uzi;
                    break;
                case WeaponType.Shotgun:
                    sfx = GameAudioClip.Shotgun;
                    break;
            }

            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(sfx);

            if (shotgunShot && bulletsPerMag != 0)
            {
                StartCoroutine(ShotgunShot());
                return;
            }

            //Transfer the bullet.
            GameObject tempBullet = Instantiate(ManagerHub.S.BulletPool.GetBullet());
            tempBullet.GetComponent<Bullet>().SetBulletType((BulletType)ProjectileLayers.PlayerProjectile);

            float randomFloat = Random.Range(-totalBulletSpread, totalBulletSpread);

            //Rotate the bullet based on the randomFloat value.
            Quaternion bulletRotation = Quaternion.Euler(0f, randomFloat, 0);

            tempBullet.transform.position = firePoint.transform.position;
            tempBullet.transform.rotation = firePoint.rotation * bulletRotation;
            tempBullet.SetActive(true);
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
                pellets[i] = Instantiate(ManagerHub.S.BulletPool.GetBullet());
                pellets[i].GetComponent<Bullet>().SetBulletType((BulletType)ProjectileLayers.PlayerProjectile); ;
            }

            foreach (GameObject pellet in pellets)
            {
                float randomFloat = Random.Range(-equipedWeapon.MaxBulletSpread, equipedWeapon.MaxBulletSpread);

                Quaternion bulletRotation = Quaternion.Euler(0f, randomFloat, 0);

                pellet.transform.position = firePoint.transform.position;
                pellet.transform.rotation = firePoint.rotation * bulletRotation;
                pellet.SetActive(true);
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

        IEnumerator ReloadAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds / 2);

            int halfMag = equipedWeapon.DefaultMagazine / 2;
            ManagerHub.S.HUDHandler.ChangeBulletsLeft(halfMag);

            yield return new WaitForSeconds(seconds / 2);

            bulletsPerMag = equipedWeapon.DefaultMagazine;
            ManagerHub.S.HUDHandler.ChangeBulletsLeft(bulletsPerMag);
            isReloading = false;
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

            objOnHand.transform.position = firePoint.position;
            objOnHand.transform.rotation = firePoint.rotation;

            objOnHand.SetActive(true);

            IThrowable throwable = objOnHand.GetComponent<IThrowable>();

            if (throwable != null)
            {
                throwable.InitiateThrow();

                ManagerHub.S.PlayerController.Access<PlayerAnimations>("pAnimations").PlayThrowAnimation();
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

        /// <summary>
        /// Attaches an object to the players holding position
        /// </summary>
        public void SetHeldObject(GameObject obj)
        {
            objOnHand = obj;
            objOnHand.transform.position = objThrowPos.position;
            objOnHand.transform.rotation = objThrowPos.rotation;
        }
        #endregion

        protected override void PreDestroy()
        {
            ManagerHub.S.GameEventHandler.onPlayerRewind -= SetIsAttacking;
        }
    }
}