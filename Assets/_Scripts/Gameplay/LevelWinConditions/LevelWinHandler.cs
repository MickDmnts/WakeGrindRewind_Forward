using System.Collections;
using UnityEngine;

using WGR.Core;

namespace WGR.Gameplay
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Must Know]
     * 1. This script is attached in a gameObject in each scene and checks if all the enemies are killed to enable the next level transition system.
     */
    public class LevelWinHandler : MonoBehaviour
    {
        //Private variables
        int totalEnemiesInLevel = 0;
        TransitLevel transitLevelHandle;

        private void Awake()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to set up the script behaviour when the scene loads.
        /// </summary>
        void EntrySetup()
        {
            if (GameManager.S != null)
                GameManager.S.GameEventHandler.onEnemyDeath += DecreaseEnemyCount;

            transitLevelHandle = FindObjectOfType<TransitLevel>();
        }

        private void Start()
        {
            StartCoroutine(GetBelatedEnemyData());
        }

        /// <summary>
        /// Call to wait a frame an then get the enemy level count from the GameManager
        /// and then invoke CheckWinConditionsMet();
        /// </summary>
        IEnumerator GetBelatedEnemyData()
        {
            yield return new WaitForEndOfFrame();

            totalEnemiesInLevel = GameManager.S.AIEntityManager.GetEnemyRefCount();
            CheckWinConditionsMet();

            yield return null;
        }

        /// <summary>
        /// Call to decrease the totalEnemiesInLevel by one and invoke the CheckWinConditionsMet() method.
        /// </summary>
        void DecreaseEnemyCount()
        {
            totalEnemiesInLevel--;

            CheckWinConditionsMet();
        }

        /// <summary>
        /// Call to check if all level enemies are killed, if yes call the EnableLevelTransition() from TransitLevel script.
        /// </summary>
        void CheckWinConditionsMet()
        {
            if (totalEnemiesInLevel <= 0)
            {
                //Open elevator door and play ching sound;
                transitLevelHandle?.EnableLevelTransition();
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onEnemyDeath -= DecreaseEnemyCount;
            }
        }
    }
}