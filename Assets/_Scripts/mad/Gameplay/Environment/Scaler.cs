using System.Collections;
using UnityEngine;

namespace WGRF.Interactions
{
    /// <summary>
    /// Responsible for scaling down a gameobject
    /// </summary>
    public class Scaler : MonoBehaviour
    {
        void Start()
        { StartCoroutine(ScaleDown()); }

        ///<summary>Scales down a gameobject to 0 scale</summary>
        IEnumerator ScaleDown()
        {
            Vector3 initialScale = transform.localScale;
            float duration = 3f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localScale = Vector3.zero;

            Destroy(gameObject);
        }
    }
}