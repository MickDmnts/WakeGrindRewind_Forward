using System;
using System.Collections.Generic;
using UnityEngine;

namespace WGRF.Core
{
    ///<summary>Every available audio sound</summary>
    public enum GameAudioClip
    {
        DoorHit,
        GameplayLoop,
        EmptyGun,
        PickUp,
        Pistol,
        Uzi,
        Shotgun,
        Hit,
        KnifeCut,
        Punch,
        Static,
        VHSClose,
        VHSOpen,
        VHSSlideIn,
        VHSSlideOut,
        Pause,
        Unpause,
        SlowDownTime,
        PressRewind,
        Rewind,
        StopTime,
        Hurt,
        Menu,
        Hub
    }

    [Serializable]
    public struct AudioData
    {
        ///<summary>The audio type</summary>
        public GameAudioClip gameAudioClip;
        ///<summary>The associated audio clips of the type</summary>
        public AudioClip[] clips;
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
        ///<summary>The audio clips per sound type</summary>
        [SerializeField, Tooltip("The audio clips per sound type")] AudioData[] audioData;

        ///<summary>The sound audio clips per sound type pairs</summary>
        Dictionary<int, List<AudioClip>> clipsPerSoundType;

        protected override void PreAwake()
        {
            ConfigureAudioSources(ref ostSource, ref sfxSource);
            GroupAudioClips();
        }

        ///<summary>Fills the clipsPerSoundType dictionary cache with the audioData pairs</summary>
        void GroupAudioClips()
        {
            clipsPerSoundType = new Dictionary<int, List<AudioClip>>();
            for (int i = 0; i < Enum.GetNames(typeof(GameAudioClip)).Length; i++)
            { clipsPerSoundType.Add(i, new List<AudioClip>()); }

            for (int i = 0; i < audioData.Length; i++)
            {
                for (int j = 0; j < audioData[i].clips.Length; j++)
                {
                    clipsPerSoundType[(int)audioData[i].gameAudioClip].Add(audioData[i].clips[j]);
                }
            }
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
        public void SetLoadedSettings(UserSettings loadedSettings)
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
            List<AudioClip> clips = clipsPerSoundType[(int)sound];
            AudioClip audio = clips[0];

            if (clips.Count > 1)
            { audio = clips[UnityEngine.Random.Range(0, clips.Count)]; }

            sfxSource.PlayOneShot(audio);
        }

        ///<summary>Plays the gameplay loop</summary>
        public void PlayLoop()
        {
            ostSource.clip = clipsPerSoundType[(int)GameAudioClip.GameplayLoop][0];
            ostSource.loop = true;
            ostSource.Play();
        }

        ///<summary>Plays the gameplay loop</summary>
        public void PlayMenu()
        {
            ostSource.clip = clipsPerSoundType[(int)GameAudioClip.Menu][0];
            ostSource.loop = true;
            ostSource.Play();
        }

        ///<summary>Plays the gameplay loop</summary>
        public void PlayHubSFX()
        {
            ostSource.clip = clipsPerSoundType[(int)GameAudioClip.Hub][0];
            ostSource.loop = true;
            ostSource.Play();
        }
        #endregion
    }
}