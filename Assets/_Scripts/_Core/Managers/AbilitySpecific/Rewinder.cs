using System;
using System.Collections;
using UnityEngine;
using WGR.Core.Managers;

namespace WGR.Abilities
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Dynamically used: Every class variable is changed throughout the game.
     * 
     * [Class Flow]
     * 1. When created this script sets interpolation duration to 1f and CanRecord to true.
     * 2. The main entry point of this script is the RecordEntity(...) method.
     * 
     * [Must Know]
     * 1. The start, middle and last positions of the entity passed are stored to achieve a rewind effect.
     * 2. The RewindEntity(...) method gets called from the RewindTime script.
     */

    public class Rewinder : MonoBehaviour
    {
        #region Dynamically_Changed
        public bool CanRecord { get; set; }

        //for External handling
        bool startRewind;
        IRewindable entity;
        Action methodCallbackOnFinish;

        //Bezier curve calculations
        float startTime, u, interpolationDuration;
        Vector3 entityLastPos, middlePos, startPos;
        //Vector3 p01, p12, p012;
        #endregion

        private void Awake()
        {
            interpolationDuration = 3f;
        }

        private void Start()
        {
            CanRecord = true;
        }

        /// <summary>
        /// Call to start the entity position storing.
        /// <para>Stores the entity passed.</para>
        /// </summary>
        /// <param name="entity">The gameObject to store.</param>
        /// <param name="abilityRecordDuration">How long does the storing lasts.</param>
        public void RecordEntity(IRewindable entity, float abilityRecordDuration)
        {
            this.entity = entity;

            StartCoroutine(RecordStartMiddlePositions(entity, abilityRecordDuration));
        }

        /// <summary>
        /// Call to store the start, and the (abilityRecordDuration / 2f) positions of the entity passed.
        /// </summary>
        /// <param name="entity">The gameObject to store its positions.</param>
        /// <param name="abilityRecordDuration">How long does the storing lasts.</param>
        IEnumerator RecordStartMiddlePositions(IRewindable entity, float abilityRecordDuration)
        {
            //store start position.
            startPos = entity.GetPosition();

            yield return new WaitForSeconds(abilityRecordDuration / 2f);

            //Store middle position
            middlePos = entity.GetPosition();

            yield return null;
        }

        /// <summary>
        /// Call to move the passed entity back to its original position.
        /// </summary>
        /// <param name="entity">The entity to move</param>
        /// <param name="externalCallback">The method to call when the rewind ends.</param>
        public void RewindEntity(IRewindable entity, Action externalCallback)
        {
            //Store the last position of the entity - used in the bezier curve calculation.
            entityLastPos = entity.GetPosition();

            this.methodCallbackOnFinish = externalCallback;

            //Set startRewind to true to calculate the bezier curve and move the gameObject
            startTime = Time.time;
            startRewind = true;

            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 0f;
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Rewind);
            }
        }

        private void FixedUpdate()
        {
            //Runs ONLY if the player used the rewind ability
            if (startRewind)
            {
                u = (Time.time - startTime) / interpolationDuration;

                if (u >= 1)
                {
                    u = 1;
                    startRewind = false;

                    //Reset 
                    ResetRewinderBehaviour();

                    methodCallbackOnFinish();
                }

                Vector3 p01, p12, p012;
                p01 = (1 - u) * entityLastPos + u * middlePos;
                p12 = (1 - u) * middlePos + u * startPos;

                p012 = (1 - u) * p01 + u * p12;

                //Continuously sets the entity position.
                entity.SetPosition(p012);
            }
        }

        /// <summary>
        /// Call to set CanRecord to true.
        /// </summary>
        public void ResetRewinderBehaviour()
        {
            CanRecord = true;
            u = 1;
            startRewind = false;

            if (GameManager.S.PlayerEntity != null)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.GetAnimator().speed = 1f;
                GameManager.S.GameSoundsHandler.ChangeSoundPitch(1f);
            }
        }
    }
}