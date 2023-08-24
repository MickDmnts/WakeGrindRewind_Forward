using UnityEngine;

namespace WGR.Core
{
    /* 
     * This script handles the fade in and fade out between scene changing.
     */
    public class SceneFader : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] Animator sceneFadeController;

        /// <summary>
        /// Call to play the FadeIn fader animation.
        /// </summary>
        public void FadeIn()
        {
            sceneFadeController.Play("FadeIn", 0);
        }

        /// <summary>
        /// Call to play the FadeOut fader animation.
        /// </summary>
        public void FadeOut()
        {
            sceneFadeController.Play("FadeOut", 0);
        }
    }
}