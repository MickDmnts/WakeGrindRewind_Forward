using System;
using System.Collections;
using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// This class is responsivble for handling the internal time scale of the game.
    /// All time changes will and must happen here.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class InternalTime
    {
        ///<summary>The handler of the internal timer</summary>
        MonoBehaviour handler;
        ///<summary>The cached fixed delta time</summary>
        float fixedDeltaTime;

        ///<summary>Subscribe to this event to get notified when the time scale changes</summary>
        public event Action onTimeScaleChange;
        ///<summary>Subscribe to this event to get notified when the time scale resets</summary>
        public event Action onTimeScaleReset;

        ///<summary>Called when the time scale changes</summary>
        void OnTimeScaleChange()
        { onTimeScaleChange?.Invoke(); }

        ///<summary>Called when the time scale resets</summary>
        void OnTimeScaleReset()
        { onTimeScaleReset?.Invoke(); }

        /// <summary>
        /// Constructs an internal timer instance and assigns a handler to it for Coroutines only.
        /// </summary>
        /// <param name="handler">The handler of the internal time manager.</param>
        public InternalTime(MonoBehaviour handler)
        {
            this.fixedDeltaTime = Time.fixedDeltaTime;
            this.handler = handler;
        }

        /// <summary>
        /// Changes the time scale of the engine to the passed value.
        /// </summary>
        /// <param name="newTimeScale">Can not be greater than 1f and smaller than 0f</param>
        public void ChangeTimeScale(float newTimeScale)
        { 
            if (newTimeScale <= 0f)
            { newTimeScale = 0.1f; }
            else if (newTimeScale >= 1.0f)
            { newTimeScale = 1f; }

            OnTimeScaleChange();
            Time.timeScale = newTimeScale;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        /// <summary>
        /// Changes the time scale of the engine to the passed value and then resets if after the reset delay.
        /// </summary>
        /// <param name="newTimeScale">Can not be greater than 1f and smaller than 0f</param>
        /// <param name="resetAfterSecs">Any value.</param>
        public void ChangeTimeScale(float newTimeScale, float resetAfterSecs)
        {
            if (newTimeScale <= 0f)
            { newTimeScale = 0.1f; }
            else if (newTimeScale >= 1.0f)
            { newTimeScale = 1f; }

            OnTimeScaleChange();

            Time.timeScale = newTimeScale;
            handler.StartCoroutine(ResetTimeScaleAfter(resetAfterSecs, OnTimeScaleReset));
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        /// <summary>
        /// Resets the time scale after the passed seconds.
        /// </summary>
        /// <param name="seconds">Reset after ?</param>
        IEnumerator ResetTimeScaleAfter(float seconds, Action cb)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;

            cb();
        }
    }
}