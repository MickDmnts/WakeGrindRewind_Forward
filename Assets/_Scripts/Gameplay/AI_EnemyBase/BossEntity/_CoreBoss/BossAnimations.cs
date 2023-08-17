using UnityEngine;

namespace WGR.Gameplay.AI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Only the animator gets cached.
     * 
     * Base Types: AIEntityAnimations
     * 
     * This class is responsible for controlling and playing boss animations.
     * 
     */
    public class BossAnimations : AIEntityAnimations
    {
        //Private Variable
        Animator bossAnimator;

        private void Start()
        {
            bossAnimator = GetComponent<Animator>();
        }

        public override Animator GetAnimator()
        {
            return bossAnimator;
        }

        public override void SetAnimatorPlaybackSpeed(float value)
        {
            bossAnimator.speed = value;
        }

        #region BOSS_SPECIFIC_METHODS
        /// <summary>
        /// Call to set the idle animation state of the boss entity.
        /// </summary>
        public void SetIsIdleState(bool state)
        {
            bossAnimator.SetBool("isIdle", state);
        }

        /// <summary>
        /// Call to play the death animation of the boss entity.
        /// </summary>
        public void PlayDeathAnimation()
        {
            bossAnimator.Play("boss_death", 0);
        }
        #endregion
    }
}