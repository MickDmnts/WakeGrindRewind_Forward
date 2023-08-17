using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RetroAesthetics {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    #if UNITY_5_4_OR_NEWER && !RETRO_SCENE_VIEW_OFF
        [ImageEffectAllowedInSceneView]
    #endif

    public class RetroCameraEffect : MonoBehaviour
    {
        [Tooltip("If enabled, simulated TV noise is added to the output.")]
        public bool useStaticNoise = true;
        [Tooltip("Static noise texture. White regions represent noise.")]
        public Texture noiseTexture;
        [SerializeField, Range(0, 2.5f)]
        [Tooltip("Amount of TV noise to blend into the output.")]
        public float staticIntensity = 0.5f;

        public enum GlitchDirections {
            None, Vertical, Horizontal, Both
        }
        
        [Space]
        public GlitchDirections randomGlitches = GlitchDirections.Vertical;

        [SerializeField, Range(0, 2.5f)]
        public float glitchIntensity = 1.0f;
        [SerializeField, Range(0, 100)]
        public int glitchFrequency = 10;

        [Space]
        public bool useDisplacementWaves = true;

        [SerializeField, Range(0, 5f)]
        public float displacementAmplitude = 1f;
        
        [SerializeField, Range(10, 150)]
        public float displacementFrequency = 100;
        
        [SerializeField, Range(0, 5)]
        public float displacementSpeed = 1f;

        [Space]
        public bool useChromaticAberration = true;
        
        [SerializeField, Range(0, 50)]
        public float chromaticAberration = 10f;

        [Space]
        public bool useVignette = true;
        [SerializeField, Range(0, 1)]
        public float vignette = 0.1f;

        [Space]
        public bool useBottomNoise = true;
        [Range(0.0f, 0.5f)]
        public float bottomHeight = 0.04f;
        [Range(0.0f, 3.0f)]
        public float bottomIntensity = 1.0f;
        public bool useBottomStretch = true;

        [Space]
        public bool useRadialDistortion = true;
        public float radialIntensity = 20.0f;
        public float radialCurvature = 4.0f;

        [Space]
        public float gammaScale = 1f;

        [Space]
        public bool useScanlines = true;
        public float scanlineSize = 512f;
        [Range(0f, 1f)]
        public float scanlineIntensity = 0.5f;

        [HideInInspector]
        public Material _material;

        private bool _isFading = false;
        private float _gammaTarget;
        private float _gammaDelta;
        private Action _callback;

        public virtual void Glitch(float amount = 1.0f) {
            Vector2 offsetPos = Vector2.zero;
            
            if (randomGlitches == GlitchDirections.Horizontal || randomGlitches == GlitchDirections.Both) {
                offsetPos.x = Random.Range(-0.25f * amount, 0.25f * amount);
            }
            
            if (randomGlitches == GlitchDirections.Vertical || randomGlitches == GlitchDirections.Both) {
                offsetPos.y = Random.Range(-0.25f * amount, 0.25f * amount);
            }

            _material.SetVector("_OffsetPos", offsetPos);

            _material.SetFloat("_ChromaticAberration", Random.Range(chromaticAberration, 
                            amount * chromaticAberration * 2.5f));
        }

        public virtual void FadeIn(float speed = 1.0f, Action callback = null) {
            _isFading = true;
            gammaScale = 0f;
            _gammaTarget = 1f;
            _gammaDelta = speed;
            _callback = callback;
        }

        public virtual void FadeOut(float speed = 1.0f, Action callback = null) {
            _isFading = true;
            _gammaTarget = 0f;
            _gammaDelta = -1f * speed; 
            _callback = callback;
        }
        
        void Awake() {
            _material = new Material(Shader.Find("Hidden/RetroCameraEffect"));
            _material.SetTexture("_SecondaryTex", noiseTexture);
            _material.SetFloat("_OffsetPosY", 0f);
            _material.SetFloat("_DisplacementAmplitude", displacementAmplitude / 1000f);
            _material.SetFloat("_DisplacementFrequency", displacementFrequency);
            _material.SetFloat("_DisplacementSpeed", displacementSpeed);   
        }

        void Update() {
            if (_isFading) {
                gammaScale += _gammaDelta * Time.deltaTime;
                if ((gammaScale > _gammaTarget && _gammaDelta > 0f) || 
                        (gammaScale < _gammaTarget && _gammaDelta < 0f)) {
                    gammaScale = _gammaTarget;
                    _isFading = false;
                    if (_callback != null) {
                        _callback();
                    }
                    _callback = null;
                }
            }
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null) {
                Awake();
            }

            // TV noise
            _material.SetFloat("_OffsetNoiseX", Random.Range(0f, 1.0f));
            float offsetNoise = _material.GetFloat("_OffsetNoiseY");
            _material.SetFloat("_OffsetNoiseY", offsetNoise + Random.Range(-0.05f, 0.05f));

            // Distortion
            if (randomGlitches != GlitchDirections.None) {
                if (Random.Range(0, 15) == 0) {
                    _material.SetFloat("_DisplacementAmplitude", Random.Range(0f, 10f * displacementAmplitude / 1000f));
                } else {
                    _material.SetFloat("_DisplacementAmplitude", displacementAmplitude / 1000f);
                }
            }

            // Fullscreen shift - after each jump return to center.
            Vector2 offsetPos = _material.GetVector("_OffsetPos");

            if (offsetPos.y > 0.0f) {
                offsetPos.y -= Random.Range(0f, +offsetPos.y);
            } else if (offsetPos.y < 0.0f) {
                offsetPos.y += Random.Range(0f, -offsetPos.y);
            }

            if (offsetPos.x > 0.0f) {
                offsetPos.x -= Random.Range(0f, +offsetPos.x);
            } else if (offsetPos.x < 0.0f) {
                offsetPos.x += Random.Range(0f, -offsetPos.x);
            }

            _material.SetVector("_OffsetPos", offsetPos);
            
            // Channel color shift
            float offsetColor = _material.GetFloat("_ChromaticAberration");
            if (offsetColor > chromaticAberration) {            
                _material.SetFloat("_ChromaticAberration", offsetColor - 15f * Time.deltaTime);
            } else if (randomGlitches != GlitchDirections.None && Random.Range(0, 100 - glitchFrequency) == 0) {
                Glitch(glitchIntensity);
            }

            if (useStaticNoise) _material.EnableKeyword("NOISE_ON"); 
            else _material.DisableKeyword("NOISE_ON");
            
            if (useChromaticAberration) _material.EnableKeyword("CHROMATIC_ON"); 
            else _material.DisableKeyword("CHROMATIC_ON");
            
            if (useDisplacementWaves) _material.EnableKeyword("DISPLACEMENT_ON"); 
            else _material.DisableKeyword("DISPLACEMENT_ON");
            
            if (useVignette) _material.EnableKeyword("VIGNETTE_ON");
            else _material.DisableKeyword("VIGNETTE_ON");
            
            if (useBottomNoise) _material.EnableKeyword("NOISE_BOTTOM_ON"); 
            else _material.DisableKeyword("NOISE_BOTTOM_ON");

            if (useBottomStretch) _material.EnableKeyword("BOTTOM_STRETCH_ON");
            else _material.DisableKeyword("BOTTOM_STRETCH_ON");
            
            if (useRadialDistortion) _material.EnableKeyword("RADIAL_DISTORTION_ON"); 
            else _material.DisableKeyword("RADIAL_DISTORTION_ON");

            if (useScanlines) _material.EnableKeyword("SCANLINES_ON"); 
            else _material.DisableKeyword("SCANLINES_ON");

            _material.SetFloat("_StaticIntensity", staticIntensity);
            _material.SetFloat("_ChromaticAberration", chromaticAberration);
            _material.SetFloat("_Vignette", vignette);
            _material.SetFloat("_NoiseBottomHeight", bottomHeight);
            _material.SetFloat("_NoiseBottomIntensity", bottomIntensity);
            _material.SetFloat("_RadialDistortion", radialIntensity * 0.01f);
            _material.SetFloat("_RadialDistortionCurvature", radialCurvature);
            _material.SetFloat("_ScanlineSize", scanlineSize);
            _material.SetFloat("_ScanlineIntensity", (1f - scanlineIntensity) * 1.34f);
            _material.SetFloat("_Gamma", gammaScale);
            
            Graphics.Blit(source, destination, _material);
        }

    }

}