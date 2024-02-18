using UnityEngine;
using UnityEngine.EventSystems;
using WGRF.Core;

namespace WGRF.UI
{
    /// <summary>
    /// Controls the pointer sprite on enter/exit
    /// </summary>
    public class DesktopIconHover : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        { ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Hand); }

        public void OnPointerExit(PointerEventData eventData)
        { ManagerHub.S.CursorHandler.SetMouseSprite(MouseSprite.Cursor); }
    }
}