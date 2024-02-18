using UnityEngine;

namespace WGRF.UI
{
    /// <summary>
    /// This script handles the fade in and fade out between scene changing.
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        ///<summary>The scene fader animator</summary>
        [Header("Set in inspector")]
        [SerializeField, Tooltip("The scene fader animator")] Animator sceneFadeController;

        /// <summary>
        /// Call to play the FadeIn fader animation.
        /// </summary>
        public void FadeIn()
        { sceneFadeController.Play("FadeIn", 0); }

        /// <summary>
        /// Call to play the FadeOut fader animation.
        /// </summary>
        public void FadeOut()
        { sceneFadeController.Play("FadeOut", 0); }
    }
}