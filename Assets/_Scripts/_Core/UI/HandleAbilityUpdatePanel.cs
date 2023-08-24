using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WGR.Core
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector Variables: These variables must be asssigned from the inspector.
     * 
     * [Must know]
     * RefreshAbilityUI gets called every time the player hub gets loaded through OnSceneChanged().
     */

    public class HandleAbilityUpdatePanel : MonoBehaviour
    {
        #region INSPECTOR_VARIABLES
        [Header("Assign player points left text")]
        [SerializeField] TextMeshProUGUI pointsLeftText;
        [SerializeField] TextMeshProUGUI descriptionText;

        [Header("Set in inspector")]
        [SerializeField] List<Image> slowDownIcons;
        [SerializeField] List<Image> rewindTimeIcons;
        [SerializeField] List<Image> stopTimeIcons;

        [Header("Set in inspector")]
        [SerializeField] Button goToAbilitiesTestButton;
        #endregion

        private void Awake()
        {
            if (GameManager.S != null)
            {
                GameManager.S.UIManager.UserUIHandle = this;
                GameManager.S.GameEventHandler.onSceneChanged += RefreshUISceneChanged;
            }
        }

        /// <summary>
        /// Called whenever the scene gets changed to refresh the UI.
        /// </summary>
        public void RefreshUISceneChanged(GameScenes scene)
        {
            if (scene == GameScenes.PlayerHub)
            {
                RefreshAbilityUI();

                GameManager.S.UIManager.EnablePanel(UIPanel.PlayerInfo);
            }
        }

        private void Start()
        {
            if (goToAbilitiesTestButton != null)
            {
                goToAbilitiesTestButton.onClick.AddListener(() => GoToAbilitiesTests());
            }
        }

        /// <summary>
        /// Call to load the abilities test scene - assigned to the goToAbilitiesTest button.
        /// </summary>
        void GoToAbilitiesTests()
        {
            if (GameManager.S != null)
            {
                GameManager.S.LevelManager.TransitToAbilitiesTests();
                GameManager.S.UIManager.DisablePanel(UIPanel.AbilitiesPanel);
            }
        }

        /// <summary>
        /// Call to update the passed type ability.
        /// <para>Updates the description and refreshes the UI. (UpdateDescription() - RefreshAbilityUI())</para>
        /// </summary>
        public void UpdateAbility(AbilityType type)
        {
            if (GameManager.S != null)
            {
                GameManager.S.AbilityManager.UpdateAbilityTier(type);
            }

            UpdateDescription(type);
            RefreshAbilityUI();
        }

        /// <summary>
        /// Call to update the abilities UI description based on the ability type passed.
        /// <para>Refreshes the UI. (RefreshAbilityUI())</para>
        /// </summary>
        public void UpdateDescription(AbilityType type)
        {
            descriptionText.SetText(GameManager.S.AbilityManager.GetAbilityDescription(type));

            RefreshAbilityUI();
        }

        /// <summary>
        /// Call to set the description text to the passed value.
        /// </summary>
        public void UpdateDescription(string text)
        {
            descriptionText.SetText(text);
        }

        /// <summary>
        /// Call to set the description text string to String.Empty.
        /// <para>Calls RefreshAbilityUI().</para>
        /// </summary>
        public void ClearDescriptionText()
        {
            descriptionText.SetText(System.String.Empty);

            RefreshAbilityUI();
        }

        /// <summary>
        /// Call to update the remaining skill points text based on players skill points.
        /// <para>Calls HighlightAbilityIconsBasedOnTier().</para>
        /// </summary>
        void RefreshAbilityUI()
        {
            pointsLeftText.SetText(GameManager.S.SkillPointHandle.RemainingSkillPoints().ToString());

            HighlightAbilityIconsBasedOnTier();
        }

        /// <summary>
        /// Call to refresh the whole ability icon set and set X number of icosns to white, where X is its 
        /// ability tier unlocked by the player.
        /// </summary>
        void HighlightAbilityIconsBasedOnTier()
        {
            //Cache its ability tier
            int slowTier, rewTier, stopTier;
            slowTier = GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.Slowtime);
            rewTier = GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.RewindTime);
            stopTier = GameManager.S.AbilityManager.GetAbilityCurrentTier(AbilityType.StopTime);

            //Refresh its icon set
            ChangeIconColor(slowDownIcons, slowTier, Color.white);
            ChangeIconColor(rewindTimeIcons, rewTier, Color.white);
            ChangeIconColor(stopTimeIcons, stopTier, Color.white);
        }

        /// <summary>
        /// Call to set every icon up to the passed number (highlightUpTo) to passedColor.
        /// </summary>
        /// <param name="listOfIcons">The list containing the icon set.</param>
        /// <param name="highlightUpTo">How many icons to set the color of.</param>
        /// <param name="color">The color to set the icons to.</param>
        void ChangeIconColor(List<Image> listOfIcons, int highlightUpTo, Color color)
        {
            if (highlightUpTo > listOfIcons.Count)
            {
                Debug.LogWarning($"Passed list count of icons to hightlight - listCount{listOfIcons.Count} " +
                    $"is smaller than the wanted icon to hightlight {highlightUpTo}");
            }

            for (int i = 0; i < highlightUpTo; i++)
            {
                listOfIcons[i].color = color;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged -= RefreshUISceneChanged;
            }
        }
    }
}