using UnityEngine;

namespace WGRF.AI
{
    /*
     * This class file is purely used as a data container for the necessary
     * data and info the enemy Behaviour Tree will use.
     */
    [System.Serializable]
    public class EnemyNodeData : INodeData
    {
        AIEntity enemyEntity; //Handler of this NodeData

        LayerMask occlusionLayers; //Inspector given value from EnemyEntity
        bool isDead; //EnemyEntity controlled value.
        bool canShoot; //EnemyEntity controlled value.

        public AIEntity EnemyEntity => enemyEntity;
        public AIEntityAnimations EnemyAnimations => enemyEntity.Controller.Access<EnemyAnimations>("eAnimations");
        public LayerMask OcclusionLayers => occlusionLayers;
        public bool IsDead { get => isDead; set { isDead = value; } }
        public bool CanShoot { get => canShoot; set { canShoot = value; } }
        public Transform Target { get => ((EnemyEntity)enemyEntity).Target; set { enemyEntity.SetAttackTarget(value); } }
        public float WeaponRange { get => enemyEntity.Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.MinShootDistance; }
    }
}