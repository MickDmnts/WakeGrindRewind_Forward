using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WGRF.AI;
using WGRF.Core;

namespace WGRF.Interactions
{
    /// <summary>
    /// The available door rooms
    /// </summary>
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

    /// <summary>
    /// A breakable door handler
    /// </summary>
    public class Breakable : MonoBehaviour, IKickable
    {
        ///<summary>Is this door a room blocker?</summary>
        [SerializeField, Tooltip("Is this door a room blocker?")] bool nextRoomDoor;
        ///<summary>The room this door belongs to</summary>
        [SerializeField, Tooltip("The room this door belongs to")] DoorRoom doorRoom;
        ///<summary>The kicking force</summary>
        [SerializeField, Tooltip("The kicking force")] float kickForce = 10;

        ///<summary>Is the door still locked?</summary>
        bool isLocked = false;
        ///<summary>The chromatic aberration post effect</summary>
        ChromaticAberration chromaticAberration;
        ///<summary>The chromatic aberration initial value </summary>
        float initialIntensity;

        void Start()
        {
            isLocked = nextRoomDoor;
            ManagerHub.S.PostProcessVolume.sharedProfile.TryGet<ChromaticAberration>(out chromaticAberration);
            initialIntensity = chromaticAberration.intensity.value;
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponentInParent<AIEntity>())
            { SimulateKnockback(other.transform.position); }
        }

        public void SimulateKnockback(Vector3 incomingDir)
        {
            if (isLocked) { return; }

            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.ForcePunch);

            StartCoroutine(IncreaseChromatic());
            StartCoroutine(Throwback(incomingDir));
        }

        ///<summary>Pushes every door piece to the opposite direction of the kick</summary>
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
        }

        ///<summary>Briefly increases the chromatic aberration intensity</summary>
        IEnumerator IncreaseChromatic()
        {
            float elapsedTime = 0f;
            while (elapsedTime < 0.15f)
            {
                elapsedTime += Time.deltaTime;
                chromaticAberration.intensity.value = Mathf.Lerp(initialIntensity, 1f, elapsedTime / 0.25f);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < 0.15f)
            {
                elapsedTime += Time.deltaTime;
                chromaticAberration.intensity.value = Mathf.Lerp(1f, initialIntensity, elapsedTime / 0.25f);
                yield return null;
            }

            chromaticAberration.intensity.value = initialIntensity;

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