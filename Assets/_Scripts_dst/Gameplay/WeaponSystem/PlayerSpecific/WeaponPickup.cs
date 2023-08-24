using UnityEngine;

namespace WGR.Gameplay.BattleSystem
{
    using WGR.Core;

    /* [CLASS DOCUMENTATION]
    * 
    * Inspector variable : Must be set from the inspector
    * Private variables: These values change in runtime.
    * 
    * [Must know]
    * 1. The weapon stats are set through the GameManager PlayerEntity field.
    * 2. The weapon gameObject gets cached to the weapon manager when picked up.
    * 
    */
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Set in inspector - SO Data")]
        public Weapon weaponData;

        #region PRIVATE_VARIABLES
        SpriteRenderer prefabRenderer;
        bool isActive = true;
        bool canInteract = false;
        int cachedBullets;
        #endregion

        private void Awake()
        {
            prefabRenderer = GetComponentInChildren<SpriteRenderer>();
            cachedBullets = weaponData.DefaultMagazine;
        }

        private void Start()
        {
            prefabRenderer.sprite = weaponData.WeaponSprite;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Early exit in case the weapon is inactive.
            if (!isActive) return;

            if (other.CompareTag("Player"))
            {
                canInteract = true;
            }
        }

        private void Update()
        {
            //Early exit in case the weapon is inactive.
            if (!canInteract || !isActive) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (GameManager.S != null)
                {
                    if (GameManager.S.LevelManager.FocusedScene == GameScenes.NewGameIntro)
                    {
                        GameManager.S.GameEventHandler.OnWeaponPickup();
                    }
                }

                SetWeaponStatsToPlayer();
            }
        }

        /// <summary>
        /// Call to set the weaponData stats to the player entity, call PlayerShooting.DropEquipedWeapon
        /// <para>and then cache the weapon to the weapon gameObject to the weapon manager.</para>
        /// </summary>
        void SetWeaponStatsToPlayer()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.WeaponPickUp);

                GameManager.S.PlayerEntity.PlayerShooting.DropEquipedWeapon();

                GameManager.S.PlayerEntity.PlayerShooting.SetWeaponInfo(weaponData, cachedBullets);

                GameManager.S.WeaponManager.CacheWeaponByType(weaponData.WeaponType, gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isActive) return;

            if (canInteract)
            {
                canInteract = false;
            }
        }

        /// <summary>
        /// Call to set the dropped weapon bullet count.
        /// </summary>
        public void SetWeaponBullets(int bullets)
        {
            cachedBullets = bullets;
        }

        /// <summary>
        /// Call to get the dropped weapon bullet count.
        /// </summary>
        /// <returns></returns>
        public int GetCachedWeaponBullets() => cachedBullets;
    }
}