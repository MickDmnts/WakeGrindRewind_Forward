using UnityEngine;

namespace WGRF.AI
{
    public class EnemyNodeData : INodeData
    {
        AIEntity enemyEntity;

        LayerMask occlusionLayers;
        bool canAttack;
        bool canShoot;

        public EnemyNodeData(EnemyEntity enemyEntity)
        { this.enemyEntity = enemyEntity; }

        public AIEntity EnemyEntity => enemyEntity;
        public AIEntityAnimations EnemyAnimations => enemyEntity.Controller.Access<EnemyAnimations>("eAnimations");
        public LayerMask OcclusionLayers => occlusionLayers;
        public bool IsDead { get => enemyEntity.IsDead; set { enemyEntity.IsDead = value; } }
        public bool CanAttack { get => canAttack; set { canAttack = value; } }
        public bool CanShoot { get => canShoot; set { canShoot = value; } }
        public Transform Target { get => ((EnemyEntity)enemyEntity).Target; }
        public float WeaponRange { get => enemyEntity.Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.MinShootDistance; }
    }
}