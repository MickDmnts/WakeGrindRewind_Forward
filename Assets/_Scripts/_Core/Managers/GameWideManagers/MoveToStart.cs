using UnityEngine;
using WGR.Core.Managers;

namespace WGR.Scripted
{
    /* [CLASS DOCUMENTATION]
     * 
     * [Variable specific]
     * Dynamically changed: Theses values change in runtime.
     * 
     * [Class flow]
     * 1. The NotifyPlayerToSpawn() is called when the player enters a level with this script attached to a GameObject.
     * 
     * [Must Know]
     * 1. The currentPoint is set to the transform of the GameObject the script is attached to.
     */

    public class MoveToStart : MonoBehaviour
    {
        //Dynamically changed
        Vector3 currentPoint;

        private void Awake()
        {
            //Store the transform of the gameObject this script is attached to.
            currentPoint = transform.position;
        }

        private void Start()
        {
            if (currentPoint != null)
            {
                NotifyPlayerToSpawn(currentPoint);
            }
        }

        /// <summary>
        /// Called whenever a scene changed to move the player to the currentPoint.
        /// </summary>
        /// <param name="spawnPoint">The spawn position in the world.</param>
        void NotifyPlayerToSpawn(Vector3 spawnPoint)
        {
            if (GameManager.S != null)
                GameManager.S.GameEventHandler.OnPlayerSpawn(spawnPoint);
        }
    }
}