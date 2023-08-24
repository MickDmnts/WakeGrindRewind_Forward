using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be set from the inspector
     * Private variable: These variables change in runtime.
     * 
     * [Class Flow]
     * 1. The entry point of this class is the OnTriggerEnter method that fires off the dialogue sequence.
     * 
     * [Must know]
     * 1. This script depends on the DialogueController class being present in the scene.
     */
    public class ThrowWeaponSequence : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] GameObject throwWeaponButtonPrompt;
        [SerializeField] GameObject deathSequenceActivator;
        [SerializeField] List<ScriptedSequence> sequence;

        #region PRIVATE_VARIABLES
        DialogueController dialogueController;
        ButtonPrompt weaponPickUpPrompt;

        bool canContinue;

        bool sequenceActivated;
        bool interpolate;
        #endregion

        private void Start()
        {
            dialogueController = FindObjectOfType<DialogueController>();
            weaponPickUpPrompt = GetComponentInChildren<ButtonPrompt>();

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onWeaponPickup += StartThrowSequence;
            }
        }

        private void Update()
        {
            //Move the player to throw position.
            if (interpolate)
            {
                if (GameManager.S != null)
                    GameManager.S.PlayerEntity.SetPosition(Vector3.Lerp(GameManager.S.PlayerEntity.GetPosition(), transform.position, 1f * Time.deltaTime));
            }
        }

        void StartThrowSequence()
        {
            if (sequenceActivated) return;

            //Activate the inspector button prompt.
            throwWeaponButtonPrompt.SetActive(true);

            //Disable Player behaviour and set his rotation
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.PlayerEntity.PlayerAnimations.SetWalkAnimationState(false);
                GameManager.S.PlayerEntity.transform.rotation = Quaternion.Euler(new Vector3(0f, 45f, 0f));
            }

            //Sets activated to true so this sequence gets activated only one
            interpolate = true;
            sequenceActivated = true;

            StartCoroutine(ThrowSequence());
        }

        /// <summary>
        /// Call to set canContinue to true.
        /// </summary>
        void EnableSequenceContinueation()
        {
            canContinue = true;
        }

        /// <summary>
        /// Call to start the throw weapon sequence which demonstrates the weapon throwing mechaninc of the game.
        /// <para>The player is deactivated and prompted only with buttons.</para>
        /// </summary>
        IEnumerator ThrowSequence()
        {
            weaponPickUpPrompt.gameObject.SetActive(false);

            //Throw weapon sequence start - enables player attacking ONLY
            if (GameManager.S != null)
                GameManager.S.PlayerEntity.PlayerShooting.IsAttackActive = true;

            while (!Input.GetKeyDown(KeyCode.Mouse1))
            {
                yield return null;
            }

            //Deactivate the button prompt
            throwWeaponButtonPrompt.SetActive(false);

            if (GameManager.S != null)
            {
                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 1f;
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.DoorKick);
            }

            FindObjectOfType<CameraBehaviour>().StartShakeSequence(1f, 5f);

            //Display the inpsector sentences
            canContinue = false;

            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.PlayerShooting.IsAttackActive = false;
            }

            StartCoroutine(dialogueController.TypeSentenceSequences(EnableSequenceContinueation, 0.01f, sequence[0].sentences, sequence[0].imagePerSentence));

            while (!canContinue)
            {
                yield return null;
            }

            //Sequence ending.
            SequenceEnd();
        }

        /// <summary>
        /// Call to re-enable the player controller and disable the dialogue box.
        /// </summary>
        void SequenceEnd()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);
                GameManager.S.PlayerEntity.IsActive = true;
            }

            //Activate the next sequence
            deathSequenceActivator.SetActive(true);

            //stops the interpolation
            interpolate = false;
        }
    }
}