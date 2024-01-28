using UnityEngine;
using WGRF.BattleSystem;

namespace WGRF.Core
{
    /// <summary>
    /// Every available bullet type in the game.
    /// </summary>
    public enum BulletType
    {
        Enemy = 12,
        Player = 13,
    }

    [DefaultExecutionOrder(50)]
    public class BulletHandler : MonoBehaviour
    {
        [Header("Set in inspector - Bullet prefab")]
        [SerializeField] string bulletPath;

        /* [Header("Set in inspector - Projectile layers")]
        [SerializeField] LayerMask playerBulletLayer;
        [SerializeField] LayerMask enemyBulletLayer; */

        [Header("Set in inspector - Bullet default speed")]
        [SerializeField] float startingSpeed;

        /// <summary>
        /// Call to get a Bullet Gameobject reference based on the passed type.
        /// </summary>
        public GameObject GetBullet()
        { return UnityAssets.Load(bulletPath, false); }
    }
}