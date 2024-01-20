using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WGRF.Core;

namespace WGRF.UI
{
    /// <summary>
    /// The available sound setting fields
    /// </summary>
    public enum SoundSetting
    {
        Master = 0,
        OST = 1,
        SFX = 2,
    }

    public class SliderController : MonoBehaviour, IUIController
    {
        ///<summary>The associated text field that this slider updates</summary>
        [SerializeField, Tooltip("The associated text field that this slider updates")] TextMeshProUGUI associatedText;
        ///<summary>The associated text field default text</summary>
        [SerializeField, Tooltip("The associated text field default text")] string text;
        ///<summary>The sound setting this slider corresponds to</summary>
        [SerializeField, Tooltip("The sound setting this slider corresponds to")] SoundSetting soundSetting;

        ///<summary>The attached slider</summary>
        Slider slider;

        ///<summary>Returns the slider current value</summary>
        public int Value => (int)slider.value;

        void Awake()
        {
            slider = GetComponent<Slider>();
        }

        void Start()
        {
            int val = GetSoundValue(soundSetting);
            OnValueChanged();
            slider.value = val;
        }

        public void OnValueChanged()
        {
            string temp = text.Replace("{vl}", Value.ToString());
            associatedText.text = temp;
        }

        /// <summary>
        /// Returns the serialized sound value from the game settings cache
        /// </summary>
        int GetSoundValue(SoundSetting soundSetting)
        {
            int val = -1;

            switch (soundSetting)
            {
                case SoundSetting.Master:
                    val = (int)ManagerHub.S.SettingsHandler.UserSettings.masterVolume;
                    break;
                case SoundSetting.OST:
                    val = (int)ManagerHub.S.SettingsHandler.UserSettings.ostVolume;
                    break;
                case SoundSetting.SFX:
                    val = (int)ManagerHub.S.SettingsHandler.UserSettings.sfxVolume;
                    break;
            }

            return val;
        }

        ///<summary>Writes the sound configurations into the user settings cache</summary>
        public void WriteToSettings()
        {
            UserSettings package = ManagerHub.S.SettingsHandler.UserSettings;
            package.masterVolume = soundSetting == SoundSetting.Master ? Value : package.masterVolume;
            package.ostVolume = soundSetting == SoundSetting.OST ? Value : package.ostVolume;
            package.sfxVolume = soundSetting == SoundSetting.SFX ? Value : package.sfxVolume;

            ManagerHub.S.SettingsHandler.SetUserSettings(package);
        }
    }
}