using UnityEngine;

namespace WGRF.Core
{
    public class RoomEndController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ManagerHub.S.HUDHandler.OpenScoreUI();
                ManagerHub.S.InternalTime.ChangeTimeScale(0.01f);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ManagerHub.S.HUDHandler.CloseScoreUI();
                ManagerHub.S.InternalTime.ChangeTimeScale(1f);
            }
        }
    }
}