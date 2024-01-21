using UnityEngine;
using UnityEngine.EventSystems;
using WGRF.Core;

namespace WGRF.UI
{
    public class OnMouseHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Set in inspector")]
        public AbilityType type;

        //Private Variables
        bool isHovering = false;

        /// <summary>
        /// Called whenever the cursor enters the assigned buttons' area to update the ability description 
        /// based on the selected ability type from the inspector.
        /// <para>Sets isHovering to true.</para>
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            //ManagerHub.S.UIManager.UserUIHandle.UpdateDescription(type);
            isHovering = true;
        }

        /// <summary>
        /// Called whenever the cursor gets pressed and isHovering is true FOR THIS SCRIPT INSTANCE.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isHovering)
            {
                //ManagerHub.S.UIManager.UserUIHandle.UpdateAbility(type);
            }
        }

        /// <summary>
        /// Called whenever the cursor leaves the assigned buttons' area to clear the ability 
        /// description text.
        /// <para>Sets isHovering to false.</para>
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            //ManagerHub.S.UIManager.UserUIHandle.ClearDescriptionText();
            isHovering = false;
        }
    }
}