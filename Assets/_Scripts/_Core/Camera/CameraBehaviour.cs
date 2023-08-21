using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace WGR.Core
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Class Flow]
     * 1. When the Player scene loads the camera automatically finds the player entity.
     * 2. On startup the script calculates the zoom values based on inspector given zoom values.
     * 
     * [Must Know]
     * 1. DeactivateCamera() gets called when the player dies.
     * 2. StartShakeSequence() gets called when an enemy dies by any means.
     * 3. OnZoomOut() gets called when the player fires a GUN
     * 4. OnZoomIn() gets called when the player stops shooting a GUN
     */

    public class CameraBehaviour : MonoBehaviour
    {
        #region INPECTOR_VARS
        [Header("\tSet in Inspector")]
        [Range(0.1f, 3f)]
        public float maxZoomOut;

        [Range(0.1f, 3f)]
        public float maxZoomIn;

        [Header("Zooming force")]
        [Range(0.001f, 1f)]
        public float zoomForce;

        [Header("Set boundaries per level")]
        [SerializeField] List<Collider> boundaries;
        #endregion

        #region DYNAMICALLY_CHANGED
        CinemachineVirtualCamera cmVirtualCam;
        CinemachineBasicMultiChannelPerlin cmNoisePerlin;

        //Internal zoom values
        bool playerIsShooting;
        float defaultOrthoSize = 7, defaultOrthoSizeCache, internalZoomOut, internalZoomIn;
        bool isActive = true;

        //Shake calculations
        float shakeLength;
        float totalShakeLength;

        float defaultIntensity;
        #endregion

        private void Start()
        {
            cmVirtualCam = GameObject.FindGameObjectWithTag("CM_Main").GetComponent<CinemachineVirtualCamera>();
            cmNoisePerlin = cmVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if (GameManager.S != null)
            {
                SubscribeEvents();

                CalculateInternalZoomValues();
            }

            defaultOrthoSizeCache = defaultOrthoSize;
        }

        #region STARTUP_BEHAVIOUR
        /// <summary>
        /// Call to subscribe the required methods to their appropriate events
        /// on the GameEventsHandler
        /// </summary>
        void SubscribeEvents()
        {
            GameManager.S.GameEventHandler.onSceneChanged += SetBoundary;

            GameManager.S.GameEventHandler.cameraShakeOnEnemyDeath += StartShakeSequence;

            GameManager.S.GameEventHandler.onPlayerShootStart += OnPlayerShoot;
            GameManager.S.GameEventHandler.onPlayerShootEnd += OnPlayerStopShoot;

        }

        /// <summary>
        /// Call to calculate the internal zoom values based on the inspector set values
        /// </summary>
        void CalculateInternalZoomValues()
        {
            internalZoomOut = defaultOrthoSize + maxZoomOut;
            internalZoomIn = defaultOrthoSize - maxZoomIn;
        }
        #endregion

        #region SHAKE_BEHAVIOUR
        /// <summary>
        /// Call to initiate the camera shake sequence.
        /// <para>Calls the SimulateShake() method</para>
        /// <para>Sets isShaking to true</para>
        /// </summary>
        public void StartShakeSequence(float length, float power)
        {
            shakeLength = length;

            cmNoisePerlin.m_AmplitudeGain = power;
            defaultIntensity = cmNoisePerlin.m_AmplitudeGain;

            totalShakeLength = length;
            shakeLength = length;
        }

        private void Update()
        {
            if (!isActive) return;

            if (shakeLength > 0)
            {
                shakeLength -= Time.deltaTime;
                DampShakeIntensity();
            }

            if (playerIsShooting)
            {
                cmVirtualCam.m_Lens.OrthographicSize = Interpolate(zoomForce, cmVirtualCam.m_Lens.OrthographicSize, internalZoomOut);
            }
            else
            {
                cmVirtualCam.m_Lens.OrthographicSize = Interpolate(0.03f, cmVirtualCam.m_Lens.OrthographicSize, internalZoomIn);
            }
        }

        void DampShakeIntensity()
        {
            cmNoisePerlin.m_AmplitudeGain = Mathf.Lerp(defaultIntensity, 0f, (1 - (shakeLength / totalShakeLength)));
        }
        #endregion

        #region SHOOT_BEHAVIOUR
        /// <summary>
        /// Call to slowly zoom out the camera with maxZoomOut being the internalZoomOut
        /// </summary>
        void OnPlayerShoot()
        {
            playerIsShooting = true;
        }

        /// <summary>
        /// Call to slowly zoom in the camera with maxZoomIn being the internalZoomIn
        /// </summary>
        void OnPlayerStopShoot()
        {
            playerIsShooting = false;
        }
        #endregion

        #region UTILITIES
        /// <summary>
        /// Smoothly lerp to the new value in each frame
        /// </summary>
        /// <param name="posA">The starting value</param>
        /// <param name="posB">The final value</param>
        /// <returns>A float closer to endValue each time Interpolate() is called based on lerpTime</returns>
        float Interpolate(float lerpTime, float startValue, float endValue)
        {
            return (1 - lerpTime) * startValue + lerpTime * endValue;
        }
        #endregion

        #region Boundary
        private void SetBoundary(GameScenes currentScene)
        {
            switch (currentScene)
            {
                case GameScenes.PlayerHub:
                    defaultOrthoSize = defaultOrthoSizeCache;
                    defaultOrthoSize -= 2;
                    //CalculateInternalZoomValues();

                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[0];
                    break;
                case GameScenes.NewGameIntro:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[1];
                    break;
                case GameScenes.Level1:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[2];
                    break;
                case GameScenes.Level2:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[3];
                    break;
                case GameScenes.Level3:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[4];
                    break;
                case GameScenes.Level4:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[5];
                    break;
                case GameScenes.Level5:
                    defaultOrthoSize += 2;
                    //CalculateInternalZoomValues();

                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[6];
                    break;
                case GameScenes.AbilitiesTutorial:
                    cmVirtualCam.GetComponent<CinemachineConfiner>().m_BoundingVolume = boundaries[7];
                    break;
            }

            CalculateInternalZoomValues();
        }
        #endregion

        private void OnDestroy()
        {
            GameManager.S.GameEventHandler.onSceneChanged -= SetBoundary;

            GameManager.S.GameEventHandler.cameraShakeOnEnemyDeath -= StartShakeSequence;

            GameManager.S.GameEventHandler.onPlayerShootStart -= OnPlayerShoot;
            GameManager.S.GameEventHandler.onPlayerShootEnd -= OnPlayerStopShoot;
        }

    }
}