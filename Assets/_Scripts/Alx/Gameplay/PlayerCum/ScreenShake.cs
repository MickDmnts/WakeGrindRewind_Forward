using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;
using Cinemachine;

public class ScreenShake : CoreBehaviour
{
    private float shakeTimer;
    private float shakeTimerTotal;
    private bool isFiring;

    CinemachineVirtualCamera vcamera;

    void Awake()
    {
        vcamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        ManagerHub.S.GameEventsHandler.onPlayerFire() += ShakeCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFiring)
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;

                CinemachineBasicMultiChannelPerlin perlin = vcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                perlin.m_AmplitudeGain = Mathf.Lerp(startIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);

            }
            isFiring = false;
        }
    }

    private void ScreenShake(float intensity, float timer)
    {
        if (timer > 0)
        {
            if (isFiring) = true;
            CinemachineBasicMultiChannelPerlin perlin = vcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            perlin.m_AmplitudeGain = intensity;

            shakeTimer = timer;
            shakeTimerTotal = timer;
            startIntensity = intensity;
        }
       
    }
   
    private void PreDestroy()
    {
        ManagerHub.S.GameEventsHandler.onPlayerFire() -= ShakeCamera;
    }
}

