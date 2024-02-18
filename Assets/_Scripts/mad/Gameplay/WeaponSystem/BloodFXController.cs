using UnityEngine;

namespace WGRF.BattleSystem
{
    /// <summary>
    /// The purpose of this script is to be attached on the bloodFX prefab 
    /// and automatically play and destroy the particle system after 2 seconds.
    /// </summary>
    public class BloodFXController : MonoBehaviour
    {
        ///<summary>The blood fx particle instance</summary>
        ParticleSystem bloodFX;

        private void Awake()
        { bloodFX = GetComponent<ParticleSystem>(); }

        private void Start()
        {
            bloodFX.Play();
            Destroy(gameObject, 2f);
        }
    }
}