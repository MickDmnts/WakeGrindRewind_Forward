using UnityEngine;
using WGR.Core.Managers;
using WGR.UI;

namespace WGR.Scripted
{
    public class AbilityTVActivator : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] SpriteRenderer tvSprite;
        [SerializeField] GameObject tvPrompt;

        HubPanelActivator hubPanelActivator;

        private void Awake()
        {
            hubPanelActivator = tvSprite.gameObject.GetComponent<HubPanelActivator>();
        }

        private void Start()
        {
            CheckForTVActivation();
        }

        /// <summary>
        /// Call to check if the player dies more than 2 times.
        /// <para>If true, set the alpha value of the TV sprite to 1 and activate its prompt</para>
        /// <para>else set its alpha value to 0.5f and deactivate its prompt.</para>
        /// </summary>
        void CheckForTVActivation()
        {
            if (GameManager.S.PlayerDeaths >= 2)
            {
                tvSprite.color = new Color(tvSprite.color.r, tvSprite.color.g, tvSprite.color.b, 1f);
                tvPrompt.SetActive(true);

                hubPanelActivator.SetCanActivate(true);
            }
            else
            {
                tvSprite.color = new Color(tvSprite.color.r, tvSprite.color.g, tvSprite.color.b, 0.5f);
                tvPrompt.SetActive(false);

                hubPanelActivator.SetCanActivate(false);
            }
        }
    }
}
