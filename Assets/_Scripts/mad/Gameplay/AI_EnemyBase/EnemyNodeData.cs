using UnityEngine;

namespace WGRF.AI
{   
    /// <summary>
    /// Caches the agents node data information
    /// </summary>
    public class EnemyNodeData : INodeData
    {   
        ///<summary>Reference to the agent handler</summary>
        AIEntity enemyEntity;

        ///<summary>The layers the agent traverses around</summary>
        LayerMask occlusionLayers;
        ///<summary>Can the agent attack?</summary>
        bool canAttack;
        ///<summary>Can the agent shoot?</summary>
        bool canShoot;

        /// <summary>
        /// Creates an enemy node data instance
        /// </summary>
        /// <param name="enemyEntity">The agent handler of this cache</param>
        public EnemyNodeData(EnemyEntity enemyEntity)
        { this.enemyEntity = enemyEntity; }

        ///<summary>Returns the agent of this data cache</summary>
        public AIEntity EnemyEntity => enemyEntity;
        ///<summary>Returns the enemy animation handler</summary>
        public AIEntityAnimations EnemyAnimations => enemyEntity.Controller.Access<EnemyAnimations>("eAnimations");
        ///<summary>Returns the enemy occlusion layers</summary>
        public LayerMask OcclusionLayers => occlusionLayers;
        ///<summary>Is this agent dead?</summary>
        public bool IsDead { get => enemyEntity.IsDead; set { enemyEntity.IsDead = value; } }
        ///<summary>Can this agent attack?</summary>
        public bool CanAttack { get => canAttack; set { canAttack = value; } }
        ///<summary>Can this agent shoot?</summary>
        public bool CanShoot { get => canShoot; set { canShoot = value; } }
        ///<summary>This agent's target</summary>
        public Transform Target { get => ((EnemyEntity)enemyEntity).Target; }
        ///<summary>The equiped weapon range</summary>
        public float WeaponRange { get => enemyEntity.Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.MinShootDistance; }
    }
}