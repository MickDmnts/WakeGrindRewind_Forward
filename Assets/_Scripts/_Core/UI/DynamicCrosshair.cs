using UnityEngine;
using WGR.Core.Managers;

namespace WGR.UI
{/* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be asssigned from the inspector.
     * Private Variables: These variables change in runtime.
     * 
     * [Class flow]
     * The only responsibility of this script is to move the crosshair mouse panel to 
     * the in-game equivalent of the user's mouse position.
     */
    public class DynamicCrosshair : MonoBehaviour
    {
        #region INSPECOTR_VARIABLES
        [Header("Set in inspector")]
        [SerializeField] RectTransform mousePanel;
        [SerializeField] float minSize;
        [SerializeField] float maxSize;
        #endregion

        #region PRIVATE_VARIABLES
        Camera uiCamera;
        float currentSize;
        bool playerShooting = false;
        #endregion

        private void Start()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to set up the script for correct entry behaviour.
        /// </summary>
        void EntrySetup()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onPlayerShootStart += StartIncreasing;
                GameManager.S.GameEventHandler.onPlayerShootEnd += StopIncreasing;
            }

            if((uiCamera = GetComponent<Camera>()) == null)
            {
                Utils.MissingComponent("Camera", this);
            }
        }

        /// <summary>
        /// Call to set playerShooting to true.
        /// </summary>
        void StartIncreasing()
        {
            playerShooting = true;
        }

        /// <summary>
        /// Call to set playerShooting to false.
        /// </summary>
        void StopIncreasing()
        {
            playerShooting = false;
        }

        private void Update()
        {
            FollowMousePos();

            //Clamp the mouse size.
            if (playerShooting)
            {
                currentSize = Mathf.Lerp(currentSize, maxSize, Time.deltaTime * 10f);

                mousePanel.sizeDelta = new Vector2(currentSize, currentSize);
            }
            else
            {
                currentSize = Mathf.Lerp(currentSize, minSize, Time.deltaTime * 5f);

                mousePanel.sizeDelta = new Vector2(currentSize, currentSize);
            }
        }

        /// <summary>
        /// Call to set the crosshair panel position to the in-game equivalent
        /// of the user's on-screen mouse.
        /// </summary>
        void FollowMousePos()
        {
            Vector3 mousePos = uiCamera.ScreenToWorldPoint(Input.mousePosition);

            mousePos.Set(mousePos.x, mousePos.y, 0f);

            mousePanel.position = mousePos;
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onPlayerShootStart -= StartIncreasing;
                GameManager.S.GameEventHandler.onPlayerShootEnd -= StopIncreasing;
            }
        }
    }
}