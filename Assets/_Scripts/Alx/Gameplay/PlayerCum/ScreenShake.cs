using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;
using Cinemachine;

public class ScreenShake : CoreBehaviour
{
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startIntensity;
    private bool isFiring;
    
    CinemachineVirtualCamera vcamera;

    protected override void  PreAwake()
    {
        vcamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {
        ManagerHub.S.GameEventsHandler.onPlayerShootStart() += ShakeCamera;
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

    
/*
 Makes the Camera shake, based on intensity of shake and its duration
*/
    private void ShakeCamera(float intensity, float timer)
    {
        if (timer > 0)
        {
            if (isFiring)
            {
                CinemachineBasicMultiChannelPerlin perlin = vcamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_AmplitudeGain = intensity;
                shakeTimer = timer;
                shakeTimerTotal = timer;
                startIntensity = intensity;
            }
          
        }
       
    }
   
    protected override void PreDestroy()
    {
        ManagerHub.S.GameEventHandler.onPlayerShootStart() -= ShakeCamera;

    
    }
}

