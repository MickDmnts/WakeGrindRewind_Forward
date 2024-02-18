using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{   
    /// <summary>
    /// A base class for ai entity animations
    /// </summary>
    public abstract class AIEntityAnimations : CoreBehaviour
    {   
        ///<summary>The animator of this agent</summary>
        protected Animator enemyAnimator;

        ///<summary>Returnst the animator of this agent</summary>
        public Animator Animator  => enemyAnimator;

        /// <summary>
        /// Call to set THIS animators' playback speed to the passed value.
        /// </summary>
        public abstract void SetAnimatorPlaybackSpeed(float value);
    }
}