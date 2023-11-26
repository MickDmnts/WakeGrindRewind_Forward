using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using WGRF.AI.Entities;
using WGRF.Core;
using WGRF.Core.Managers;

namespace WGRF.AI.FOV
{
    /// <summary>
    /// A class used to transfer inpector given data to the FOV manager in runtime.
    /// </summary>
    [System.Serializable]
    class DetectorValue
    {
        public string detectorName;
        public Vector3 facing;
        public Color radiusColor;
        public Color frustrumColor;
        public float maxAngle;
        public float maxRadius;
    }

    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: Detector values must be set from the inspector so detectors can be constructed.
     * Private variables: These variables change throughout the game.
     * 
     * [Must know]
     * 0. Every AI Entity has a separate FOV manager attached to it.
     * 
     * 1. The manager creates every detector passed in the inspectorData list and stores it in a dictionary along with its GameObject anchor object
     *      so we have GameObject - Detector pairs.
     * 2. Detectors are updated through this manager.
     * 3. When a detector ray collides with the set target they notify the AIEntity through this manager.
     */
    public class AIEntityFOVManager : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] List<DetectorValue> inspectorData;

        public bool IsDetectorActive { get; private set; }

        #region PRIVATE_VARIABLES
        AIEntity enemyEntity;

        Dictionary<GameObject, DetectorValue> gameObjectDetectors;

        List<AIEntityFOVDetector> detectors;
        #endregion

        private void Awake()
        {
            enemyEntity = GetComponentInParent<AIEntity>();
            gameObjectDetectors = new Dictionary<GameObject, DetectorValue>();
        }

        private void Start()
        {
            //Sub to the ai entity events.
            if (enemyEntity != null)
            {
                enemyEntity.onPlayerFound += DeactivateAllDetectors;
                enemyEntity.onObserverDeath += DeactivateAllDetectors;
            }

            //Initialize the detectors list
            detectors = new List<AIEntityFOVDetector>();

            InitializeHoldersAndDetectors();

            //Used for detector testing.
            //If we are running the game as intented, get the detector target from the GM...
            if (ManagerHub.S != null)
            {
                SetTargetAllDetectors(ManagerHub.S.PlayerController.transform);
            }
            else //...else just create temporary fov target gameObjects so we get no errors.
            {
                SetTargetAllDetectors(new GameObject("TempFovTarget").transform);
            }

            //Mark the detectors as active.
            IsDetectorActive = true;
        }

        /// <summary>
        /// Call to create a detector parent gameObject with the inspector given (DetectorName) and assign it every detector passed from the inpector.
        /// </summary>
        void InitializeHoldersAndDetectors()
        {
            foreach (DetectorValue detector in inspectorData)
            {
                //Create the anchor
                GameObject tempGO = new GameObject
                {
                    name = detector.detectorName
                };
                tempGO.transform.SetParent(transform);
                tempGO.transform.position = transform.position;

                //Init the detector data
                InitializeDetector(detector, tempGO.transform);

                //Add the detector and its anchor to the dictionar.
                gameObjectDetectors.Add(tempGO, detector);
            }
        }

        /// <summary>
        /// Call to create a AIEntityFOVDetector instance and add it as a value to the detectors list.
        /// </summary>
        /// <param name="inspectorData">The data given from the inspector to construct the detector.</param>
        /// <param name="parent">The anchor of the detector.</param>
        void InitializeDetector(DetectorValue inspectorData, Transform parent)
        {
            AIEntityFOVDetector tempDetector = new AIEntityFOVDetector(this, parent, inspectorData.radiusColor,
                inspectorData.frustrumColor, inspectorData.maxAngle, inspectorData.maxRadius, inspectorData.facing);

            detectors.Add(tempDetector);
        }

        /// <summary>
        /// Call to mass-set the target of every detector inside the detectors list.
        /// </summary>
        /// <param name="target"></param>
        void SetTargetAllDetectors(Transform target)
        {
            foreach (AIEntityFOVDetector detector in detectors)
            {
                detector.SetTarget(target);
            }
        }

        /// <summary>
        /// Call to mark every Entity detector of THIS gameObject as inactive.
        /// </summary>
        public void DeactivateAllDetectors()
        {
            foreach (AIEntityFOVDetector detector in detectors)
            {
                detector.IsEnabled(false);
            }

            IsDetectorActive = false;
        }

        /// <summary>
        /// Call to mark every Entity detector of THIS gameObject as active.
        /// <para>If the enemy AI is marked as dead, then returns.</para>
        /// </summary>
        public void ActivateAllDetectors()
        {
            if (enemyEntity.GetIsDead()) return;

            foreach (AIEntityFOVDetector detector in detectors)
            {
                detector.IsEnabled(true);
            }

            IsDetectorActive = true;
        }

        //Detector updating
        private void Update()
        {
            UpdateDetectors();
        }

        /// <summary>
        /// Call to invoke the Update method of every detector present in the detectors list.
        /// <para>The order of update is based on the list indexing.</para>
        /// </summary>
        void UpdateDetectors()
        {
            if (!IsDetectorActive) return;

            foreach (AIEntityFOVDetector detector in detectors)
            {
                detector.Update();
            }
        }

        /// <summary>
        /// Call to get THIS FOV managers' enemy entity.
        /// </summary>
        /// <returns></returns>
        public AIEntity GetEnemyEntity()
        { return enemyEntity; }

        #region VISUALIZATION
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!EditorApplication.isPlaying)
            {
                foreach (DetectorValue inspectorSetup in inspectorData)
                {
                    //Draw the detection radius around the enemy
                    Handles.color = inspectorSetup.radiusColor;
                    Handles.DrawWireDisc(transform.position, Vector3.up, inspectorSetup.maxRadius);

                    //Create the left and right linesOfSight
                    Vector3 fovLineFront1 = Quaternion.AngleAxis(inspectorSetup.maxAngle, Vector3.up) * transform.forward * inspectorSetup.maxRadius;
                    Vector3 fovLineFront2 = Quaternion.AngleAxis(-inspectorSetup.maxAngle, Vector3.up) * transform.forward * inspectorSetup.maxRadius;

                    //Draw the FOV
                    Gizmos.color = inspectorSetup.frustrumColor;
                    Gizmos.DrawRay(transform.position, fovLineFront1);
                    Gizmos.DrawRay(transform.position, fovLineFront2);

                    //Draw the middle black ray
                    Gizmos.color = Color.black;
                    Gizmos.DrawRay(transform.position, transform.forward * inspectorSetup.maxRadius);

                    //Find the angle on a disc to visualize the editor facing values.
                    //So we can see where the detector will face in edit time.
                    Vector3 start = transform.position;
                    float angle = inspectorSetup.facing.y;
                    float radiants = angle * Mathf.Deg2Rad;

                    float x = Mathf.Cos(radiants) * inspectorSetup.maxRadius + transform.position.x;
                    float y = transform.position.y;
                    float z = Mathf.Sin(radiants) * inspectorSetup.maxRadius + transform.position.z;

                    Vector3 end = new Vector3(x, y, z);

                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(start, end);
                }
            }
            else
            {
                foreach (AIEntityFOVDetector detector in detectors)
                {
                    detector.OnDrawGizmos();
                }
            }
        }
#endif
        #endregion

        private void OnDestroy()
        {
            if (enemyEntity != null)
            {
                enemyEntity.onPlayerFound -= DeactivateAllDetectors;
                enemyEntity.onObserverDeath -= DeactivateAllDetectors;
            }
        }
    }
}