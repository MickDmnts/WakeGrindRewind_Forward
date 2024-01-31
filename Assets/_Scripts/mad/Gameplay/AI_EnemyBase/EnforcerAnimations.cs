using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    public class EnforcerAnimations : AIEntityAnimations
    {
        protected override void PreAwake()
        {
            SetController(GetComponent<Controller>());
            enemyAnimator = GetComponent<Animator>();
        }

        public override void SetAnimatorPlaybackSpeed(float value)
        { enemyAnimator.speed = value; }

        /// <summary>
        /// Call to set the walking animation acitve state.
        /// </summary>
        public void SetWalkStateAnimation(bool state)
        {
            enemyAnimator.SetBool("isWalking", state);
        }

        /// <summary>
        /// Call to play the death animation.
        /// </summary>
        public void PlayDeathAnimation()
        {
            enemyAnimator.Play("enemy_death", 0);
            enemyAnimator.SetBool("isDead", true);
        }

        /// <summary>
        /// Call to activate the universal weapon holding stance of the enemy.
        /// </summary>
        /// <param name="state"></param>
        public void SetHoldingRangedWeaponState(bool state)
        {
            enemyAnimator.SetBool("isHoldingGun", state);

            if (state.Equals(true))
            {
                enemyAnimator.Play("enemy_HoldingGun");
            }
        }
    }
}