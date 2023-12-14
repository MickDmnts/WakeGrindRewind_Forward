using UnityEngine;

namespace WGRF.Core
{
    public enum GameAudioClip
    {
        //@TODO:
    }

    /// <summary>
    /// This class is responsible for all of WGRF game sounds.
    /// </summary>
    [DefaultExecutionOrder(0)]
    public class GameSoundsHandler : CoreBehaviour
    {
        ///<summary>The master audio source value</summary>
        [Header("Audio sources settings")]
        [SerializeField, Tooltip("The master audio value")] float masterAudioValue;
        ///<summary>The soundtrack audio source</summary>
        [SerializeField, Tooltip("The soundtrack audio source")] AudioSource ostSource;
        ///<summary>The SFX audio source</summary>
        [SerializeField, Tooltip("The SFX audio source")] AudioSource sfxSource;

        ///<summary>An array containing all of the game wide audio clips</summary>
        [Header("Audio clips")]
        [SerializeField, Tooltip("A list containing all of the game wide audio clips")]
        AudioClip[] levelWideClips;

        ///<summary>An array containing all of the SFX clips of the game</summary>
        [SerializeField, Tooltip("An array containing all of the SFX clips of the game")]
        AudioClip[] sfxClips;

        protected override void PreAwake()
        {
            ConfigureAudioSources(ref ostSource, ref sfxSource);
        }

        private void Start()
        {
            SetLoadedSettings(ManagerHub.S.SettingsHandler.UserSettings);
        }

        /// <summary>
        /// Properly configures the OST and SFX audio sources of the game.
        /// </summary>
        /// <exception cref="System.NullReferenceException">If any of the two audio sources is null.</exception>
        void ConfigureAudioSources(ref AudioSource ost, ref AudioSource sfx)
        {
            if (ost == null)
            { throw new System.NullReferenceException("No OST audio source assigned."); }

            if (sfx == null)
            { throw new System.NullReferenceException("No SFX audio source assigned."); }

            ost.loop = true;
            sfx.loop = false;

            ost.priority = 127;
            sfx.priority = 255;
        }

        /// <summary>
        /// Updates the audio source volumes from the passed loaded settings package.
        /// </summary>
        /// <param name="loadedSettings"></param>
        void SetLoadedSettings(UserSettings loadedSettings)
        {
            masterAudioValue = loadedSettings.masterVolume;

            ForceOSTVolume(loadedSettings.ostVolume);
            ForceSFXVolume(loadedSettings.sfxVolume);
        }

        /// <summary>
        /// Sets the volume of the passed source to the passed value.
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="source">The source volume to modify</param>
        /// <param name="value">The new volume value. New value must be [0-10]</param>
        void SetVolume(ref AudioSource source, float value)
        {
            if (value < 0f || value > 10f)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Audio source value must be between 0 and 10.\nThe value will get capped at 0 or 10.");
#endif
            }

            //Cap at master volume value...
            if (value > masterAudioValue)
            { value = masterAudioValue; }
            else if (value < 0f) //Cap at 0...
            { value = 0f; }

            source.volume = value / 10;
        }

        #region AUDIO SOURCE VOLUME MODIFYING
        /// <summary>
        /// Forcefully sets the volume of the OST audio source to the passed new volume value.
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="newVolume">The new volume of the OST audio source.</param>
        public void ForceOSTVolume(float newVolume)
        {
            SetVolume(ref ostSource, newVolume);
        }

        /// <summary>
        /// Increases the OST audio source volume by the value passed.
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="value">The value to increase the audio source volume by.</param>
        public void IncreaseOSTVolumeBy(float value)
        {
            SetVolume(ref ostSource, ostSource.volume + value);
        }

        /// <summary>
        /// Decreases the OST audio source volume by the value passed
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="value">Teh value to decrease the audio source volume by.</param>
        public void DecreaseOSTVolumeBy(float value)
        {
            SetVolume(ref ostSource, ostSource.volume - value);
        }

        /// <summary>
        /// Forcefully sets the volume of the SFX audio source to the passed new volume value.
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="newVolume">The new volume of the SFX audio source.</param>
        public void ForceSFXVolume(float newVolume)
        {
            SetVolume(ref sfxSource, newVolume);
        }

        /// <summary>
        /// Increases the SFX audio source volume by the value passed
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="value">The value to increase the audio source volume by.</param>
        public void IncreaseSFXVolumeBy(float value)
        {
            SetVolume(ref sfxSource, sfxSource.volume + value);
        }

        /// <summary>
        /// Decreases the SFX audio source volume by the value passed
        /// The value gets capped at the current game wide master volume.
        /// </summary>
        /// <param name="value">The value to decrease the audio source volume by.</param>
        public void DecreaseSFXVolumeBy(float value)
        {
            SetVolume(ref sfxSource, sfxSource.volume - value);
        }

        /// <summary>
        /// Plays the requested SFX as an one shot
        /// </summary>
        /// <param name="sound">The SFX to play</param>
        public void PlayOneShotSFX(GameAudioClip sound)
        {
            sfxSource.PlayOneShot(sfxClips[(int)sound]);
        }
        #endregion
    }
}