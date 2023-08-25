using System;
using System.Collections.Generic;
using UnityEngine;
using WGR.Core.Managers;
using WGR.UI;

namespace WGR.Scripted
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
    public class DialogueOnTriggerEnter : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("Set in inspector")]
        [SerializeField, Range(0, 0.2f)] float typingSpeed; //How fast should the text be displayed
        [Tooltip("Everything that's inside this Text Box will be displayed " +
            "letter by letter in the canvas infoBox")]
        [SerializeField, TextArea] string sentenceToType;

        [Header("Enable the below checkbox to have multiple sentences typed.")]
        [SerializeField] bool multiSentenced;
        [SerializeField] List<string> sentences;
        [SerializeField] List<Sprite> imagePerSentence;
        #endregion

        //Private variable
        bool activated = false;
        DialogueController dialogueController;

        private void Start()
        {
            dialogueController = FindObjectOfType<DialogueController>();
        }

        public void OnTriggerEnter(Collider collision)
        {
            //Activate only if the trigger detects the player prefab
            if (collision.CompareTag("Player") && !activated)
            {
                activated = true;

                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);

                if (!multiSentenced)
                {
                    IsSingleSentenced();
                }
                else
                {
                    IsMultiSentenced();
                }

                //Deactivate the player when we enter the interaction state
                GameManager.S.PlayerEntity.IsActive = false;
            }
        }

        /// <summary>
        /// Call to initiate a single sentenced dialogue sequence.
        /// </summary>
        void IsSingleSentenced()
        {
            StartCoroutine(dialogueController.TypeSentence(sentenceToType, RoutineCallback, typingSpeed, true));
        }

        /// <summary>
        /// Call to initiate a multi sentenced dialogue sequence.
        /// </summary>
        void IsMultiSentenced()
        {
            StartCoroutine(dialogueController.TypeSentenceSequences(RoutineCallback, typingSpeed, sentences, imagePerSentence));
        }

        /// <summary>
        /// Call to enable the players ability to exit the UI interaction
        /// and stop the fast forwarding ability.
        /// <para>Coroutine use ONLY, passed as an action callback</para>
        /// </summary>
        void RoutineCallback()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);

                GameManager.S.PlayerEntity.IsActive = true;
            }
            else Utils.MissingComponent("GameManager", this);
        }

        private void OnValidate()
        {
            if (multiSentenced)
            {
                sentenceToType = string.Empty;
            }
        }
    }
}