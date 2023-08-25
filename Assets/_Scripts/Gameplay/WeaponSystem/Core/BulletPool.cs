using System.Collections.Generic;
using UnityEngine;
using WGR.BattleSystem;

namespace WGR.Core.Managers
{
    /// <summary>
    /// Every available bullet type in the game.
    /// </summary>
    public enum BulletType
    {
        Enemy,
        Player,
    }

    /* [CLASS DOCUMENTATION]
    * 
    * Inspector variable : Must be set from the inspector
    * Private variables: These values change in runtime.
    * 
    * [Class flow]
    * 1. When the game loads the manager divides the amountToPool value by 2 and creates 2 Queues, one for the player bullets
    *   and one for the enemy bullets.
    * 2. Each bullet instance is assigned a BulletType so we don't have friendly fire between entities.
    * 
    * [Must know]
    * 1. The GetPooledBulletByType(...) returns null in case the pool is empty.
    * 
    */
    [DefaultExecutionOrder(50)]
    public class BulletPool : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("Set in inspector - Bullet prefab")]
        [SerializeField] GameObject bulletPrefab;

        [Header("Set in inspector - Projectile layers")]
        [SerializeField] LayerMask playerBulletLayer;
        [SerializeField] LayerMask enemyBulletLayer;

        [Header("Set in inspector - Bullet default speed")]
        [SerializeField] float startingSpeed;

        [Header("\tSet in inspector - Total bullet amount")]
        [SerializeField, Tooltip("The amount will be separated in two pools.\n" +
            "50% in player bullets - 50% in enemy bullets.")]
        int amountToPool;
        #endregion

        #region PRIVATE_VARIABLES
        Transform enemyBulletAnchor;
        Transform playerBulletAnchor;

        Queue<GameObject> enemyProjectiles = new Queue<GameObject>();
        Queue<GameObject> playerProjectiles = new Queue<GameObject>();

        int enemyProjectileNum;
        int playerProjectileNum;
        #endregion

        #region STARTUP_BEHAVIOUR
        private void Awake()
        {
            CreateAnchors();
        }

        /// <summary>
        /// Call to create 2 anchor gameObjects that will have the bullets as children so the hierarchy is kept clean.
        /// </summary>
        void CreateAnchors()
        {
            GameObject enemyAnchor = new GameObject("EnemyProjectiles");
            enemyAnchor.transform.SetParent(transform);
            enemyBulletAnchor = enemyAnchor.transform;

            GameObject playerAnchor = new GameObject("PlayerProjectiles");
            playerAnchor.transform.SetParent(transform);
            playerBulletAnchor = playerAnchor.transform;
        }

        private void Start()
        {
            SetBulletStaticSpeeds();

            CreateObjPools();
        }

        /// <summary>
        /// Call to set the default BulletStatic speeds based on the values set from the inspector.
        /// </summary>
        void SetBulletStaticSpeeds()
        {
            BulletStatics.StartingSpeed = startingSpeed;

            BulletStatics.CurrentSpeed = BulletStatics.StartingSpeed;
        }

        /// <summary>
        /// Call to initiate the bullet pool creation sequence.
        /// <para>1. Call SetPoolBounds() to set the queue bounds.</para>
        /// <para>2. Call PopulatePool(with BulletType.Player)</para>
        /// <para>3. Call PopulatePool(with BulletType.Enemy)</para>
        /// </summary>
        void CreateObjPools()
        {
            SetPoolBounds();

            PopulatePool(ref playerProjectiles, playerProjectileNum, playerBulletAnchor, BulletType.Player);
            PopulatePool(ref enemyProjectiles, enemyProjectileNum, enemyBulletAnchor, BulletType.Enemy);
        }

        /// <summary>
        /// Call to divide the inspector amountToPool by 2 and set the enemyProjectileNum 
        /// and playerProjectileNum to the generated value.
        /// </summary>                                                 
        void SetPoolBounds()
        {
            enemyProjectileNum = amountToPool / 2;
            playerProjectileNum = enemyProjectileNum;
        }

        /// <summary>
        /// Call to populate the passed Queue reference with iterations amount of bulletType bullets.
        /// </summary>
        /// <param name="pool">The Queue to populate.</param>
        /// <param name="iterations">How many bullets to create.</param>
        /// <param name="anchor">The parent gameObject in the hierarchy</param>
        /// <param name="bulletType">What bullet type to create?</param>
        void PopulatePool(ref Queue<GameObject> pool, int iterations, Transform anchor, BulletType bulletType)
        {
            for (int i = 0; i < iterations; i++)
            {
                GameObject obj = Instantiate(bulletPrefab);
                obj.transform.SetParent(anchor, true);
                obj.SetActive(false);

                Bullet objScript = obj.GetComponent<Bullet>();
                if (objScript != null)
                {
                    objScript.SetBulletType(bulletType);

                    //Queue the now created bullet
                    pool.Enqueue(obj);
                }
            }
        }
        #endregion

        #region UTILITIES
        /// <summary>
        /// Call to get a Bullet Gameobject reference based on the passed type.
        /// <para>Returns null in case the pool is empty.</para>
        /// </summary>
        public GameObject GetPooledBulletByType(BulletType bulletType)
        {
            //Grab the first bullet GO
            switch (bulletType)
            {
                case BulletType.Enemy:
                    GameObject enemyBullet = enemyProjectiles.Dequeue();
                    return enemyBullet;

                case BulletType.Player:
                    GameObject playerBullet = playerProjectiles.Dequeue();
                    return playerBullet;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Call to cache the passed Bullet gameObject back to its appropriate Queue based on the bulletType.
        /// </summary>
        /// <param name="bullet">The bullet script attached to the gameObject.</param>
        public void CacheBullet(Bullet bullet, BulletType bulletType)
        {
            switch (bulletType)
            {
                case BulletType.Enemy:
                    bullet.gameObject.SetActive(false);
                    enemyProjectiles.Enqueue(bullet.gameObject);
                    break;

                case BulletType.Player:
                    bullet.gameObject.SetActive(false);
                    playerProjectiles.Enqueue(bullet.gameObject);
                    break;

            }
        }
        #endregion
    }
}