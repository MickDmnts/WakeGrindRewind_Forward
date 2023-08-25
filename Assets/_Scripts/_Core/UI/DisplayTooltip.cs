using UnityEngine;
using UnityEngine.EventSystems;
using WGR.Core.Managers;

namespace WGR.UI
{
    public class DisplayTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Set in inspector")]
        [SerializeField, TextArea] string textToDisplay;

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.S.UIManager.UserUIHandle.UpdateDescription(textToDisplay);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameManager.S.UIManager.UserUIHandle.ClearDescriptionText();
        }
    }
}