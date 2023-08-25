using UnityEngine;
using UnityEngine.AI;
using WGR.AI.Entities.Hostile.Boss;

namespace WGR.AI.Nodes
{
    /*
     * This class file is purely used as a data container for the necessary
     * data and info the Boss Behaviour Tree will use.
     */
    [System.Serializable]
    public class BossNodeData : INodeData
    {
        #region GENERAL_USE
        BossEntity enemyEntity; //Cached through the BossNodeData creation.
        BossAnimations enemyAnimations; //Cached through the BossNodeData creation.
        NavMeshAgent agent; //Cached through the BossNodeData creation.

        LayerMask occlusionLayers; //Inspector given value from EnemyEntity

        Vector3 originalPosition; //BossEntity controlled value.
        #endregion

        #region Behaviour Tree Flow
        BossPhase currentBossPhase; //Assigned at the Boss agent creation and changed through the fight
        Transform currentHideRoom; //Changed throughout the boss fight.

        bool isDead; //BossEntity controlled value.
        bool isHiding; //BossEntity controlled value.
        bool isStunned; //BossEntity controlled value.
        #endregion

        #region Attack behaviour flow
        Transform target; //Assigned through the AIEntity manager.
        bool canShoot;//BossEntity controlled value.

        bool targetIsDead; //Set from BossEntity 
        bool targetFound = false; //Set from FOVManager.
        float weaponRange; //controlled from set weapon
        #endregion

        #region GETTERS
        public void SetEnemyEntity(BossEntity value) => enemyEntity = value;
        public void SetEnemyAnimations(BossAnimations value) => enemyAnimations = value;
        public void SetNavMeshAgent(NavMeshAgent value) => agent = value;
        public void SetCanShoot(bool value) => canShoot = value;
        public void SetOriginalPos(Vector3 pos) => originalPosition = pos;
        public void SetOcclusionLayers(LayerMask value) => occlusionLayers = value;

        public void SetIsDead(bool value) => isDead = value;
        public void SetCurrentBossPhase(BossPhase bossPhase) => currentBossPhase = bossPhase;
        public void SetCurrentHideRoom(Transform transform) => currentHideRoom = transform;
        public void SetIsHiding(bool value) => isHiding = value;
        public void SetIsStunned(bool value) => isStunned = value;

        public void SetTarget(Transform value) => target = value;
        public void SetTargetFound(bool value) => targetFound = value;
        public void SetWeaponRange(float value) => weaponRange = value;

        public void SetTargetIsDead(bool value) => targetIsDead = value;
        #endregion

        #region SETTERS
        public BossEntity GetEnemyEntity() => enemyEntity;
        public BossAnimations GetEnemyAnimations() => enemyAnimations;
        public NavMeshAgent GetNavMeshAgent() => agent;
        public Vector3 GetOriginalPos() => originalPosition;
        public bool GetCanShoot() => canShoot;
        public LayerMask GetOcclusionLayers() => occlusionLayers;

        public bool GetIsDead() => isDead;
        public BossPhase GetCurrentBossPhase() => currentBossPhase;
        public Transform GetCurrentHideRoom() => currentHideRoom;
        public bool GetIsHiding() => isHiding;
        public bool GetIsStunned() => isStunned;

        public Transform GetTarget() => target;
        public bool GetTargetFound() => targetFound;
        public float GetWeaponRange() => weaponRange;

        public bool GetTargetIsDead() => targetIsDead;
        #endregion
    }
}
