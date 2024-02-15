using System.Collections;
using UnityEngine;
using WGRF.AI;
using WGRF.Core;

namespace WGRF.Interactions
{
    public enum DoorRoom
    {
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
        Room6,
        Room7,
    }

    public class Breakable : MonoBehaviour, IKickable
    {
        [SerializeField] bool nextRoomDoor;
        [SerializeField] DoorRoom doorRoom;
        [SerializeField] float kickForce = 10;

        bool isLocked = false;

        void Start()
        { isLocked = nextRoomDoor; }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<AIEntity>())
            { SimulateKnockback(other.transform.position); }
        }

        public void SimulateKnockback(Vector3 incomingDir)
        {
            if (isLocked) { return; }

            StartCoroutine(Throwback(incomingDir));
        }

        IEnumerator Throwback(Vector3 incomingDir)
        {
            GetComponent<Collider>().isTrigger = true;
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Rigidbody rb = transform.GetChild(0).GetComponent<Rigidbody>();
                rb.isKinematic = false;
                transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(-90, 90), 0));
                rb.AddRelativeForce((-(incomingDir - transform.position)) * kickForce, ForceMode.Impulse);

                transform.GetChild(0).gameObject.AddComponent<Scaler>();
                transform.GetChild(0).SetParent(null);
            }

            yield return null;

            Destroy(gameObject);
        }

        void LateUpdate()
        {
            if (nextRoomDoor)
            {
                int cnt = ManagerHub.S.AIHandler.GetAliveAgentCount((int)doorRoom);
                if (cnt <= 0)
                {
                    isLocked = false;
                    nextRoomDoor = false;
                    ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.ElevatorRing);
                }
            }
        }
    }
}