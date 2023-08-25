using System.Collections.Generic;
using UnityEngine;
using WGR.BattleSystem;

namespace WGR.Core.Managers
{
    /* [CLASS DOCUMENTATION]
     * 
     * [Variable specific]
     * Inspector values: These values must be set from the inspector for the script to work correct.
     * Dynamically changed: These values change in runtime.
     * 
     * [Must Know]
     * 1. The weaponManager creates and stores X instances of every weapon prefab passed in the weaponPrefab list,
     *      when the game starts.
     * 2. WeaponType is used from methods to return a weaponPrefab.
     * 3. When a weapon is returned from a method, a reference of it is stored in the ThrownWeapons list through .Peek().
     */

    [DefaultExecutionOrder(75)]
    public class WeaponManager : MonoBehaviour
    {
        //Populate this list based on the indexing of WeaponType enum
        [Header("Set in inspector")]
        [SerializeField] List<GameObject> weaponPrefabs;
        [SerializeField] int prefabsPerObject;
        [SerializeField] List<Weapon> weaponsToBeCounted;

        #region DYNAMICALLY_CHANGED
        //A list that holds different queues based on the weaponPrefabs
        List<Queue<GameObject>> weaponQueues = new List<Queue<GameObject>>();

        //A list of the thrown weapons
        List<GameObject> thrownWeapons = new List<GameObject>();

        public WeaponKillCount WeaponKillCount { get; private set; }
        #endregion

        private void Awake()
        {
            WeaponKillCount = gameObject.AddComponent<WeaponKillCount>();
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged += RecacheWeapons;

                CreateWeaponQueues();
                PopulateQueues();
            }
            else { Utils.MissingComponent("GameManager", this); }
        }

        /// <summary>
        /// Called when a new scene loads to retract any thrown and dropped weapon from the previous loaded scenes.
        /// Sets thrownWeapon list to new List<GameObject>().
        /// </summary>
        void RecacheWeapons(GameScenes activeScene)
        {
            if (thrownWeapons.Count == 0) return;

            foreach (GameObject weapon in thrownWeapons)
            {
                if (weapon == null) continue;

                if (weapon.activeInHierarchy)
                {
                    weapon.transform.position = transform.position;
                    weapon.SetActive(false);

                    WeaponPickup toBeCached = weapon.GetComponent<WeaponPickup>();
                    toBeCached.SetWeaponBullets(toBeCached.weaponData.DefaultMagazine);

                    CacheWeaponByType(toBeCached.weaponData.WeaponType, weapon);
                }
            }

            thrownWeapons = new List<GameObject>();
        }

        /// <summary>
        /// Call to create and add Queue<GameObject> on weaponQueues based on weaponPrefabs.Count.
        /// </summary>
        void CreateWeaponQueues()
        {
            for (int i = 0; i < weaponPrefabs.Count; i++)
            {
                weaponQueues.Add(new Queue<GameObject>());
            }
        }

        /// <summary>
        /// Call to create X prefab instances of each weapon and store them in a the Queues stored in weaponQueues.
        /// Every weapon instance is deactivated upon creation and attached to a dynamically generated gameObject that
        /// acts as an anchor.
        /// </summary>
        void PopulateQueues()
        {
            int weaponIndex = 0;

            foreach (Queue<GameObject> queue in weaponQueues)
            {
                //Create the anchor gameObject
                GameObject anchor = new GameObject($"WeaponInIndex_{weaponIndex}");
                anchor.transform.parent = transform;

                //Create instances of the current weapon.
                for (int i = 0; i < prefabsPerObject; i++)
                {
                    GameObject tempWeapon = Instantiate(weaponPrefabs[weaponIndex], anchor.transform);
                    tempWeapon.SetActive(false);
                    queue.Enqueue(tempWeapon);
                }
                weaponIndex++;
            }
        }

        /// <summary>
        /// Call to get a weapon prefab based on its weapon type
        /// </summary>
        /// <param name="type">Which weapon prefab you want</param>
        /// <returns>A weapon prefab of the passed type</returns>
        public GameObject GetWeaponByType(WeaponType type, int playerBulletsLeft = -1)
        {
            GameObject tempWeapon;

            switch (type)
            {
                case WeaponType.Knife:
                    thrownWeapons.Add(weaponQueues[0].Peek());
                    tempWeapon = weaponQueues[0].Dequeue();
                    break;

                case WeaponType.BaseballBat:
                    thrownWeapons.Add(weaponQueues[1].Peek());
                    tempWeapon = weaponQueues[1].Dequeue();
                    break;

                case WeaponType.Pistol:
                    thrownWeapons.Add(weaponQueues[2].Peek());
                    tempWeapon = weaponQueues[2].Dequeue();
                    break;

                case WeaponType.SemiAutomatic:
                    thrownWeapons.Add(weaponQueues[3].Peek());
                    tempWeapon = weaponQueues[3].Dequeue();
                    break;

                case WeaponType.Shotgun:
                    thrownWeapons.Add(weaponQueues[4].Peek());
                    tempWeapon = weaponQueues[4].Dequeue();
                    break;

                default:
                    tempWeapon = null;
                    break;
            }

            if (tempWeapon != null)
            {
                WeaponPickup weaponPickup = tempWeapon.GetComponent<WeaponPickup>();

                if (weaponPickup != null && weaponPickup.weaponData.WeaponCategory == WeaponCategory.Ranged)
                {
                    weaponPickup.SetWeaponBullets((playerBulletsLeft == -1) ? weaponPickup.weaponData.DefaultMagazine : playerBulletsLeft);
                }
            }

            return tempWeapon;
        }

        /// <summary>
        /// Call to cache the used weapon back in the appropriate Queue based
        /// on it's weaponType
        /// </summary>
        public void CacheWeaponByType(WeaponType type, GameObject weapon)
        {
            switch (type)
            {
                case WeaponType.Knife:
                    weaponQueues[0].Enqueue(weapon);
                    break;
                case WeaponType.BaseballBat:
                    weaponQueues[1].Enqueue(weapon);
                    break;
                case WeaponType.Pistol:
                    weaponQueues[2].Enqueue(weapon);
                    break;
                case WeaponType.SemiAutomatic:
                    weaponQueues[3].Enqueue(weapon);
                    break;
                case WeaponType.Shotgun:
                    weaponQueues[4].Enqueue(weapon);
                    break;
            }

            WeaponPickup weaponPickup = weapon.GetComponent<WeaponPickup>();

            if (weaponPickup != null && weaponPickup.weaponData.WeaponCategory == WeaponCategory.Ranged)
            {
                weaponPickup.SetWeaponBullets(weaponPickup.GetCachedWeaponBullets());
            }

            weapon.SetActive(false);
            weapon.transform.position = transform.position;
        }

        /// <summary>
        /// Call to get the weaponsToBeCounted list.
        /// </summary>
        /// <returns></returns>
        public List<Weapon> GetWeaponSOsList()
        {
            return weaponsToBeCounted;
        }

        /// <summary>
        /// Call to find and invoke the PlayerEntity.PlayerShooting.SetWeaponInfo(...) method to set the
        /// equiped weapon to the passed weapon type.
        /// <para>Called from a WeaponRepresenter instance when the player presses a weapon select button.</para>
        /// </summary>
        public void SetStartingWeaponOnPlayer(WeaponType type)
        {
            Weapon tempWeapon;

            foreach (Weapon weapon in weaponsToBeCounted)
            {
                if (weapon.WeaponType.Equals(type))
                {
                    tempWeapon = weapon;
                    if (GameManager.S != null)
                    {
                        GameManager.S.PlayerEntity.PlayerShooting.SetWeaponInfo(tempWeapon);
                    }
                    else
                        Utils.MissingComponent("GameManager", this);
                }
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged -= RecacheWeapons;
            }
            else
                Utils.MissingComponent("GameManager", this);
        }
    }
}