using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be asssigned from the inspector.
     * Private Variables: These variables change in runtime.
     * 
     * [Class flow]
     * This script is attached on a gameObject that can be found in runtime or cached before hand
     * and haves 3 public methods that can be called to display text on a predefined UI textBox.
     */

    public class DialogueController : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        TextMeshProUGUI infoText;
        Image speakerIcon;
        #endregion

        private void Awake()
        {
            CacheComponents();
        }

        /// <summary>
        /// Call to cache the script needed components.
        /// </summary>
        void CacheComponents()
        {
            infoText = GameManager.S.UIManager.GetInfoTextBoxPanel().GetComponentInChildren<TextMeshProUGUI>();
            speakerIcon = GameManager.S.UIManager.GetInfoTextBoxPanel().transform.GetChild(1).GetComponent<Image>();
        }

        /// <summary>
        /// Display the given sentence letter by letter in the given text box with a small interval.
        /// </summary>
        public IEnumerator TypeSentence(string sentenceToType, float typeSpeed, bool waitForInput, bool dontShowButtonPrompt = false)
        {
            SetIconAlphaValue(0f);

            //Clear the text
            infoText.SetText("");

            //Start displaying each letter one by one and play the Tick Sound 
            foreach (char letter in sentenceToType.ToCharArray())
            {
                infoText.text += letter;

                yield return new WaitForSeconds(typeSpeed);
            }

            if (!dontShowButtonPrompt)
            {
                infoText.text += "\n Press E to continue...";

                if (waitForInput)
                {
                    while (!Input.GetKeyDown(KeyCode.E))
                    {
                        //... and finally call the couroutine callback method
                        yield return null;
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// Display the given sentence letter by letter in the given text box with a small interval and then 
        /// call the passed callbackMethod.
        /// <para><paramref name="dontShowButtonPrompt"/> -If true then the sentence gets displayed automatically.</para>
        /// </summary>
        public IEnumerator TypeSentence(string sentenceToType, Action callbackMethod, float typeSpeed, bool waitForInput, bool dontShowButtonPrompt = false)
        {
            SetIconAlphaValue(0f);

            //Clear the text
            infoText.SetText("");

            //Start displaying each letter one by one and play the Tick Sound 
            foreach (char letter in sentenceToType.ToCharArray())
            {
                infoText.text += letter;

                yield return new WaitForSeconds(typeSpeed);
            }

            if (!dontShowButtonPrompt)
            {
                infoText.text += "\n Press E to continue...";

                if (waitForInput)
                {
                    while (!Input.GetKeyDown(KeyCode.E))
                    {
                        //... and finally call the couroutine callback method
                        yield return null;
                    }
                }
            }

            callbackMethod();
        }

        /// <summary>
        /// Display the given list of sentences along with their icons letter by letter in the text box with a small interval and then 
        /// call the passed callbackMethod.
        /// <para><paramref name="dontShowButtonPrompt"/> -If true then the sentence gets displayed automatically.</para>
        /// </summary>
        public IEnumerator TypeSentenceSequences(Action callbackMethod, float typeSpeed, List<string> sentenceToType, List<Sprite> imagePerSentence, bool dontShowPrompt = false)
        {
            //Clear the text
            infoText.SetText("");

            //Start displaying each letter one by one and play the Tick Sound 
            for (int i = 0; i < sentenceToType.Count; i++)
            {
                infoText.SetText("");

                speakerIcon.sprite = imagePerSentence[i];

                //Enable the speaker icon
                SetIconAlphaValue(1f);

                foreach (char letter in sentenceToType[i].ToCharArray())
                {
                    infoText.text += letter;

                    yield return new WaitForSeconds(typeSpeed);
                }

                if (!dontShowPrompt)
                {
                    infoText.text += "\n Press E to continue...";

                    while (!Input.GetKeyDown(KeyCode.E))
                    {
                        //... and finally call the couroutine callback method
                        yield return null;
                    }
                }
            }

            //Disable the speaker icon
            SetIconAlphaValue(0f);

            callbackMethod();
        }

        /// <summary>
        /// Call to set the speakerIcon alpha value to the passed value.
        /// </summary>
        /// <param name="value"></param>
        void SetIconAlphaValue(float value)
        {
            speakerIcon.color = new Color(255f, 255f, 255f, value);
        }
    }
}