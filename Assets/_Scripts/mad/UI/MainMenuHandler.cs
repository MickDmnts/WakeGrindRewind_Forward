using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WGRF.Core;

namespace WGRF.UI
{
    /// <summary>
    /// Teh base class handling the main menu
    /// </summary>
    public class MainMenuHandler : MonoBehaviour
    {
        ///<summary>WGRF: Main menu panel</summary>
        [SerializeField, Tooltip("Main menu panel")] GameObject wgrfMmPanel;
        ///<summary>WGR Main menu panel</summary>
        [SerializeField, Tooltip("Main menu panel")] GameObject wgrMmPanel;
        ///<summary>Settings menu panel</summary>
        [SerializeField, Tooltip("Settings menu panel")] GameObject settingsPanel;
        ///<summary>Shutdown menu panel</summary>
        [SerializeField, Tooltip("Shutdown menu panel")] GameObject shutdownPanel;
        ///<summary>The start game button</summary>
        [SerializeField, Tooltip("The start game button")] Button startBtn;
        ///<summary>The WGR start game button</summary>
        [SerializeField, Tooltip("The WGR start game button")] Button wgrStartBtn;
        ///<summary>The settings button</summary>
        [SerializeField, Tooltip("The settings button")] Button settingsBtn;
        ///<summary>The close button</summary>
        [SerializeField, Tooltip("The close button")] Button closeBtn;
        ///<summary>The WGR close button</summary>
        [SerializeField, Tooltip("The WGR close button")] Button wgrCloseBtn;
        ///<summary>The start button text</summary>
        [SerializeField, Tooltip("The start button text")] TextMeshProUGUI startTxt;
        ///<summary>The WGR start button text</summary>
        [SerializeField, Tooltip("The WGR start button text")] TextMeshProUGUI wgrStartTxt;

        ///<summary>Stores the ui controllers of the main menu</summary>
        IUIController[] uiControllers;

        void Start()
        {
            shutdownPanel.SetActive(false);
            wgrfMmPanel.SetActive(false);
            wgrMmPanel.SetActive(false);
            settingsPanel.SetActive(false);
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Bootup);
            ManagerHub.S.GameSoundsHandler.StopMenu();
        }

        ///<summary>Loads the WGRF run scene</summary>
        public void StartWGRF()
        { StartCoroutine(StartGameRoutine()); }

        ///<summary>Starst the game after a small placebo interval</summary>
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

        ///<summary>Loads the WGR game</summary>
        public void StartWGR()
        { StartCoroutine(StartWGRApp()); }

        IEnumerator StartWGRApp()
        {
            wgrStartBtn.interactable = false;
            wgrCloseBtn.interactable = false;
            wgrStartTxt.SetText("Loading...");
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.FlipLoad);
            ManagerHub.S.GameSoundsHandler.StopMenu();

            yield return new WaitForSeconds(3f);

            Process.Start(ManagerHub.S.Globals.LegacyExecutable);

            ExitGame();
        }

        ///<summary>Updates and writes the configurated settings from the settings panel</summary>
        public void SaveSettings()
        {
            uiControllers = GetComponentsInChildren<IUIController>();
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
        {
            shutdownPanel.SetActive(!shutdownPanel.activeSelf);
            if (!shutdownPanel.activeSelf)
            { ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor); }
        }

        ///<summary>Toggles the MM panel ON/OFF</summary>
        public void ToggleWGRFMMPanel()
        {
            wgrfMmPanel.SetActive(!wgrfMmPanel.activeSelf);

            if (wgrfMmPanel.activeSelf)
            { ManagerHub.S.GameSoundsHandler.PlayMenu(); }
            else
            {
                ManagerHub.S.GameSoundsHandler.StopMenu();
                ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
            }
        }

        ///<summary>Toggles the MM panel ON/OFF</summary>
        public void ToggleWGRMMPanel()
        {
            wgrMmPanel.SetActive(!wgrMmPanel.activeSelf);

            if (wgrMmPanel.activeSelf)
            { ManagerHub.S.GameSoundsHandler.PlayMenu(); }
            else
            {
                ManagerHub.S.GameSoundsHandler.StopMenu();
                ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor);
            }
        }

        ///<summary>Plays the mouse click sound</summary>
        public void PlayClick()
        { ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Click); }

        ///<summary>Toggles the settings panel ON/OFF</summary>
        public void ToggleSettingsPanel()
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            if (!settingsPanel.activeSelf)
            { ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor); }
        }

        ///<summary>Quits the application</summary>
        public void ExitGame()
        { Application.Quit(); }
    }
}