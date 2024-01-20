using System.Collections.Generic;
using UnityEngine;

namespace WGRF.Core
{
    public class RoomGenerator : CoreBehaviour
    {
        [SerializeField]
        GameObject playerHub;
        [SerializeField]
        List<GameObject> rooms;

        List<GameObject> unusedRooms;
        List<GameObject> usedRooms;

        RoomController activeRoom;

        protected override void PreAwake()
        { SetID("roomGen"); }

        void Start()
        { activeRoom = playerHub.GetComponent<RoomController>(); }

        public void ResetGenerator()
        {
            unusedRooms = rooms;
            for (int i = 0; i < usedRooms.Count; i++)
            {
                Destroy(usedRooms[i].transform.root.gameObject);
            }

            activeRoom = playerHub.GetComponent<RoomController>();
        }

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