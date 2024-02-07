using UnityEngine;

namespace WGRF.AI
{
    public class EnforcerNodeData : INodeData
    {
        AIEntity enforcerEntity;

        LayerMask occlusionLayers;
        bool canProtect;

        AIEntity lowestHpEnemy;

        public EnforcerNodeData(EnforcerEntity enforcerEntity)
        { this.enforcerEntity = enforcerEntity; }

        public AIEntity EnemyEntity => enforcerEntity;
        public AIEntityAnimations EnemyAnimations => enforcerEntity.Controller.Access<EnforcerAnimations>("eAnimations");
        public LayerMask OcclusionLayers => occlusionLayers;
        public bool IsDead { get => enforcerEntity.IsDead; set { enforcerEntity.IsDead = value; } }
        public bool CanProtect { get => canProtect; set { canProtect = value; } }
        public Transform Target { get => ((EnforcerEntity)enforcerEntity).Target; }
        public float WeaponRange { get => enforcerEntity.Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.MinShootDistance; }
        public AIEntity LowestHpEnemy { get => lowestHpEnemy; set => lowestHpEnemy = value; }
    }
}