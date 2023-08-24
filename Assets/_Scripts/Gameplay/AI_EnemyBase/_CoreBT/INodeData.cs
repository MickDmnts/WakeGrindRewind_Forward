using UnityEngine;
using UnityEngine.AI;

namespace WGR.Gameplay.AI
{
    /*
     * This interface is used to ensure type safety of the classes passed throughout the 
     * Behaviour trees.
     */
    public interface INodeData
    {
        public void SetTargetIsDead(bool value);
        public bool GetTargetIsDead();

        public void SetNavMeshAgent(NavMeshAgent agent);
        public NavMeshAgent GetNavMeshAgent();

        public void SetTarget(Transform target);
        public Transform GetTarget();

        public void SetOcclusionLayers(LayerMask value);
        public LayerMask GetOcclusionLayers();

        public void SetWeaponRange(float value);
        public float GetWeaponRange();

        public void SetCanShoot(bool value);
        public bool GetCanShoot();

        public void SetTargetFound(bool value);
        public bool GetTargetFound();

        public void SetOriginalPos(Vector3 pos);
        public Vector3 GetOriginalPos();
    }
}