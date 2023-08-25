using System.Collections.Generic;
using UnityEngine;

using WGR.AI.Entities;
using WGR.AI.FOV;

namespace WGR.Core.Managers
{
    /* CLASS DOCUMENTATION *\
     * 
     * [Variable Specifics]
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Must Know]
     * 1. The EnemyEntity list is created anew whenever the scene changes, this is faster than using .Clear().
     * 2. When the scene loads every AIEntity in the scene PASSES its reference to the manager, not the opposite.
     * 3. Every player ability uses the GetEnemyEntityRefs() and GetEnemyRefCount() for enemy entity manipulation.
     */

    public class AIEntityManager : MonoBehaviour
    {
        //Dynamically changed
        List<AIEntity> aiEntitiesInLevel;
        List<AIEntityFOVManager> aIEntityFOVManagers;

        AIEntityFOVManager bossFOVManager;

        private void Awake()
        {
            //Create the list of the enemies
            aiEntitiesInLevel = new List<AIEntity>();
            aIEntityFOVManagers = new List<AIEntityFOVManager>();

            //Called whenever the scene is changed
            GameManager.S.GameEventHandler.onSceneUnloaded += ClearEntityList;
        }

        /// <summary>
        /// Called from each enemy in the level when it loads.
        /// </summary>
        public void AddEntityReference(AIEntity entity)
        {
            if (!aiEntitiesInLevel.Contains(entity))
            {
                aiEntitiesInLevel.Add(entity);
                aIEntityFOVManagers.Add(entity.GetComponentInChildren<AIEntityFOVManager>());

                entity.SetAttackTarget(GameManager.S.PlayerEntity.transform);
                entity.SetIsAgentActive(true);
            }
        }

        public void SetAgentBehaviourUpdate(bool value)
        {
            foreach (AIEntity agent in aiEntitiesInLevel)
            {
                agent.SetIsAgentActive(value);
            }
        }

        #region PLAYER_USAGE
        /// <summary>
        /// Call to get a reference to the EnemyEntity list.
        /// </summary>
        /// <returns></returns>
        public List<AIEntity> GetEnemyEntityRefs()
        {
            return aiEntitiesInLevel;
        }

        public void ActivateDetectors()
        {
            foreach (AIEntityFOVManager agentFOVManager in aIEntityFOVManagers)
            {
                if (agentFOVManager.IsDetectorActive) continue;

                agentFOVManager.ActivateAllDetectors();
            }
        }

        public void SetBossDetectors(AIEntityFOVManager bossFOVManager)
        {
            this.bossFOVManager = bossFOVManager;
        }

        public void ActivateBossPlayerDetectors()
        {
            bossFOVManager.ActivateAllDetectors();
        }

        /// <summary>
        /// Call to get an EnemyEntity list count.
        /// </summary>
        /// <returns></returns>
        public int GetEnemyRefCount()
        {
            return aiEntitiesInLevel.Count;
        }
        #endregion

        /// <summary>
        /// Called whenever the scene is changed.
        /// Creates a new List<EnemyEntity>(), does not use .Clear();
        /// </summary>
        /// <param name="activeScene"></param>
        void ClearEntityList(GameScenes activeScene)
        {
            aiEntitiesInLevel = new List<AIEntity>();
        }

        public void KillAllEnemiesInLevel()
        {
            foreach (AIEntity enemy in aiEntitiesInLevel)
            {
                if (enemy == null) continue;

                do
                {
                    enemy.AttackInteraction();
                } while (!enemy.GetIsDead());
            }
        }

        private void OnDestroy()
        {
            GameManager.S.GameEventHandler.onSceneUnloaded -= ClearEntityList;
        }
    }
}