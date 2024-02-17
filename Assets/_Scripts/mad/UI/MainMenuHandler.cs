using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WGRF.Core;

namespace WGRF.UI
{
    public class MainMenuHandler : MonoBehaviour
    {
        ///<summary>Main menu panel</summary>
        [SerializeField, Tooltip("Main menu panel")] GameObject mmPanel;
        ///<summary>Settings menu panel</summary>
        [SerializeField, Tooltip("Settings menu panel")] GameObject settingsPanel;
        ///<summary>Shutdown menu panel</summary>
        [SerializeField, Tooltip("Shutdown menu panel")] GameObject shutdownPanel;
        ///<summary>The start game button</summary>
        [SerializeField, Tooltip("The start game button")] Button startBtn;
        ///<summary>The settings button</summary>
        [SerializeField, Tooltip("The settings button")] Button settingsBtn;
        ///<summary>The close button</summary>
        [SerializeField, Tooltip("The close button")] Button closeBtn;
        ///<summary>The start button text</summary>
        [SerializeField, Tooltip("The start button text")] TextMeshProUGUI startTxt;

        ///<summary>Stores the ui controllers of the main menu</summary>
        IUIController[] uiControllers;

        void Awake()
        { uiControllers = GetComponentsInChildren<IUIController>(); }

        void Start()
        {
            shutdownPanel.SetActive(false);
            mmPanel.SetActive(false);
            settingsPanel.SetActive(false);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Bootup);
        }

        ///<summary>Loads the run scene</summary>
        public void StartGame()
        { StartCoroutine(StartGameRoutine()); }

        IEnumerator StartGameRoutine()
        {
            startBtn.interactable = false;
            settingsBtn.interactable = false;
            closeBtn.interactable = false;
            startTxt.SetText("Loading...");
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.FlipLoad);
            ManagerHub.S.GameSoundsHandler.StopMenu();

            yield return new WaitForSeconds(3f);

            ManagerHub.S.StageHandler.LoadRun();
        }

        ///<summary>Updates and writes the configurated settings from the settings panel</summary>
        public void SaveSettings()
        {
            foreach (IUIController controller in uiControllers)
            { controller.WriteToSettings(); }

            ManagerHub.S.SettingsHandler.UpdateUserSettings(ManagerHub.S.SettingsHandler.UserSettings);
            ManagerHub.S.GameSoundsHandler.SetLoadedSettings(ManagerHub.S.SettingsHandler.UserSettings);
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            { PlayClick(); }
        }

        ///<summary>Toggles the Shutdown panel ON/OFF</summary>
        public void ToggleShutdownPanel()
        { shutdownPanel.SetActive(!shutdownPanel.activeSelf); }

        ///<summary>Toggles the MM panel ON/OFF</summary>
        public void ToggleMMPanel()
        {
            mmPanel.SetActive(!mmPanel.activeSelf);

            if (mmPanel.activeSelf)
            { ManagerHub.S.GameSoundsHandler.PlayMenu(); }
            else
            { ManagerHub.S.GameSoundsHandler.StopMenu(); }
        }

        ///<summary>Plays the mouse click sound</summary>
        public void PlayClick()
        { ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Click); }

        ///<summary>Toggles the settings panel ON/OFF</summary>
        public void ToggleSettingsPanel()
        { settingsPanel.SetActive(!settingsPanel.activeSelf); }

        ///<summary>Quits the application</summary>
        public void ExitGame()
        { Application.Quit(); }
    }
}