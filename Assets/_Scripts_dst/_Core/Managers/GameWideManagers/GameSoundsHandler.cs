using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WGR.Core
{
    public enum GameAudioClip
    {
        EmptyGunSound,
        PunchSound,
        ShootSound,
        ElevatorArrival,
        DoorKick,
        WeaponPickUp,
        Hit,
        Hit1,
        Hit2,
        Hit3,
        KnifeCut1,
        KnifeCut2,
        Punch,
        PunchSwing1,
        PunchSwing2,
        WeaponThrow,
        StaticVHS,
        VHSClose,
        VHSOpen,
        VHSSlideIn,
        VHSSlideOut,
        VHSStatic,
        Pause,
        Unpause,
        PressPlay,
        PressRewind,
        PressStop,
        Rewind,
        StopActivate,
        Uzi,
        Shotgun,
        DoorOpen,
    }

    public class GameSoundsHandler : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] List<AudioClip> levelWideClips;
        [SerializeField] List<AudioClip> sfxClips;

        public AudioSource mainAudioSource;
        public AudioSource sfxSource;

        Slider musicSlider;

        private void Start()
        {
            GameManager.S.GameEventHandler.onSceneChanged += ChangeSoundtrack;
        }

        void ChangeSoundtrack(GameScenes scenes)
        {
            switch (scenes)
            {
                case GameScenes.MainMenu:
                    mainAudioSource.clip = levelWideClips[0];
                    mainAudioSource.Play();
                    break;

                case GameScenes.NewGameIntro:
                    mainAudioSource.clip = levelWideClips[1];
                    mainAudioSource.Play();
                    break;

                case GameScenes.PlayerHub:
                    mainAudioSource.clip = levelWideClips[2];
                    mainAudioSource.Play();
                    break;

                case GameScenes.AbilitiesTutorial:
                    break;

                case GameScenes.Level1:
                    mainAudioSource.clip = levelWideClips[3];
                    mainAudioSource.Play();

                    PlayOneShot(GameAudioClip.ElevatorArrival);
                    break;

                case GameScenes.Level2:
                    mainAudioSource.clip = levelWideClips[3];
                    mainAudioSource.Play();

                    PlayOneShot(GameAudioClip.ElevatorArrival);
                    break;

                case GameScenes.Level3:
                    mainAudioSource.clip = levelWideClips[3];
                    mainAudioSource.Play();

                    PlayOneShot(GameAudioClip.ElevatorArrival);
                    break;

                case GameScenes.Level4:
                    mainAudioSource.clip = levelWideClips[3];
                    mainAudioSource.Play();

                    PlayOneShot(GameAudioClip.ElevatorArrival);
                    break;

                case GameScenes.Level5:
                    mainAudioSource.clip = levelWideClips[3];
                    mainAudioSource.Play();

                    PlayOneShot(GameAudioClip.ElevatorArrival);
                    break;
            }
        }

        public void ForceAmbienceSFX()
        {
            mainAudioSource.clip = levelWideClips[2];
            mainAudioSource.Play();
        }

        public void ForcePlayBossMusic()
        {
            mainAudioSource.clip = levelWideClips[4];
            mainAudioSource.Play();
        }

        public void ForcePlayBossStunnedMusic()
        {
            mainAudioSource.clip = levelWideClips[5];
            mainAudioSource.Play();
        }

        public void SetSoundReferences(Slider musicSlider)
        {
            this.musicSlider = musicSlider;
        }

        public void ChangeSoundPitch(float value)
        {
            mainAudioSource.pitch = value;
            sfxSource.pitch = value;
        }

        public void SetVolume(float value)
        {
            mainAudioSource.volume = value;
            sfxSource.volume = value;

            musicSlider.value = value * 10f;
        }

        public void IncreaseVolumeByPoint1()
        {
            if (mainAudioSource.volume < 1f)
            {
                mainAudioSource.volume += 0.1f;
            }

            if (sfxSource.volume < 1f)
            {
                sfxSource.volume += 0.1f;
            }

            if (musicSlider.value < musicSlider.maxValue)
            {
                musicSlider.value += 1f;
            }
        }

        public void DecreaseVolumeByPoint1()
        {
            if (mainAudioSource.volume > 0f)
            {
                mainAudioSource.volume -= 0.1f;
            }

            if (sfxSource.volume > 0f)
            {
                sfxSource.volume -= 0.1f;
            }

            if (musicSlider.value > musicSlider.minValue)
            {
                musicSlider.value -= 1f; ;
            }
        }

        public void PlayOneShot(GameAudioClip clip)
        {
            sfxSource.PlayOneShot(sfxClips[(int)clip]);
        }

        public void SetGameWideSoundtrackState(bool pause)
        {
            if (pause)
            {
                mainAudioSource.Pause();
            }
            else
            {
                mainAudioSource.UnPause();
            }
        }

        private void OnDestroy()
        {
            GameManager.S.GameEventHandler.onSceneChanged -= ChangeSoundtrack;
        }
    }
}