using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WGR.Core
{
    /// <summary>
    /// This class is used for better sentence-icon handling in the inspector.
    /// </summary>
    [System.Serializable]
    public class ScriptedSequence
    {
        public List<string> sentences;
        public List<Sprite> imagePerSentence;
    }

    /* [CLASS DOCUMENTATION]
     *
     * Inspector variables: These variables must be set from the inspector.
     * Private variables: These variables change in runtime.
     * 
     * [Class flow]
     * The entry point of this script is the OnTriggerEnter method that starts the dialogue sequence.
     * 
     * [Must know]
     * 1. The player gets deactivated when the sequence starts.
     */
    public class IntroSceneSequencer : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("Set in inspector")]
        [SerializeField] ButtonPrompt dialoguePrompt;
        [SerializeField] List<ScriptedSequence> sequence;
        [SerializeField] Transform punchPosition;

        [Header("Baseball Bat pickup")]
        [SerializeField] ButtonPrompt pickUpPrompt;
        [SerializeField] Transform pickUpPos;
        [SerializeField] GameObject baseballBatPrefab;

        [Header("Player weapon throw sequence trigger")]
        [SerializeField] GameObject throwSequenceStart;
        #endregion

        #region PRIVATE_VARIABLES
        DialogueController dialogueController;
        bool activated;
        bool isInRange;
        bool canContinue;
        bool interpolate;
        #endregion

        private void Start()
        {
            dialogueController = FindObjectOfType<DialogueController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !activated)
            {
                isInRange = true;
            }
        }

        private void Update()
        {
            if (interpolate)
            {
                GameManager.S.PlayerEntity.SetPosition(Vector3.Lerp(GameManager.S.PlayerEntity.transform.position, punchPosition.position, 1f * Time.deltaTime));
                GameManager.S.PlayerEntity.transform.rotation = Quaternion.Euler(Vector3.forward);
            }

            if (Input.GetKeyDown(KeyCode.E) && isInRange && !activated)
            {
                interpolate = true;

                dialoguePrompt.DisableInteraction();

                //Sequence 1
                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.PlayerEntity.PlayerAnimations.SetWalkAnimationState(false);

                StartCoroutine(dialogueController.TypeSentenceSequences(RoutineCallback, 0.01f, sequence[0].sentences, sequence[0].imagePerSentence));
                activated = true;
            }
        }

        void RoutineCallback()
        {
            StartCoroutine(SecondSequence());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInRange = false;
            }
        }

        #region SEQUENCES_HANDLING
        /// <summary>
        /// Call to set canContinue to true.
        /// </summary>
        void EnableSequenceContinueation()
        {
            canContinue = true;
        }

        /// <summary>
        /// Call to initiate the second intro scene sequence.
        /// <para>Disables the player movement.</para>
        /// </summary>
        /// <returns></returns>
        IEnumerator SecondSequence()
        {
            canContinue = false; //Reset canContinue so we wait for input.
            StartCoroutine(dialogueController.TypeSentence("Press L.Click to motivate.", EnableSequenceContinueation, 0.05f, false, true));

            //Set up the player properly.
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.transform.rotation = punchPosition.rotation;
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.PlayerEntity.PlayerShooting.IsAttackActive = true;
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 1f;
            }
            else Utils.MissingComponent("GameManager", this);

            while (!Input.GetKeyDown(KeyCode.Mouse0) || !canContinue)
            {
                yield return null;
            }

            //Shake the camera
            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
            FindObjectOfType<CameraBehaviour>().StartShakeSequence(1f, 5f);

            //Sequence 2
            canContinue = false; //Reset canContinue so we wait for input.
            GameManager.S.PlayerEntity.PlayerShooting.IsAttackActive = false;
            StartCoroutine(dialogueController.TypeSentence("- I ain’t telling you jack shit! Boss is gonna kill me.", EnableSequenceContinueation, 0.01f, false));

            while (!Input.GetKeyDown(KeyCode.E) || !canContinue)
            {
                yield return null;
            }

            //Spawn a bat prefab.
            interpolate = false;

            Instantiate(baseballBatPrefab, pickUpPos);
            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.WeaponPickUp);
            GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);
            GameManager.S.PlayerEntity.IsActive = true;

            //End of sequence
            RoomSequenceEnd();
        }

        /// <summary>
        /// Called from the end of the coroutine when the sequence finishes to enable player movement.
        /// </summary>
        void RoomSequenceEnd()
        {

            //Enable next sequence triggers.
            throwSequenceStart.SetActive(true);
            pickUpPrompt.gameObject.SetActive(true);
        }
        #endregion
    }
}