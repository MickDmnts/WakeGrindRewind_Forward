using UnityEngine;

namespace WGRF.Core
{
    public class RoomEndController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ManagerHub.S.InternalTime.StopRoomTimer();
                ManagerHub.S.HUDHandler.OpenScoreUI();
                ManagerHub.S.InternalTime.ChangeTimeScale(0f);
            }
        }
    }
}