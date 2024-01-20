using UnityEngine;
using UnityEngine.UI;
using WGRF.Core;

namespace WGRF.UI
{
    public enum ToggleSetting
    {
        Gore = 0,
    }

    /// <summary>
    /// Simple toggle controller for background change.
    /// </summary>
    public class ToggleController : MonoBehaviour, IUIController
    {
        ///<summary>The toggle setting of this toggler</summary>
        [SerializeField, Tooltip("The toggle setting of this toggler")] ToggleSetting toggleSetting;
        ///<summary>Toggle on sprite</summary>
        [SerializeField, Tooltip("Toggle on sprite")] Sprite toggleOn;
        ///<summary>Toggle off sprite</summary>
        [SerializeField, Tooltip("Toggle off sprite")] Sprite toggleOff;

        ///<summary>The toggle this controller is attached to</summary>
        Toggle toggler;
        ///<summary>The toggle background</summary>
        Image bg;

        ///<summary>Returns the active state of this toggle</summary>
        public bool IsOn => toggler.isOn;

        void Start()
        {
            toggler = GetComponent<Toggle>();
            bg = transform.GetChild(0).GetComponent<Image>();

            toggler.isOn = GetToggleState(toggleSetting);

            SetToggleState();
        }

        ///<summary>Sets the toggle background sprite based on the toggle isOn state.</summary>
        public void SetToggleState()
        { bg.sprite = toggler.isOn ? toggleOn : toggleOff; }

        bool GetToggleState(ToggleSetting toggleSetting)
        {
            bool state = false;

            switch (toggleSetting)
            {
                case ToggleSetting.Gore:
                    state = ManagerHub.S.SettingsHandler.UserSettings.goreVFX;
                    break;
            }

            return state;
        }

        ///<summary>Updates the cached toggle setting values in the cached user settings</summary>
        public void WriteToSettings()
        {
            UserSettings package = ManagerHub.S.SettingsHandler.UserSettings;
            switch (toggleSetting)
            {
                case ToggleSetting.Gore:
                    package.goreVFX = IsOn;
                    break;
            }

            ManagerHub.S.SettingsHandler.SetUserSettings(package);
        }
    }
}