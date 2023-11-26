using UnityEngine;

namespace WGRF.AI.Entities
{
    /* [CLASS DOCUMENTATION]
     *
     * Base abstract class for AI Entity animation controlling.
     */
    public abstract class AIEntityAnimations : MonoBehaviour
    {
        /// <summary>
        /// Call to get THIS AI entity animator reference.
        /// </summary>
        /// <returns></returns>
        public abstract Animator GetAnimator();

        /// <summary>
        /// Call to set THIS animators' playback speed to the passed value.
        /// </summary>
        /// <param name="value"></param>
        public abstract void SetAnimatorPlaybackSpeed(float value);
    }
}