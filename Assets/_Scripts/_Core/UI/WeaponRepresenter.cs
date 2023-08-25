using UnityEngine;
using UnityEngine.UI;
using WGR.BattleSystem;
using WGR.Core.Managers;

namespace WGR.UI
{
    /* [CLASS DOCUMENTATION]
     * 
     * All the class variables are private and changed in runtime.
     * 
     * [Must know]
     * 1. This script is attached to every UI weaponSelectButton and handles the 
     *  data transfer between the clicked weapon button and the WeaponSelectionUI manager.
     * 2. buttonType is externally set from inside the WeaponSelectionUI manager.
     * 
     */
    public class WeaponRepresenter : MonoBehaviour
    {
        #region PRIVATE_VARIABLES
        Button attachedButton;
        Color deselectedColor;
        Color selectedColor = Color.cyan;

        WeaponType buttonType;
        #endregion

        private void Start()
        {
            EntrySetup();
        }

        /// <summary>
        /// Call to set up the default script behaviour.
        /// </summary>
        void EntrySetup()
        {
            attachedButton = GetComponent<Button>();
            deselectedColor = attachedButton.image.color;

            attachedButton.onClick.AddListener(() => SelectAsActiveWeapon());
        }

        /// <summary>
        /// Call to set the buttonType value of this scripts instance to the passed value.
        /// </summary>
        /// <param name="type"></param>
        public void SetButtonWeaponType(WeaponType type)
        {
            buttonType = type;
        }

        /// <summary>
        /// * Added an onClick event on the attached button *
        /// <para>Call to invoke the WeaponSelectionUIHandler.SetStartingWeapon(...) and
        /// WeaponSelectionUIHandler.DeselectAllButtonsButSelected(...) to visualize the selected weapon.</para>
        /// </summary>
        void SelectAsActiveWeapon()
        {
            if (GameManager.S != null)
            {
                GameManager.S.WeaponSelectionUIHandler.SetStartingWeapon(buttonType);
                GameManager.S.WeaponSelectionUIHandler.DeselectAllButtonsButSelected(attachedButton);
            }
            else Utils.MissingComponent("GameManager", this);
            attachedButton.image.color = selectedColor;
        }

        public void DeselectWeaponButton(Button selectedButton)
        {
            if (!selectedButton.Equals(attachedButton))
            {
                //Visualize deselection
                attachedButton.image.color = deselectedColor;
            }
        }
    }
}
