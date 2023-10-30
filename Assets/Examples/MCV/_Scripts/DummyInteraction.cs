#if UNITY_EDITOR
using UnityEngine;
using WGRF.Core;

public class DummyInteraction : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);

        //Cache the controller of the collided-with object...
        Controller controller = collision.transform.root.GetComponent<Controller>();

        //Same as if(controller != null)...
        if (controller)
        {
            //Access the controller's cached script you want...
            controller.Access<DummyPlayer>("playerEntity").DummyAttack(10f); //... then do stuff.
        }
    }
}
#endif