using UnityEngine;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be set from the inspector
     * Private variable: These variables change in runtime.
     *  
     *  This script is attached to a trigger and spawns an enemy when the player enters
     *  its vicinity.
     *
     */
    public class ActivateAbilityTest : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] GameObject enemyPrefab;
        [SerializeField] Transform spawnPoint;

        //Private variable
        GameObject lastSpawnedEnemy;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SpawnEnemyDummy();
            }
        }

        /// <summary>
        /// Call to spawn a dummy enemy in spawnPoint position.
        /// </summary>
        void SpawnEnemyDummy()
        {
            if (lastSpawnedEnemy != null)
            {
                lastSpawnedEnemy.SetActive(false);
                Destroy(lastSpawnedEnemy);
            }

            lastSpawnedEnemy = Instantiate(enemyPrefab);
            lastSpawnedEnemy.transform.position = spawnPoint.position;
            lastSpawnedEnemy.SetActive(true);
        }
    }
}