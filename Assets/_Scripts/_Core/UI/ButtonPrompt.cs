using UnityEngine;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be asssigned from the inspector.
     * Private Variables: These variables change in runtime.
     * 
     * [Class flow]
     * The entry point of this script is OnTriggerEnter that gets called when the player enters the 
     *      gameobject's trigger area.
     */
    public class ButtonPrompt : MonoBehaviour
    {
        /// <summary>
        /// Every layer the canvas can exist on.
        /// </summary>
        enum CanvasLayers
        {
            NonVisible = 28,
            Visible = 29,
        }

        #region INSPECTOR_VARIABLES
        [Header("Set in inspector")]
        [SerializeField] bool alwaysInteractable;
        [SerializeField] bool dontDeactivateOnExit;
        [SerializeField] bool visibleOnStart;
        #endregion

        #region PRIVATE_VARIABLES
        SpriteRenderer buttonSprite;
        bool interacted;
        #endregion  

        void Start()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to correctly set up the button behaviour and cache its needed components.
        /// </summary>
        void EntrySetup()
        {
            buttonSprite = transform.GetComponentInChildren<SpriteRenderer>();

            if (visibleOnStart)
            {
                SetLayer(CanvasLayers.Visible);
            }
            else
            {
                SetLayer(CanvasLayers.NonVisible);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //Continue only if the collided gameobject is the player.
            if (other.CompareTag("Player"))
            {
                //Early exit if the canvas is not alwaysInteractable or its already interacted.
                if (!alwaysInteractable && interacted) return;

                SetLayer(CanvasLayers.Visible);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //Early exit if dontDeactivateOnExit is enabled from the inspector.
            if (dontDeactivateOnExit) return;

            if (other.CompareTag("Player"))
            {
                SetLayer(CanvasLayers.NonVisible);
            }
        }

        /// <summary>
        /// Call to set the canvas layer to the passed layer.
        /// </summary>
        /// <param name="layer"></param>
        void SetLayer(CanvasLayers layer)
        {
            buttonSprite.transform.root.gameObject.layer = (int)layer;
            buttonSprite.transform.gameObject.layer = (int)layer;
        }

        /// <summary>
        /// Cal to make the canvas non-interactable and invisible.
        /// </summary>
        public void DisableInteraction()
        {
            interacted = true;
            SetLayer(CanvasLayers.NonVisible);
        }
    }
}