using UnityEngine;
using WGRF.Core;

namespace WGRF.UI
{
    public class MainMenuHandler : MonoBehaviour
    {
        ///<summary>Main menu panel</summary>
        [SerializeField, Tooltip("Main menu panel")] GameObject mmPanel;
        ///<summary>Settings menu panel</summary>
        [SerializeField, Tooltip("Settings menu panel")] GameObject settingsPanel;

        ///<summary>Stores the ui controllers of the main menu</summary>
        IUIController[] uiControllers;

        void Awake()
        { uiControllers = GetComponentsInChildren<IUIController>(); }

        void Start()
        {
            mmPanel.SetActive(true);
            settingsPanel.SetActive(false);
            ManagerHub.S.GameSoundsHandler.PlayMenu();
        }

        ///<summary>Loads the run scene</summary>
        public void StartGame()
        { ManagerHub.S.StageHandler.LoadRun(); }

        ///<summary>Updates and writes the configurated settings from the settings panel</summary>
        public void SaveSettings()
        {
            foreach (IUIController controller in uiControllers)
            { controller.WriteToSettings(); }

            ManagerHub.S.SettingsHandler.UpdateUserSettings(ManagerHub.S.SettingsHandler.UserSettings);
            ManagerHub.S.GameSoundsHandler.SetLoadedSettings(ManagerHub.S.SettingsHandler.UserSettings);
        }

        ///<summary>Toggles the settings panel ON/OFF</summary>
        public void ToggleSettingsPanel()
        { settingsPanel.SetActive(!settingsPanel.activeSelf); }

        ///<summary>Quits the application</summary>
        public void ExitGame()
        { Application.Quit(); }
    }
}