using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WGR.UI;

namespace WGR.Core.Managers
{
    /// <summary>
    /// All the available in-game UI panels that can be activated.
    /// </summary>
    public enum UIPanel
    {
        MainMenuPanel,
        PauseMenu,
        PlayerInfo,

        //Player hub panels
        VisualsPanel,
        AbilitiesPanel,
        StartWeaponPanel,
        AchievementsPanel,

        DialogueBox,
        SaveDeletion,
    }


    /* [CLASS DOCUMENTATION]
     * 
     * [Variable specific]
     * Dynamically changed: Theses values change in runtime.
     * 
     * [Must Know]
     * 1. The uiPanels are set from the UIRender scene when it loads, not the opposite.
     */

    public class UI_Manager : MonoBehaviour
    {
        //Dynamically changed
        List<GameObject> uiPanels;

        Button quitGameFromPause;
        Button continueButton;

        HandleAbilityUpdatePanel abilityButtonsHandle;
        public HandleAbilityUpdatePanel UserUIHandle
        {
            get { return abilityButtonsHandle; }
            set { abilityButtonsHandle = value; }
        }

        bool isPaused = false;
        public bool IsPaused
        {
            get { return isPaused; }
        }

        bool playerHubPanelActive = false;
        public bool PlayerHubPanelActive
        {
            get { return playerHubPanelActive; }
            set { playerHubPanelActive = value; }
        }

        #region AWAKE_CALLED_EXTERNALLY
        /// <summary>
        /// Call to set the uiPanels list to the passed reference.
        /// </summary>
        /// <param name="panels"></param>
        public void SetUIPanels(List<GameObject> panels)
        {
            this.uiPanels = panels;
        }

        /// <summary>
        /// Call to set the quitGameFromPause button to the passed reference.
        /// </summary>
        public void SetQuitFromPauseScreenButton(Button button)
        {
            quitGameFromPause = button;

            SetQuitButtonOnClickEvent();
        }

        void SetQuitButtonOnClickEvent()
        {
            quitGameFromPause.onClick.AddListener(() => QuitGame());
        }

        public void SetContinueFromPauseScreenButton(Button button)
        {
            continueButton = button;

            SetContinueButtonOnClickEvent();
        }

        void SetContinueButtonOnClickEvent()
        {
            continueButton.onClick.AddListener(() => PauseGame());
        }

        #endregion

        private void Start()
        {
            GameManager.S.GameEventHandler.onSceneChanged += SetPauseBehaviourToDefault;
        }

        void SetPauseBehaviourToDefault(GameScenes scene)
        {
            PlayerHubPanelActive = false;
        }

        private void Update()
        {
            //For game pausing
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }

        /// <summary>
        /// Call to activate the Options panel if the user is not in the MainMenu.
        /// Sets isPaused to true if successfully paused or false if unpaused.
        /// </summary>
        void PauseGame()
        {
            if (IsInInvalidScene() || PlayerHubPanelActive) return;

            if (!isPaused)
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Pause);

                EnablePanel(UIPanel.PauseMenu);
                GameManager.S.PlayerEntity.IsActive = false;
                isPaused = true;

                GameManager.S.AIEntityManager.SetAgentBehaviourUpdate(false);
                GameManager.S.GameEventHandler.OnGamePaused();
            }
            else
            {
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Unpause);

                DisablePanel(UIPanel.PauseMenu);
                GameManager.S.PlayerEntity.IsActive = true;
                isPaused = false;

                GameManager.S.AIEntityManager.SetAgentBehaviourUpdate(true);
                GameManager.S.GameEventHandler.OnGameResumed();
            }

            GameManager.S.GameSoundsHandler.SetGameWideSoundtrackState(IsPaused);
        }

        /// <summary>
        /// Call to iterate through the currently active scenes, if one of them is the MainMenu then return true
        /// false otherwise.
        /// </summary>
        bool IsInInvalidScene()
        {
            if (GameManager.S.LevelManager.FocusedScene == GameScenes.MainMenu
                || GameManager.S.LevelManager.FocusedScene == GameScenes.NewGameIntro)
                return true;

            return false;
        }

        /// <summary>
        /// Call to enable the passed UI panel.
        /// </summary>
        public void EnablePanel(UIPanel panel)
        {
            uiPanels[(int)panel].SetActive(true);
        }

        /// <summary>
        /// Call to disable the passed UI panel.
        /// </summary>
        public void DisablePanel(UIPanel panel)
        {
            uiPanels[(int)panel].SetActive(false);
        }

        public GameObject GetInfoTextBoxPanel() => uiPanels[uiPanels.Count - 2];

        void QuitGame()
        {
            Application.Quit();
        }

        private void OnDestroy()
        {
            GameManager.S.GameEventHandler.onSceneChanged -= SetPauseBehaviourToDefault;
        }
    }
}