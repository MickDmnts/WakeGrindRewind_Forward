using UnityEngine;
using WGR.BattleSystem;
using WGR.Core.Managers;

namespace WGR.Entities.Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        //Private variable
        Animator playerAnimator;

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange += PlaybackOnStateChange;
            }
        }

        /// <summary>
        /// Subscribed to the onPlayerStateChange event.
        /// <para>Call to set the animator playback speed based on the value passed.</para>
        /// <para>If the value passed is true then sets the speed to 1, otherwise 0.</para>
        /// </summary>
        /// <param name="isActive"></param>
        void PlaybackOnStateChange(bool isActive)
        {
            if (!isActive)
            {
                playerAnimator.speed = 0f;
            }
            else
            {
                playerAnimator.speed = 1f;
            }
        }

        /// <summary>
        /// Call to set the isWalking animator bool value to the passed value.
        /// </summary>
        public void SetWalkAnimationState(bool state)
        {
            playerAnimator.SetBool("isWalking", state);
        }

        /// <summary>
        /// Call to play the player kick animation.
        /// </summary>
        public void PlayKickAnimation()
        {
            playerAnimator.Play("player_Kick", 0);
        }

        /// <summary>
        /// Call to play the player death animation.
        /// </summary>
        public void PlayDeathAnimation()
        {
            playerAnimator.Play("player_death", 0);
            playerAnimator.SetBool("isDead", true);
        }

        /// <summary>
        /// Call to play the player wake up animation.
        /// </summary>
        public void PlayWakeUpAnimation()
        {
            playerAnimator.SetBool("isWakingUp", true);
            playerAnimator.SetBool("isThrowing", false);

            playerAnimator.Play("player_wakeUp", 0);
        }

        /// <summary>
        /// Call to play the player throw animation.
        /// </summary>
        public void PlayThrowAnimation()
        {
            playerAnimator.SetBool("isThrowing", true);
        }

        /// <summary>
        /// Call to play the respective player melee animation based on the weaponType passed.
        /// <para>Also disables WeaponHolding stance by calling SetRangedWeaponAnimation(...)</para>
        /// </summary>
        public void PlayMeleeWeaponAnimation(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Punch:
                    playerAnimator.Play("player_punch", 0);
                    break;

                case WeaponType.Knife:
                    playerAnimator.Play("player_knifeAttack", 0);
                    break;

                case WeaponType.BaseballBat:
                    playerAnimator.Play("player_batAttack", 0);
                    break;
            }
        }

        /// <summary>
        /// Call to set the isHoldingGun animator boolean value to the passed value.
        /// <para>If the passed value is true, then force play the isHolding gun animation so we miss no frames.</para>
        /// </summary>
        /// <param name="state"></param>
        public void SetRangedWeaponAnimation(bool state)
        {
            playerAnimator.SetBool("isHoldingGun", state);

            if (state.Equals(true))
            {
                playerAnimator.Play("player_holdingRangedGun");
            }
        }

        /// <summary>
        /// *ANIMATION EVENT*
        /// <para>Called when the player throw animation is finished to reset the animation boolean.</para>
        /// </summary>
        public void ThrowAnimationEnd()
        {
            playerAnimator.SetBool("isThrowing", false);
        }

        /// <summary>
        /// Call to get a player animator reference.
        /// </summary>
        public Animator GetAnimator() => playerAnimator;

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange -= PlaybackOnStateChange;
            }
        }
    }
}