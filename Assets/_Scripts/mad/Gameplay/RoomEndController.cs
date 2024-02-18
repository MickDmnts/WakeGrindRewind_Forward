using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// Responsible for showing the score of the completed room.
    /// </summary>
    public class RoomEndController : MonoBehaviour
    {
        ///<summary>Was this collider activated?</summary>
        bool gotActivated = false;

        void OnTriggerEnter(Collider other)
        {
            if (gotActivated) { return; }

            if (other.CompareTag("Player"))
            {
                ManagerHub.S.InternalTime.StopRoomTimer();
                ManagerHub.S.HUDHandler.OpenScoreUI();
                ManagerHub.S.InternalTime.ChangeTimeScale(0f);

                gotActivated = true;
            }
        }
    }
}