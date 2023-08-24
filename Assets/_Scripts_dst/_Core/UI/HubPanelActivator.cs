using UnityEngine;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be asssigned from the inspector.
     * Private Variables: These variables change in runtime.
     * 
     * [Must know]
     * 1. When added as a component the script adds a BoxCollider in the gameObject.
     * 2. This script is used in the PlayerHub and gets triggered when the player gets inside the trigger 
     * volume around the AbilitiesPanel and WeaponsPanel.
     */
    [RequireComponent(typeof(BoxCollider))]
    public class HubPanelActivator : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] UIPanel panel;
        [SerializeField] bool playMusicOnInteraction;
        [SerializeField] GameAudioClip audioClip;

        #region PRIVATE_VARIABLES
        bool canActivate = true;
        bool isInRange = false;
        bool isActivated = false;
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (!canActivate) return;

            isInRange = true;
        }

        private void Update()
        {
            if (isInRange && !isActivated && Input.GetKeyDown(KeyCode.E))
            {
                ActivatePanel();
            }

            if (isActivated && Input.GetKeyDown(KeyCode.Escape))
            {
                DeactivatePanel();
            }
        }

        /// <summary>
        /// Call to enable the assigned from inspector panel.
        /// <para>Disables player controls and sets isActivate to true</para>
        /// </summary>
        void ActivatePanel()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.EnablePanel(panel);

                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.UIManager.PlayerHubPanelActive = true;

                if (playMusicOnInteraction)
                {
                    GameManager.S.GameSoundsHandler.PlayOneShot(audioClip);
                }
            }

            isActivated = true;
        }

        /// <summary>
        /// Call to disable the assigned from inspector panel.
        /// <para>Enables player controls and sets isActivate - isInRange to false</para>
        /// </summary>
        void DeactivatePanel()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.DisablePanel(panel);
                GameManager.S.PlayerEntity.IsActive = true;
                GameManager.S.UIManager.PlayerHubPanelActive = false;
            }

            isActivated = false;
        }

        private void OnTriggerExit(Collider other)
        {
            isInRange = false;
        }

        /// <summary>
        /// Call to set the ability to activate this panel when the user presses the prompted button.
        /// </summary>
        public void SetCanActivate(bool state)
        {
            canActivate = state;
        }
    }
}

