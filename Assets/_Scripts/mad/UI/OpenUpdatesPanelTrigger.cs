using UnityEngine;
using WGRF.Core;

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
