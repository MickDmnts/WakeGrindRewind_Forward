using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WGR.Core;
using WGR.Core.Managers;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * All the script variables are public and must be set from the inspector.
     * 
     * [Must Know]
     * 1. The purpose of this script is to pass the UI panels from the UI_Render scene to the GameEntry scene 
     * to update the UIManager with the new panel references present in the UI_Render scene.
     * 2. The LevelManager Fader script reference is also set from this script.
     */
    public class PanelReferences : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] List<GameObject> uiPanels;
        [SerializeField] SceneFader fader;
        [SerializeField] Button continueFromPauseButton;
        [SerializeField] Button quitFromPauseButton;

        private void Awake()
        {
            //Called in Awake!
            PassPanelReferences();
        }

        /// <summary>
        /// Call to pass the assigned panel references, the Continue and Quit buttons to the UIManager
        /// and the Fader script reference to the LevelManager.
        /// </summary>
        void PassPanelReferences()
        {
            GameManager.S.UIManager.SetUIPanels(uiPanels);

            GameManager.S.UIManager.SetContinueFromPauseScreenButton(continueFromPauseButton);
            GameManager.S.UIManager.SetQuitFromPauseScreenButton(quitFromPauseButton);

            GameManager.S.LevelManager.SetFaderReference(fader);
        }

        private void Start()
        {
            DeactivatePanels();
        }

        /// <summary>
        /// Call to deactivate all the ui panels in the beginning of the game.
        /// </summary>
        void DeactivatePanels()
        {
            for (int i = 1; i < uiPanels.Count; i++)
            {
                uiPanels[i].SetActive(false);
            }
        }
    }
}