using UnityEngine;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be assigned from the inspector.
     * Private Variables: These variables change in runtime.
     * 
     * [Class flow]
     * 1. The main entry point of this script is the OnTriggerEnter that gets called when the player
     *  enters its trigger volume.
     * 2. When triggered:
     *      A. If forceLoader is enabled and a levelLoadPacket is assigned then the 
     *          handler calls the LevelHandler.ForceLoad(...) and passes the assigned from inspector load packet.
     *      B. Otherwise, the handler calls the LevelHandler.LoadNextLevel() to load the next assigned level.
     * 
     * [Must know]
     * 1. When the player touches the trigger the handler calls the GameEventHandler.OnLevelPassed(currentScene).
     */
    public class TransitLevel : MonoBehaviour
    {
        #region INSPECTOR_VALUES
        [Header("Set in inspector")]
        [SerializeField] bool forceLoader;
        [SerializeField] LevelLoadPacket forceLoadScenePacket;
        #endregion

        #region PRIVATE_VARIABLES
        [SerializeField]
        bool hasBlocker;
        [SerializeField]
        bool canTransit = false;
        GameObject elevatorBlocker;
        #endregion

        private void Start()
        {
            if (hasBlocker)
            {
                //Cache the elevator blocker and deactivate it.
                elevatorBlocker = GetComponentInChildren<SpriteRenderer>().gameObject;
                elevatorBlocker.SetActive(true);
            }
        }

        /// <summary>
        /// Call to disable the elevator blocker sprite (if enabled),
        /// play the ambience music and the elevator SFX.
        /// <para>Sets canTransit to true.</para>
        /// <para>Does not work in LEVEL5 because level transitioning
        /// is activated by BossStunnedSequence script.</para>
        /// </summary>
        public void EnableLevelTransition()
        {
            if (GameManager.S != null
                && GameManager.S.LevelManager.FocusedScene == GameScenes.Level5)
            {
                elevatorBlocker.SetActive(false);
                canTransit = true;
                return;
            }

            //Disable the elevator blocker
            if (hasBlocker)
            {
                elevatorBlocker.SetActive(false);
            }

            //Play the sound SFXs
            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.ForceAmbienceSFX();
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.ElevatorArrival);
            }

            canTransit = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //OnTriggerEnter can work only if canTransit is true
            if (!canTransit) return;

            //Behaviour if the transit has a scriptable packet for the next level
            if (forceLoader && other.GetComponent<PlayerEntity>())
            {
                ForceTransitBehaviour();
                return;
            }

            //The default behaviour of the level transitioning
            if (other.CompareTag("Player"))
            {
                DefaultTransitBehaviour();
            }
        }

        /// <summary>
        /// Call to load the ,assigned from inspector packet, levels.
        /// <para>Calls NotifyOnLevelPassed.</para>
        /// </summary>
        void ForceTransitBehaviour()
        {
            NotifyOnLevelPassed();

            if (GameManager.S != null)
                GameManager.S.LevelManager.ForceLoad(forceLoadScenePacket.PacketIndex);
        }

        /// <summary>
        /// Call to load the next level through LevelManager.
        /// </summary>
        void DefaultTransitBehaviour()
        {
            if (GameManager.S == null)
            {
                Debug.Log("GameManager Singleton is null");
                return;
            }

            NotifyOnLevelPassed();

            GameManager.S.LevelManager.LoadNext(false);
        }

        /// <summary>
        /// Call to notify the GameManager.GameEventsHandler that the current level is finished.
        /// </summary>
        void NotifyOnLevelPassed()
        {
            GameScenes currentScene = GameManager.S.LevelManager.FocusedScene;
            GameManager.S.GameEventHandler.OnLevelPassed(currentScene);
        }
    }
}