using System.Collections;
using UnityEngine;

namespace WGRF.Interactions
{
    public class Scaler : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(ScaleDown());
        }

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