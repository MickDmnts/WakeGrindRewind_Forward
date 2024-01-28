using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /*
     * This class file is purely used as a data container for the necessary
     * data and info the enemy Behaviour Tree will use.
     */
    [System.Serializable]
    public class EnemyNodeData : INodeData
    {
        #region GENERAL_USAGE
        EnemyEntity enemyEntity; //Cached through the EnemyNodeData creation.
        EnemyAnimations enemyAnimations; //Cached through the EnemyNodeData creation.
        NavMeshAgent agent; //Cached through the EnemyNodeData creation.

        bool attackAfterStun; //Inspector given value from EnemyEntity

        LayerMask occlusionLayers; //Inspector given value from EnemyEntity

        Vector3 originalPosition; //EnemyEntity controlled value.
        #endregion

        #region BehaviourTree flow variables
        bool isPatroller; //Inspector given value from EnemyEntity

        bool isDead; //EnemyEntity controlled value.
        bool isStunned; //EnemyEntity controlled value.
        bool canShoot; //EnemyEntity controlled value.
        #endregion

        #region Patrolling behaviour specifics
        List<Transform> waypoints; //Inspector given value from EnemyEntity
        float idleTimeOnPatrol = 0; //Inspector given value from EnemyEntity

        int currentWaypointIndex = 0; //Set through the Patrolling behaviour master node.
        float waypointDistanceOffset; //Inspector given value from EnemyEntity
        #endregion

        #region Attack behvaiour specifics
        Transform target; //Set from AIEntity manager.

        bool targetIsDead; //Set from enemy entity script.
        bool targetFound; //Set from AIFovManagers
        float weaponRange; //Set from the enemy equiped weapon.
        #endregion

        #region SETTERS
        //General Usage
        public void SetEnemyEntity(EnemyEntity value) => enemyEntity = value;
        public void SetEnemyAnimations(EnemyAnimations value) => enemyAnimations = value;
        public void SetNavMeshAgent(NavMeshAgent value) => agent = value;
        public void SetAttackAfterStun(bool value) => attackAfterStun = value;
        public void SetOriginalPos(Vector3 pos) => originalPosition = pos;
        public void SetOcclusionLayers(LayerMask value) => occlusionLayers = value;

        //BEHAVIOU FLOW
        public void SetIsPatroller(bool value) => isPatroller = value;
        public void SetIsDead(bool value) => isDead = value;
        public void SetIsStunned(bool value) => isStunned = value;
        public void SetCanShoot(bool value) => canShoot = value;

        //Patrolling
        public void SetWaypoints(List<Transform> value) => waypoints = value;
        public void SetCurrentWaypointIndex(int value) => currentWaypointIndex = value;
        public void SetIdleTimeOnPatrol(float value) => idleTimeOnPatrol = value;
        public void SetWaypointDistanceOffset(float value) => waypointDistanceOffset = value;

        //Attacking
        public void SetTarget(Transform value) => target = value;
        public void SetTargetIsDead(bool value) => targetIsDead = value;
        public void SetTargetFound(bool value) => targetFound = value;
        public void SetWeaponRange(float value) => weaponRange = value;
        #endregion

        #region GETTERS
        //General usage
        public EnemyEntity GetEnemyEntity() => enemyEntity;
        public EnemyAnimations GetEnemyAnimations() => enemyAnimations;
        public NavMeshAgent GetNavMeshAgent() => agent;
        public bool GetAttackAfterStun() => attackAfterStun;
        public Vector3 GetOriginalPos() => originalPosition;
        public LayerMask GetOcclusionLayers() => occlusionLayers;

        //BEHAVIOUR FLOW
        public bool GetIsPatroller() => isPatroller;
        public bool GetIsDead() => isDead;
        public bool GetIsStunned() => isStunned;
        public bool GetCanShoot() => canShoot;

        //Patrolling
        public List<Transform> GetWaypoints() => waypoints;
        public int GetCurrentWaypointIndex() => currentWaypointIndex;
        public float GetIdleTimeOnPatrol() => idleTimeOnPatrol;
        public float GetWaypointDistanceOffset() => waypointDistanceOffset;

        //Attacking
        public Transform GetTarget() => target;
        public bool GetTargetIsDead() => targetIsDead;
        public bool GetTargetFound() => targetFound;
        public float GetWeaponRange() => weaponRange;
        #endregion
    }
}