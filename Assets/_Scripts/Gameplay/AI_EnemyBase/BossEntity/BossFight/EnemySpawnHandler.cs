using System.Collections;
using UnityEngine;

using WGR.AI.Entities;
using WGR.Core.Managers;
using WGR.Entities;

namespace WGR.Scripted
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These values must be set from the inspector
     * 
     * [Must know]
     * 1. This script is responsible for moving the boss and boss minions along with revealing the hidden boss room to the player.
     * 2. The SpawnBoss method gets called when the player activates a trigger after the FAKE level ending has been activated.
     * 3. The SpawnEnemyWave method gets called when the boss enters his SECOND phase of battle.
     */
    public class EnemySpawnHandler : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("Set in inspector")]
        [SerializeField] Transform spawnPoint;

        [SerializeField] GameObject bossEntity;
        [SerializeField] Transform bossEntryPosition;

        [SerializeField] DoorKickable firstPhaseDoor;
        [SerializeField] DoorKickable secondPhaseDoor;

        [SerializeField] GameObject[] wave1Spawns;

        [SerializeField] GameObject room1Mask;
        [SerializeField] GameObject room2Mask;
        #endregion

        /// <summary>
        /// Call to move the boss in the bossEntryPosition transform.
        /// </summary>
        public void SpawnBoss()
        {
            //This is done so the agent can pass through unwalkable planes.
            bossEntity.SetActive(false);
            bossEntity.transform.position = spawnPoint.position;
            bossEntity.SetActive(true);

            //Moves the agent in front of the player.
            AIEntity bossAIEntity = bossEntity.GetComponent<AIEntity>();
            if (bossAIEntity != null)
            {
                bossAIEntity.GetEntityNodeData().SetOriginalPos(spawnPoint.position);
                bossAIEntity.GetAgent().SetDestination(bossEntryPosition.position);
            }
            else Debug.LogError("The BossAIEntity could not get cached in SpawnBoss() method.");

            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.ElevatorArrival);
            }
        }

        /// <summary>
        /// Call to move and activate the boss minions in front of the elevator.
        /// <para>Reveals the first boss hiding room.</para>
        /// </summary>
        public void SpawnEnemyWave()
        {
            StartCoroutine(MoveEntities(wave1Spawns));

            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.ElevatorArrival);
            }

            UnmaskRoom(room1Mask);
            firstPhaseDoor.External_UnlockDoor();
        }

        /// <summary>
        /// Call to move and activate the boss minions in front of the elevator.
        /// </summary>
        IEnumerator MoveEntities(GameObject[] arrayOfEntities)
        {
            foreach (GameObject entity in arrayOfEntities)
            {
                //This is done so the agent can pass through unwalkable planes.
                entity.SetActive(false);
                entity.transform.position = spawnPoint.position;
                entity.SetActive(true);

                AIEntity aIEntity = entity.transform.root.GetComponent<AIEntity>();

                yield return new WaitForEndOfFrame(); //We wait a frame

                if (aIEntity != null)
                {
                    aIEntity.GetEntityNodeData().SetOriginalPos(bossEntity.transform.position);

                    yield return new WaitForSeconds(0.5f);

                    aIEntity.OnPlayerFound();
                }
                else Debug.LogError("The BossAIEntity could not get cached in SpawnBoss() method.");

                yield return new WaitForSecondsRealtime(0.2f);
            }

            yield return null;
        }

        /// <summary>
        /// Call to reveal the final boss room.
        /// </summary>
        public void EnableFinalBossRoom()
        {
            UnmaskRoom(room2Mask);

            secondPhaseDoor.External_UnlockDoor();
        }

        /// <summary>
        /// Call to move the passed gameobject in an off-camera position.
        /// </summary>
        /// <param name="roomRoot"></param>
        void UnmaskRoom(GameObject roomRoot)
        {
            roomRoot.transform.position += new Vector3(1000f, 0f, 1000f);
        }
    }
}