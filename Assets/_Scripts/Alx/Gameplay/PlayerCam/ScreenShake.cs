using UnityEngine;

using WGRF.Core;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startIntensity;

    CinemachineVirtualCamera vcamera;

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
    {
        ManagerHub.S.GameEventHandler.cameraShakeOnEnemyDeath -= ShakeCamera;
    }
}

