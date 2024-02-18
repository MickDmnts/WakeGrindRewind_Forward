using UnityEngine;

using WGRF.Core;
using Cinemachine;

namespace WGRF.UI
{
    /// <summary>
    /// The camera shake handler
    /// </summary>
    public class ScreenShake : MonoBehaviour
    {
        ///<summary>The shaking time cache</summary>
        private float shakeTimer;
        ///<summary>The current shake time</summary>
        private float shakeTimerTotal;
        ///<summary>The startin gintensity</summary>
        private float startIntensity;
        ///<summary>The cached virtual camera for the shaking</summary>
        CinemachineVirtualCamera vcamera;
        ///<summary>Is the camera currently shaking?</summary>
        bool isShaking = false;

        void Awake()
        {
            vcamera = GetComponent<CinemachineVirtualCamera>();
            ManagerHub.S.GameEventHandler.cameraShakeOnEnemyDeath += ShakeCamera;
        }

        // Update is called once per frame
        void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;

                CinemachineBasicMultiChannelPerlin perlin = vcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                perlin.m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);

            }
        }

        /// <summary>
        /// Makes the Camera shake, based on intensity of shake and its duration.
        /// </summary>
        /// <param name="intensity">The intensity of the shake</param>
        /// <param name="timer">The duration of the shake</param>
        private void ShakeCamera(float intensity, float timer)
        {
            if (isShaking) { return; }

            if (timer > 0)
            {
                CinemachineBasicMultiChannelPerlin perlin = vcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_AmplitudeGain = intensity;
                shakeTimer = timer;
                shakeTimerTotal = timer;
                startIntensity = intensity;

                isShaking = true;
            }
        }

        void OnDestroy()
        { ManagerHub.S.GameEventHandler.cameraShakeOnEnemyDeath -= ShakeCamera; }
    }
}