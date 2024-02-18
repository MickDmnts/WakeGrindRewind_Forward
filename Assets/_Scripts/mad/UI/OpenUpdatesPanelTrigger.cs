using UnityEngine;
using WGRF.Core;

namespace WGRF.UI
{
    /// <summary>
    /// Responsible for opening the updates panel upon trigger interaction
    /// </summary>
    public class OpenUpdatesPanelTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ManagerHub.S.HUDHandler.OpenUpdatesUI();

                Destroy(gameObject);
            }
        }
    }
}