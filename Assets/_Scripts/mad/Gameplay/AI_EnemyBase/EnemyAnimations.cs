using UnityEngine;
using WGRF.BattleSystem;
using WGRF.Core;

namespace WGRF.AI
{
    public class EnemyAnimations : AIEntityAnimations
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

        /// <summary>
        /// Plays the corresponding melee animation based on weapon type passed.
        /// </summary>
        public void PlayMeleeAnimation(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Punch:
                    enemyAnimator.Play("enemy_punch");
                    break;

                case WeaponType.Knife:
                    enemyAnimator.Play("enemy_knifeAttack");
                    break;

                case WeaponType.BaseballBat:
                    enemyAnimator.Play("enemy_batAttack");
                    break;
            }
        }

        /// <summary>
        /// Call to play the stunned agent animation.
        /// </summary>
        /// <param name="state"></param>
        public void SetStunnedAnimationState(bool state)
        {
            enemyAnimator.SetBool("isStunned", state);
        }
    }
}