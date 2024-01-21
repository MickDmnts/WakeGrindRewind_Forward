using UnityEngine;
using UnityEngine.UI;
using WGRF.BattleSystem;
using WGRF.Core;

namespace WGRF.UI
{
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
            //ManagerHub.S.WeaponSelectionUIHandler.SetStartingWeapon(buttonType);
            //ManagerHub.S.WeaponSelectionUIHandler.DeselectAllButtonsButSelected(attachedButton);
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
