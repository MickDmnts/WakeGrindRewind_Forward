using System.Collections;
using UnityEngine;
using WGR.AI.Entities.Hostile.Boss;
using WGR.Core.Managers;
using WGR.UI;

namespace WGR.Scripted
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These variables must be set from the inspector
     * Private Variables: These values change in runtime.
     * 
     * [Must know]
     * 1. The script handles the player decision to either kill or spare the boss.
     * 2. If the player kills the boss the Bad Ending sequence gets initiated.
     * 3. The last boss dialogue gets handled from this script too.
     */
    public class BossStunnedSequence : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] ButtonPrompt buttonPrompt;
        [SerializeField] GameObject elevatorBlocker;

        #region PRIVATE_VARIABLES
        BossEntity bossEntity;
        DialogueController dialogueController;

        bool promptActivated;
        bool insidePromptRange;

        bool canContinue;
        #endregion

        private void Start()
        {
            if (GameManager.S != null)
            { GameManager.S.GameEventHandler.onBossStunnedPhase += ActivateBossPrompt; }
            else
            { Utils.MissingComponent("GameManager", this); }

            EntrySetup();
        }

        /// <summary>
        /// Call to setup the script on scene loading.
        /// </summary>
        void EntrySetup()
        {
            if ((dialogueController = FindObjectOfType<DialogueController>()) == null)
            { Utils.MissingComponent("DialogueController", this); }

            if ((bossEntity = FindObjectOfType<BossEntity>()) == null)
            { Utils.MissingComponent("bossEntity", this); }

            //Enables the fake elevator door.
            if (elevatorBlocker != null)
            { elevatorBlocker.SetActive(true); }
            else { Utils.MissingComponent("ElevatorBlocker", this); }

            buttonPrompt.gameObject.SetActive(false);
            promptActivated = false;
        }


        #region BOSS_STUNNED_INTERACTION
        /// <summary>
        /// Call to move the button prompt in the boss position.
        /// </summary>
        void ActivateBossPrompt()
        {
            transform.position = bossEntity.transform.position;
            buttonPrompt.transform.position = transform.position;

            buttonPrompt.gameObject.SetActive(true);
            promptActivated = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                insidePromptRange = true;
            }
        }

        private void Update()
        {
            if (promptActivated && insidePromptRange)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    promptActivated = false;
                    buttonPrompt.gameObject.SetActive(false);

                    StartBossDialogueSequence();
                }
            }
        }

        /// <summary>
        /// Call to start the boss final dialogue.
        /// </summary>
        void StartBossDialogueSequence()
        {
            StartCoroutine(BossDialogue());
        }

        /// <summary>
        /// Call to set canContinue to true.
        /// </summary>
        void EnableContinuation()
        {
            canContinue = true;
        }

        /// <summary>
        /// Call display the final boss dialogue and enable the REAL level transition.
        /// </summary>
        IEnumerator BossDialogue()
        {
            canContinue = false;

            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.IsActive = false;

                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);
            }

            StartCoroutine(dialogueController.TypeSentence("Killing me won't bring her back" +
                "\nPull the trigger and condemn yourself one more time", EnableContinuation, 0.01f, false));

            while (!Input.GetKeyDown(KeyCode.E) || !canContinue)
            {
                yield return null;
            }

            //Opens the elevator door and activates the player controller.
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);

                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.ElevatorArrival);

                GameManager.S.PlayerEntity.IsActive = true;
            }

            elevatorBlocker.SetActive(false);
            canContinue = false;

            yield return null;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                insidePromptRange = false;
            }
        }
#endregion

        #region BAD_ENDING
        /// <summary>
        /// Call to start the bad ending sequence.
        /// <para>Call to display one last dialogue before moving the player to the player hub.</para>
        /// </summary>
        public void Ending_KillBossAndContinue()
        {
            StartCoroutine(BadEndingSequence());
        }

        /// <summary>
        /// Call to display one last dialogue before moving the player to the player hub.
        /// </summary>
        IEnumerator BadEndingSequence()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);
            }

            StartCoroutine(FindObjectOfType<DialogueController>().TypeSentence("The cycle never ends...", EnableContinuation, 0.01f,
                waitForInput: false, dontShowButtonPrompt: true));

            yield return new WaitForSeconds(5f);

            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);
                GameManager.S.LevelManager.TransitToPlayerHub();
            }

            yield return null;
        }
        #endregion

        private void OnDestroy()
        {
            if (GameManager.S != null)
            { GameManager.S.GameEventHandler.onBossStunnedPhase -= ActivateBossPrompt; }
        }
    }
}