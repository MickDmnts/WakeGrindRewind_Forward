using System.Collections;
using System.Collections.Generic;
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
     * 1. This script is enabled when the required number of enemies and the player enters the trigger box before the elevator.
     * 2. Boss fight starts through the BossEntity object.
     * 3. Boss is moved through the EnemySpawnHandler.
     */
    public class BossEntrySequence : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] GameObject bossCamera;
        [SerializeField] Sprite bossIcon;

        #region PRIVATE_VARIABLES
        BossEntity bossEntity;
        DialogueController dialogueController;
        EnemySpawnHandler bossMinionHandler;

        bool isTriggered;
        bool interpolate;
        bool canContinue;
        #endregion

        private void Start()
        {
            CacheHandlers();
        }

        /// <summary>
        /// Call to cache the needed handlers for sequence continuetion.
        /// </summary>
        void CacheHandlers()
        {
            dialogueController = FindObjectOfType<DialogueController>();
            bossMinionHandler = FindObjectOfType<EnemySpawnHandler>();
            bossEntity = FindObjectOfType<BossEntity>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isTriggered) return;

            if (other.CompareTag("Player"))
            {
                isTriggered = true;

                interpolate = true;

                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.IsActive = false;
                    GameManager.S.PlayerEntity.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                }

                bossCamera.SetActive(true);

                //Moves the boss in front of the elevator.
                bossMinionHandler.SpawnBoss();

                //Starts the boss dialogue.
                StartCoroutine(StartBossDialogue());
            }
        }

        private void Update()
        {
            if (interpolate)
            {
                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.SetPosition(Vector3.Lerp(GameManager.S.PlayerEntity.GetPosition(), transform.position, 0.5f));

                    if (Vector3.Distance(GameManager.S.PlayerEntity.GetPosition(), transform.position) <= 0.013)
                    {
                        interpolate = false;
                    }
                }
            }
        }

        /// <summary>
        /// Call to wait for 2 seconds and call the BossDialogue coroutine.
        /// </summary>
        IEnumerator StartBossDialogue()
        {
            yield return new WaitForSeconds(2f);

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
        /// Call to start the boss entrance sequence.
        /// <para>Enables the dialogue box and displays X dialogue.</para>
        /// <para>Starts the boss attacking behaviour through bossEntity.EnableBossEntryAttacking()</para>
        /// </summary>
        IEnumerator BossDialogue()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.EnablePanel(UIPanel.DialogueBox);
            }

            StartCoroutine(dialogueController.TypeSentenceSequences(EnableContinuation, 0.05f, new List<string> { "Here at last..." }, new List<Sprite> { bossIcon }));

            while (!canContinue)
            {
                yield return null;
            }
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);
            }

            StartBossFight();

            yield return new WaitForSeconds(2f);

            //Starts the boss fight.
            bossEntity.EnableBossEntryAttacking();
        }

        /// <summary>
        /// Call to start playing the boss music and activate the player.
        /// </summary>
        void StartBossFight()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameSoundsHandler.ForcePlayBossMusic();

                GameManager.S.PlayerEntity.IsActive = true;
            }

            //Deactivate the boss top view camera.
            bossCamera.SetActive(false);
        }
    }
}