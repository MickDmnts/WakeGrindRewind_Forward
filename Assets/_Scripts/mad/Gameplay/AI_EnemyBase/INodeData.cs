using UnityEngine;

namespace WGRF.AI
{
    /// <summary>
    /// Base interface for AI Node Data classes
    /// </summary>
    public interface INodeData
    {
        public AIEntity EnemyEntity { get; }
        public AIEntityAnimations EnemyAnimations { get; }
        public LayerMask OcclusionLayers { get; }
        public bool IsDead { get; set; }
        public bool CanShoot { get; set; }
        public Transform Target { get; set; }
        public float WeaponRange { get; }
    }
}