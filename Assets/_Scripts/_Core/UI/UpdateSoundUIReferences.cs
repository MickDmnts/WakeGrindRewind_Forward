using UnityEngine;
using UnityEngine.UI;

using WGR.Core.Managers;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Every class variable is private and dynamically changed.
     * 
     * [Must know]
     * 1. This script caches and assigns the Volume up/down methods to the volume up/down pause UI elements
     *  through the GameSoundsHandler manager.
     * 2. The GameSoundsHandler slider reference is also passed from this script.
     * 
     */
    public class UpdateSoundUIReferences : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        Slider musicSlider;
        Button volumeDown;
        Button volumeUp;
        #endregion

        private void Awake()
        {
            CacheUIElementReferences();
        }

        /// <summary>
        /// Call to find and cache the needed button and slider references.
        /// </summary>
        void CacheUIElementReferences()
        {
            musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
            volumeDown = GameObject.FindGameObjectWithTag("VolumeDown").GetComponent<Button>();
            volumeUp = GameObject.FindGameObjectWithTag("VolumeUp").GetComponent<Button>();
        }

        private void Start()
        {
            SetMusicSliderReference();
            SetOnClickEvents();
        }

        /// <summary>
        /// Call to pass the music slider reference to the GameSoundHandler manager.
        /// </summary>
        void SetMusicSliderReference()
        {
            GameManager.S.GameSoundsHandler.SetSoundReferences(musicSlider);
        }

        /// <summary>
        /// Call to assign the Decrease/Inscrease volume methods from the GameSoundsHandler to the volume up/down buttons.
        /// </summary>
        void SetOnClickEvents()
        {
            volumeDown.onClick.AddListener(() => GameManager.S.GameSoundsHandler.DecreaseVolumeByPoint1());
            volumeUp.onClick.AddListener(() => GameManager.S.GameSoundsHandler.IncreaseVolumeByPoint1());
        }
    }
}