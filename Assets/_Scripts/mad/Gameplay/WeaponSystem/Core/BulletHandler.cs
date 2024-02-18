using UnityEngine;

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

    /// <summary>
    /// The bullet handler of the game
    /// </summary>
    [DefaultExecutionOrder(50)]
    public class BulletHandler : CoreBehaviour
    {
        ///<summary>The addressable bullet path</summary>
        [Header("Set in inspector - Bullet prefab")]
        [SerializeField, Tooltip("The addressable bullet path")] string bulletPath;

        protected override void PreAwake()
        { SetID("bulletHandler"); }

        /// <summary>
        /// Call to get a Bullet Gameobject reference based on the passed type.
        /// </summary>
        public GameObject GetBullet()
        { return UnityAssets.Load(bulletPath, false); }
    }
}