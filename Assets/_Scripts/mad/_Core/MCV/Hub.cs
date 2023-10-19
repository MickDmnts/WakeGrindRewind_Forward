using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// Creates the HUB singleton and makes sure that any child gameobject will be presereved on every scene loading.
    /// </summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-398)]
    public class Hub : Controller
    {
        ///<summary>The Hub instance reference</summary>
        static Hub instance;
        
        ///<summary>The Hub instance reference</summary>
        public static Hub Instance => instance;

        protected override void Awake()
        {
            instance = this;
            SetController(instance);

            DontDestroyOnLoad(gameObject);
        }
    }

}
