using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WGR.Core.Managers;
using WGR.BattleSystem;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Inspector values: These values must be set from the isnpector.
     * Private variable: This variable is changed throughout the game.
     * 
     * [Must know]
     * 1. The GameManager.S.SetWeaponSelectionUIHandler(...) gets called on scene load so the manager sets its own reference
     *      inside the manager hub.
     * 2. This manager is responsible for the user weapon selection screen in the player hub.
     */
    public class WeaponSelectionUI : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] List<Button> weaponButtons; //The WeaponPanel buttons.
        [SerializeField] List<Image> UIPanelWeaponSprites; //The WeaponPanel buttons weapon sprites.

        [SerializeField] List<Slider> weaponKillSliders; //The sliders under the weapon sprites
        [SerializeField] List<Sprite> colouredWeaponSprites; //The coloured weapon sprites from data assets.

        //Private variable
        List<Sprite> grayedOutWeaponSprites;

        private void Awake()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to initialize the grayedOutWeaponSprites list and invoke the 
        /// GameManager.SetWeaponSelectionUIHandler(...) to set THIS scripts reference as the manager.
        /// </summary>
        void EntrySetup()
        {
            grayedOutWeaponSprites = new List<Sprite>();

            if (GameManager.S != null)
            {
                GameManager.S.SetWeaponSelectionUIHandler(this);
            }
            else Utils.MissingComponent("GameManager", this);
        }

        private void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged += UpdateWeaponKillCounts;
            }
            else Utils.MissingComponent("GameManager", this);

            UIEntrySetup();
        }

        /// <summary>
        /// Call to invoke the PopulateWeaponUISprites() and PopulateUISprites().
        /// </summary>
        void UIEntrySetup()
        {
            WeaponRepresentersSetup();
            PopulateUISprites();
        }

        #region ON_SCENE_CHANGED
        /// <summary>
        /// Called every time a scene loads and if the focused scene is the playerHub then
        /// WeaponKillUpdate coroutine gets called to update the kill counter sliders.
        /// </summary>
        void UpdateWeaponKillCounts(GameScenes scene)
        {
            if (scene.Equals(GameScenes.PlayerHub))
            {
                StartCoroutine(WeaponKillUpdate());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator WeaponKillUpdate()
        {
            List<Weapon> weaponSOList = GameManager.S.WeaponManager.GetWeaponSOsList();
            Dictionary<WeaponType, WeaponUnlockData> weaponPairDictionary =
                GameManager.S.WeaponManager.WeaponKillCount.GetWeaponUnlockDataDict();

            //Iterate throughout the weaponSOList...
            for (int i = 0; i < weaponSOList.Count; i++)
            {
                //Cache the ith weapon kill count
                int currentKillCount = weaponPairDictionary[weaponSOList[i].WeaponType].KillCount;

                //Set the weaponKillSliders[i] value to the currentKillCount value...
                weaponKillSliders[i].value = currentKillCount;

                //Cache the IsUnlocked value of the currently inspected weapon...
                bool isWeaponUnlocked = weaponPairDictionary[weaponSOList[i].WeaponType].IsUnlocked;

                //And finaly, if the weapon is unlocked, SetNativeSize() of the weapon for perfect fit and
                // set the button sprite to the coloured weapon sprite equivalent.
                if (isWeaponUnlocked)
                {
                    UIPanelWeaponSprites[i].sprite = colouredWeaponSprites[i];
                    UIPanelWeaponSprites[i].SetNativeSize();
                }
            }

            yield return null;
        }
        #endregion

        /// <summary>
        /// Call to externally set the weaponType each weapon select button represents based on the WeaponManager - weaponSOlist indexing.
        /// <para>Also populates the grayedOutWeaponSprites list so both list elements have the same indexing.</para>
        /// </summary>
        void WeaponRepresentersSetup()
        {
            List<Weapon> weaponSOList = GameManager.S.WeaponManager.GetWeaponSOsList();

            for (int i = 0; i < weaponSOList.Count; i++)
            {
                weaponButtons[i].gameObject.GetComponent<WeaponRepresenter>().SetButtonWeaponType(weaponSOList[i].WeaponType);

                grayedOutWeaponSprites.Add(weaponSOList[i].weaponGrayedSprite);
            }
        }

        /// <summary>
        /// Call to set all the UIPanelWeaponSprites to their grayedOutWeaponSprites equivalent.
        /// </summary>
        void PopulateUISprites()
        {
            for (int i = 0; i < grayedOutWeaponSprites.Count; i++)
            {
                UIPanelWeaponSprites[i].sprite = grayedOutWeaponSprites[i];
                UIPanelWeaponSprites[i].SetNativeSize();
            }
        }

        /// <summary>
        /// Call to check if the currently passed weapon type is unlocked.
        /// <para>If true, then calls the WeaponManager.SetStartingWeaponOnPlayer(...) and passes the passed weapon type.</para>
        /// </summary>
        /// <param name="type">The weapon type to check for.</param>
        public void SetStartingWeapon(WeaponType type)
        {
            if (GameManager.S.WeaponManager.WeaponKillCount.IsWeaponUnlocked(type))
            {
                GameManager.S.WeaponManager.SetStartingWeaponOnPlayer(type);
            }
        }

        /// <summary>
        /// Call to iterate throughout the weaponButtons list and call the DeselectWeaponButton(...) of each WeaponRepresenter instance.
        /// <para>Passes the passed selectedButton value to the DeselectWeaponButton method parameter.</para>
        /// </summary>
        /// <param name="selectedButton"></param>
        public void DeselectAllButtonsButSelected(Button selectedButton)
        {
            foreach (Button button in weaponButtons)
            {
                button.GetComponent<WeaponRepresenter>().DeselectWeaponButton(selectedButton);
            }
        }

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onSceneChanged -= UpdateWeaponKillCounts;
            }
            else Utils.MissingComponent("GameManager", this);
        }
    }
}