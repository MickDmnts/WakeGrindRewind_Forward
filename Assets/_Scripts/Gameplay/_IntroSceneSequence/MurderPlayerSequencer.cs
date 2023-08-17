using System.Collections;
using UnityEngine;
using WGR.Gameplay.BattleSystem;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     *
     * Inspector variables: These variables must be set from the inspector.
     * Private variables: These variables change in runtime.
     * 
     * [Class flow]
     * The entry point of this script is the OnTriggerEnter method that starts the player death sequence.
     * The whole point of this script is to prompt the user to press R to demonstrate the slow down ability.
     * 
     * [Must know]
     * 1. The player gets deactivated when the sequence starts and moved to a specific position + rotation.
     */
    public class MurderPlayerSequencer : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] Transform deathPosition;
        [SerializeField] GameObject firePoint;

        //Private variables
        bool abilitiesEnabled;
        bool interpolate;

        Animator dummyAnimator;

        private void Start()
        {
            dummyAnimator = GetComponentInChildren<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                interpolate = true; //Starts the player position interpolation

                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
                }

                StartCoroutine(PlayerDeathSequence());
            }
        }

        private void Update()
        {
            //Handles player interpolation
            if (interpolate)
            {
                if (GameManager.S != null)
                {
                    GameManager.S.PlayerEntity.SetPosition(Vector3.Lerp(GameManager.S.PlayerEntity.transform.position, deathPosition.position, 1f * Time.deltaTime));
                }
            }
        }

        IEnumerator PlayerDeathSequence()
        {
            //Deactivate the player
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 0f;
            }

            //Enable the ability activation for the user.
            if (!abilitiesEnabled)
            {
                if (GameManager.S != null)
                {
                    GameManager.S.AbilityManager.EnableAbilities();
                    GameManager.S.AbilityManager.AbilitiesCanActivate = true;
                    GameManager.S.UIManager.EnablePanel(UIPanel.PlayerInfo);
                    GameManager.S.PlayerEntity.PlayerAnimations.SetWalkAnimationState(false);
                }
                abilitiesEnabled = true;
            }

            while (!Input.GetKeyDown(KeyCode.R))
            {
                yield return null;
            }

            dummyAnimator.Play("enemy_DummyShot");

            //Start the bullet firing coroutine
            StartCoroutine(FirePellets());

            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(UIPanel.DialogueBox);
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 1f;
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Shotgun);
            }

            interpolate = false;
        }

        /// <summary>
        /// Call to fire 6 Bullets marked as enemy bullets (from bullet pool) from the firepoint positions.
        /// </summary>
        IEnumerator FirePellets()
        {
            GameObject[] pellets = new GameObject[6];
            for (int i = 0; i < 6; i++)
            {
                pellets[i] = GameManager.S.BulletPool.GetPooledBulletByType(BulletType.Enemy);
            }

            foreach (GameObject pellet in pellets)
            {
                float randomFloat = Random.Range(-7f, 7f);

                if (pellet != null)
                {
                    Quaternion bulletRotation = Quaternion.Euler(0f, randomFloat, 0);

                    pellet.transform.position = firePoint.transform.position;
                    pellet.transform.rotation = firePoint.transform.rotation * bulletRotation;
                    pellet.SetActive(true);

                    pellet.GetComponent<Bullet>().ChangeSpeed(1.8f);
                }
            }

            yield return null;
        }
    }
}