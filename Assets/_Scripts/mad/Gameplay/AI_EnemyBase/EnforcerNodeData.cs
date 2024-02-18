using UnityEngine;

namespace WGRF.AI
{
    /// <summary>
    /// Caches the agents node data information
    /// </summary>
    public class EnforcerNodeData : INodeData
    {
        ///<summary>Reference to the agent handler</summary>
        AIEntity enforcerEntity;
        ///<summary>The layers the agent traverses around</summary>
        LayerMask occlusionLayers;
        ///<summary>Can the agent protect?</summary>
        bool canProtect;
        ///<summary>The current lowest health enemy</summary>
        AIEntity lowestHpEnemy;

        /// <summary>
        /// Creates an enemy node data instance
        /// </summary>
        /// <param name="enemyEntity">The agent handler of this cache</param>
        public EnforcerNodeData(EnforcerEntity enforcerEntity)
        { this.enforcerEntity = enforcerEntity; }

        ///<summary>Returns the agent of this data cache</summary>
        public AIEntity EnemyEntity => enforcerEntity;
        ///<summary>Returns the enemy animation handler</summary>
        public AIEntityAnimations EnemyAnimations => enforcerEntity.Controller.Access<EnforcerAnimations>("eAnimations");
        ///<summary>Returns the enemy occlusion layers</summary>
        public LayerMask OcclusionLayers => occlusionLayers;
        ///<summary>Is this agent dead?</summary>
        public bool IsDead { get => enforcerEntity.IsDead; set { enforcerEntity.IsDead = value; } }
        ///<summary>Can the agent protect?</summary>
        public bool CanProtect { get => canProtect; set { canProtect = value; } }
        ///<summary>This agent's target</summary>
        public Transform Target { get => ((EnforcerEntity)enforcerEntity).Target; }
        ///<summary>The equiped weapon range</summary>
        public float WeaponRange { get => enforcerEntity.Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.MinShootDistance; }
        ///<summary>The current lowest health enemy</summary>
        public AIEntity LowestHpEnemy { get => lowestHpEnemy; set => lowestHpEnemy = value; }
    }
}