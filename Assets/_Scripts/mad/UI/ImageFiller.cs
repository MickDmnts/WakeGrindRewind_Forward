using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace WGRF.UI
{
    /// <summary>
    /// Simple fileld image filler
    /// </summary>
    public class ImageFiller : MonoBehaviour
    {
        ///<summary>Time to total fill</summary>
        [SerializeField, Tooltip("Time to total fill")] float maxFillTime = 1f;

        ///<summary>The image to fill</summary>
        Image fillImage;

        void Awake()
        { fillImage = GetComponent<Image>(); }

        void OnEnable()
        {
            StartCoroutine(Fill(maxFillTime));
        }

        /// <summary>
        /// Fills the image in fill time 
        /// </summary>
        IEnumerator Fill(float fillTime)
        {
            float timer = 0f;
            float startFillAmount = fillImage.fillAmount;
            while (timer < fillTime)
            {
                timer += Time.deltaTime;
                float fraction = Mathf.Clamp01(timer / fillTime);
                fillImage.fillAmount = Mathf.Lerp(startFillAmount, 1f, fraction);
                yield return null;
            }

            fillImage.fillAmount = 1f;
        }

        void OnDisable()
        { fillImage.fillAmount = 0f; }
    }
}