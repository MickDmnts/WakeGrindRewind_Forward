using System.Collections;
using UnityEngine;

namespace WGRF.Interactions
{
    public class Breakable : MonoBehaviour, IKickable
    {
        [SerializeField] float kickForce = 10;

        public void SimulateKnockback(Vector3 incomingDir)
        {
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
    }
}