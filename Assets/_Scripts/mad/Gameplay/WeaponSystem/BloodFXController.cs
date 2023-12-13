using UnityEngine;

namespace WGRF.Gameplay.BattleSystem
{
    /* [Class documentation]
     *  
     *  The purpose of this script is to be attached on the bloodFX prefab 
     *  and automatically play and destroy the particle system after 2 seconds.
     *  
     */
    public class BloodFXController : MonoBehaviour
    {
        //Private varible
        ParticleSystem bloodFX;

        private void Awake()
        {
            bloodFX = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            bloodFX.Play();
            Destroy(gameObject, 2f);
        }
    }
}