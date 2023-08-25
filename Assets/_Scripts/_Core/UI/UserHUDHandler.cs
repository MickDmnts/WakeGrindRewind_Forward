using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using WGR.Core.Managers;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These values must be set from the inspector.
     * Private Variables: These values change in runtime.
     * 
     * [Must know]
     * 0. The script calls the GameManager.S.SetHUDHandler(...) and passes its reference to be set as a manager in the manager hub.
     * 
     * 1. The sole purpose of this script is to handle the user HUD regarding the ability visualization and
     *  currently equiped weapon info.
     * 2. The ability indexing works exactly as the Ability Manager one, thus the 0th ability icon is the SlowDown time
     *  the 1st one is the rewind time and the last one is the stop time icon. This indexing is used in every ability aspect.
     */
    public class UserHUDHandler : MonoBehaviour
    {
        #region INSPECTOR_VALUES
        [Header("Set in inspector - User Info Panels\n" +
            "Ability info, upper left corner")]
        [SerializeField] List<Image> selectedAbilityIcons;
        [SerializeField] Image nextAbilityIconPanel;
        [SerializeField] Image previousAbilityIconPanel;
        [SerializeField] List<Sprite> allAbilityIcons;

        [Header("Weapon info")]
        [SerializeField] TextMeshProUGUI remainingBullets;
        [SerializeField] Image bulletTypeImage;
        #endregion

        #region PRIVATE_VARIABLES
        Color[] iconCachedColors;

        int abilityFillSpriteIndex = 1;
        int abilityCountdownSpriteIndex = 2;
        #endregion

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.SetHUDHandler(this);

                CacheIconColours();
            }
        }

        #region
        /// <summary>
        /// Call to cache the starting icon colors from the selectedAbilityIcons list
        /// to the iconCachedColors array. (Based on the selectedAbilityIcons list order.)
        /// <para>Initialises the iconCachedColors array.</para>
        /// </summary>
        void CacheIconColours()
        {
            //Initialise the array
            iconCachedColors = new Color[selectedAbilityIcons.Count];

            for (int i = 0; i < selectedAbilityIcons.Count; i++)
            {
                iconCachedColors[i] = selectedAbilityIcons[i].color;
            }
        }

        /// <summary>
        /// Call to change all the sprites of the selectedAbilityIcons list to the passed abilityIcon sprite.
        /// <para>Invokes SetPreviousAndNextAbilityIcons(...) to visualise the ability circle 
        /// based on the current selected ability.</para>
        /// </summary>
        /// <param name="abilityIcon">The selected ability icon.</param>
        /// <param name="currentAbilityIndex">The selected ability index (based on AbilityManager indexing)</param>
        /// <param name="unlocked">The IsUnlocked ability field.</param>
        public void ChangeSelectedAbilityIcon(Sprite abilityIcon, int currentAbilityIndex, bool unlocked)
        {
            foreach (Image icon in selectedAbilityIcons)
            {
                icon.sprite = abilityIcon;
            }

            for (int i = 0; i < selectedAbilityIcons.Count; i++)
            {
                selectedAbilityIcons[i].color = iconCachedColors[i];
            }

            SetPreviousAndNextAbilityIcons(currentAbilityIndex);
        }

        /// <summary>
        /// Call to change the previous and next ability icons of the ability circle.
        /// <para>selectedAbilityIndex is used to determine the selected ability</para>
        /// </summary>
        void SetPreviousAndNextAbilityIcons(int selectedAbilityIndex)
        {
            switch (selectedAbilityIndex)
            {
                case 0:
                    previousAbilityIconPanel.sprite = allAbilityIcons[allAbilityIcons.Count - 1];
                    nextAbilityIconPanel.sprite = allAbilityIcons[1];
                    break;

                case 1:
                    previousAbilityIconPanel.sprite = allAbilityIcons[0];
                    nextAbilityIconPanel.sprite = allAbilityIcons[allAbilityIcons.Count - 1];
                    break;

                case 2:
                    previousAbilityIconPanel.sprite = allAbilityIcons[1];
                    nextAbilityIconPanel.sprite = allAbilityIcons[0];
                    break;
            }
        }

        /// <summary>
        /// Call to set the fill amount of the abilityFillSpriteIndex sprite inside the selectedAbilityIcons list
        /// to a value ratioed between 0-1 based on the remaing and total uses of an ability.
        /// </summary>
        public void UpdateRemainingUsesIcon(int remainingUses, int totalUses)
        {
            float min = 0f;
            float max = totalUses;

            //calculate a ratio between 0-1 based on the current + total uses of the 
            float ratio01 = (remainingUses - min) / (max - min);

            //Set the fill amount.
            selectedAbilityIcons[abilityFillSpriteIndex].fillAmount = ratio01;
        }

        /// <summary>
        /// Call to set the fill amount of the abilityCountdownSpriteIndex sprite inside the selectedAbilityIcons list
        /// to a value ratioed between 0-1 based on the remaing and total uses of an ability.
        /// </summary>
        public void UpdateRemainingTimeIcon(float remainingTime, float maxTime)
        {
            float min = 0f;
            float max = maxTime;

            //calculate a ratio between 0-1 based on the current + total uses of the 
            float ratio01 = (remainingTime - min) / (max - min);

            //Set the fill amount.
            selectedAbilityIcons[abilityCountdownSpriteIndex].fillAmount = ratio01;
        }
        #endregion

        #region WEAPON_RELATET_UI
        /// <summary>
        ///  Call to set the bulletTypeImage to the passed ammoSprite.
        ///  <para>If ammoSprite is left to null then the sprites' alpha value is set to 0.</para>
        /// </summary>
        public void ChangeWeaponInfo(Sprite ammoSprite = null)
        {
            if (ammoSprite == null)
            {
                bulletTypeImage.color = new Color(255f, 255f, 255f, 0f);
            }
            else
            {
                bulletTypeImage.color = Color.white;
            }

            //Sets the sprite
            bulletTypeImage.sprite = ammoSprite;
        }

        /// <summary>
        /// Call to set the remainingBullets text to the passed value.
        /// </summary>
        public void ChangeBulletsLeft(string bulletsLeft)
        {
            remainingBullets.SetText(bulletsLeft);
        }
        #endregion
    }
}