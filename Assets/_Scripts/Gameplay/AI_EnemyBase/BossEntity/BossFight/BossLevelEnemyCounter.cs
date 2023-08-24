using UnityEngine;
using WGR.Core;

namespace WGR.Gameplay.AI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These values must be set from the inspector
     * Private Variables: These values change in runtime.
     * 
     * [Must know]
     * 1. The CheckCountForBossFightStart() method is subscribed in the GameEventHandler.onBossLevelEnemyDeath event and gets 
     *      called when an enemy dies in level 5.
     */
    public class BossLevelEnemyCounter : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] int enemyCountForBossFight;
        [SerializeField] GameObject bossEntryTrigger;

        //Private variables
        int enemiesKilled;
        bool enemyThresholdPassed = false;

        private void Awake()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onBossLevelEnemyDeath += CheckCountForBossFightStart;
            }
        }

        /// <summary>
        /// Call to increase the enemiesKilled count by 1.
        /// <para>If the enemiesKilled surpasses AND the enemyThresholdPassed is false,
        /// <para>the enemyCountForBossFight value the FAKE level passed SFXs get played.</para></para>
        /// </summary>
        void CheckCountForBossFightStart()
        {
            enemiesKilled++;

            if (enemiesKilled >= enemyCountForBossFight && !enemyThresholdPassed)
            {
                enemyThresholdPassed = true;
                bossEntryTrigger.SetActive(true);

                if (GameManager.S != null)
                {
                    GameManager.S.GameSoundsHandler.ForceAmbienceSFX();
                    GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.ElevatorArrival);
                }
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onBossLevelEnemyDeath -= CheckCountForBossFightStart;
            }
        }
    }
}