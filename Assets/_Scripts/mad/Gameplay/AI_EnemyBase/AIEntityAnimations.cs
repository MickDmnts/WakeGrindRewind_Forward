using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    public abstract class AIEntityAnimations : CoreBehaviour
    {
        protected Animator enemyAnimator;
        public Animator Animator  => enemyAnimator;

        /// <summary>
        /// Call to set THIS animators' playback speed to the passed value.
        /// </summary>
        /// <param name="value"></param>
        public abstract void SetAnimatorPlaybackSpeed(float value);
    }
}