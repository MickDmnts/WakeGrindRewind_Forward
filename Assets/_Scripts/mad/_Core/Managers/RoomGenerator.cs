using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;

namespace WGRF.WorldGeneration
{
    /// <summary>
    /// *Future feature*
    /// The room generator of the game
    /// </summary>
    public class RoomGenerator : CoreBehaviour
    {
        ///<summary>The player hub room</summary>
        [SerializeField, Tooltip("The player hub room")] GameObject playerHub;
        ///<summary>The rooms available for generation</summary>
        [SerializeField, Tooltip("The rooms available for generation")] List<GameObject> rooms;

        ///<summary>The remaining rooms cache</summary>
        List<GameObject> unusedRooms;
        ///<summary>The used rooms cache</summary>
        List<GameObject> usedRooms;
        ///<summary>The currently active room</summary>
        RoomController activeRoom;

        protected override void PreAwake()
        { SetID("roomGen"); }

        void Start()
        { activeRoom = playerHub.GetComponent<RoomController>(); }

        /// <summary>
        /// Resets the room generator
        /// </summary>
        public void ResetGenerator()
        {
            unusedRooms = rooms;
            for (int i = 0; i < usedRooms.Count; i++)
            { Destroy(usedRooms[i].transform.root.gameObject); }

            activeRoom = playerHub.GetComponent<RoomController>();
        }

        ///<summary> Instantiates, initializes and places the next room</summary>
        public void PlaceRoom()
        {
            int idx = Random.Range(0, unusedRooms.Count);
            GameObject room = Instantiate(unusedRooms[idx]);
            RoomController rc = room.GetComponent<RoomController>();
            rc.InitializeForPlacement();

            room.transform.position = activeRoom.EndPoint.transform.position;

            activeRoom = rc;
            usedRooms.Add(unusedRooms[idx]);
            unusedRooms.RemoveAt(idx);
        }
    }
}