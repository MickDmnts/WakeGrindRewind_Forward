using UnityEngine;
using UnityEditor;

namespace WGRF.AI
{
    /* [CLASS DOCUMENTATION]
     * 
     * Every variable present in the class is private.
     * 
     * [Must know]
     * 1. Each detector is handled from its AIEntityFOVManager instance.
     * 2. Multiple detectors can be constructed in a gameObject.
     * 3. Every detector value can be set in its creation, further modification is not currently present.
     *      A. External modifications will be available in future updates.
     */
    [System.Serializable]
    public class AIEntityFOVDetector
    {
        #region PRIVATE_VARIABLES
        Transform target;
        float maxAngle;
        float maxRadius;
        bool isInFOV = false;
        bool isEnabled = false;

        AIEntityFOVManager fovManager;
        Transform detector;
        Color radiusColor;
        Color frustrumColor;
        #endregion

        /// <summary>
        /// Call to construct an AI Detector.
        /// </summary>
        /// <param name="enemyFOVManager">This detectors' manager.</param>
        /// <param name="origin">The center of the detector</param>
        /// <param name="radiusColor">The detectors' radius color. (Alpha set to 255f)</param>
        /// <param name="frustrumColor">The detectors' drustrum color. (Alpha set to 255f)</param>
        /// <param name="maxAngle">The detectors' max frustrum angle (ie. 45 = 90 degrees.)</param>
        /// <param name="maxRadious">The detectors' radius.</param>
        /// <param name="rotation">The detector's facing direction.</param>
        public AIEntityFOVDetector(AIEntityFOVManager enemyFOVManager,
            Transform origin, Color radiusColor, Color frustrumColor,
            float maxAngle, float maxRadious, Vector3 rotation)
        {
            this.fovManager = enemyFOVManager;

            this.detector = origin;

            this.detector.rotation = Quaternion.Euler(rotation);

            radiusColor.a = 255f;
            frustrumColor.a = 255f;

            this.radiusColor = radiusColor;
            this.frustrumColor = frustrumColor;

            this.maxAngle = maxAngle;
            this.maxRadius = maxRadious;

            IsEnabled(true);
        }

        /// <summary>
        /// Call to set THIS detectors' target.
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void Update()
        {
            //Early exit if tge detector is inactive OR is null
            if (target == null || !isEnabled) return;

            //Check for the target ray collision if it not found yet.
            if (!isInFOV)
            {
                isInFOV = IsInFOV(detector, target, maxAngle, maxRadius);
            }
            else //else notify the AI gameObject for target ray collision.
            {
                fovManager.GetEnemyEntity().OnPlayerFound();
                IsEnabled(false);
                isInFOV = false;
            }
        }

        /// <summary>
        /// Call to check if the target is inside the maxRadious and inside the designed frustrum.
        /// </summary>
        /// <param name="observer">The center of the raycast.</param>
        /// <param name="target">The target to check collisions for.</param>
        /// <param name="maxAngle">The detector angle.</param>
        /// <param name="maxRadius">The detector radius.</param>
        /// <returns>True if the target is directly hit inside the frustrum, false otherwise.</returns>
        private bool IsInFOV(Transform observer, Transform target, float maxAngle, float maxRadius)
        {
            Collider[] overlaps = new Collider[50];
            int count = Physics.OverlapSphereNonAlloc(observer.position, maxRadius, overlaps);

            for (int i = 0; i < count + 1; i++)
            {
                if (overlaps[i] == null) continue;

                if (overlaps[i].transform.Equals(target))
                {
                    Vector3 dirBetween = (target.position - observer.position).normalized;
                    float angle = Vector3.Angle(observer.forward, dirBetween);

                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(observer.position, target.position - observer.position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform.Equals(target))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Call to set the detector active state to the passed value.
        /// </summary>
        /// <param name="state"></param>
        public void IsEnabled(bool state)
        {
            this.isEnabled = state;
        }

        /// <summary>
        /// *For future expansion*
        /// <para>Call to set THIS detectors' new radius value.</para>
        /// </summary>
        private void SetRadius(float newRadiusValue)
        {
            this.maxRadius = newRadiusValue;
        }

        /// <summary>
        /// *For future expansion*
        /// <para>Call to set THIS detectors' new frustrum angle value.</para>
        /// </summary>
        private void SetFrustrumAngle(float newFrustrumAngle)
        {
            this.maxAngle = newFrustrumAngle; ;
        }

        #region VISUALIZATION
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if (target == null || !isEnabled) return;

            //Draw the detection radius around the enemy
            Handles.color = radiusColor;
            Handles.DrawWireDisc(detector.transform.position, Vector3.up, maxRadius);

            //Create the left and right linesOfSight
            Vector3 fovLineFront1 = Quaternion.AngleAxis(maxAngle, Vector3.up) * detector.forward * maxRadius;
            Vector3 fovLineFront2 = Quaternion.AngleAxis(-maxAngle, Vector3.up) * detector.forward * maxRadius;

            //Draw the FOV
            Gizmos.color = frustrumColor;
            Gizmos.DrawRay(detector.position, fovLineFront1);
            Gizmos.DrawRay(detector.position, fovLineFront2);

            if (!isInFOV)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawRay(detector.position, (target.position - detector.position).normalized * maxRadius);

            //Draw the middle black ray
            Gizmos.color = Color.black;
            Gizmos.DrawRay(detector.position, detector.forward * maxRadius);
        }
#endif
        #endregion
    }
}