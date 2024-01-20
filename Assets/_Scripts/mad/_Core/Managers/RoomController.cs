using UnityEngine;

namespace WGRF.Core
{
    public class RoomController : MonoBehaviour
    {
        [SerializeField] GameObject start;
        [SerializeField] GameObject end;

        public GameObject StartPoint => start;
        public GameObject EndPoint => end;

        public void InitializeForPlacement()
        {
            transform.root.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}