using UnityEngine;

namespace WGRF.WorldGeneration
{
    /// <summary>
    /// *Future feature*
    /// Attached to each room to determine its start and end entrances
    /// </summary>
    public class RoomController : MonoBehaviour
    {
        ///<summary>The entrance of the room</summary>
        [SerializeField, Tooltip("The entrance of the room")] GameObject start;
        ///<summary>The exit of the room</summary>
        [SerializeField, Tooltip("The exit of the room")] GameObject end;

        ///<summary>Returns the entrance of the room</summary>
        public GameObject StartPoint => start;
        ///<summary>Returns the exit of the room</summary>
        public GameObject EndPoint => end;

        /// <summary>
        /// Initializes the room to be placement ready
        /// </summary>
        public void InitializeForPlacement()
        { transform.root.rotation = Quaternion.Euler(0f, 0f, 0f); }
    }
}