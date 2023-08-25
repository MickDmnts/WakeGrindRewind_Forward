using UnityEngine;

namespace WGR.Interactions
{
    /*
     * [Must Know]
     * 1. Used in KickableEntity class
     */
    public interface IKickable
    {
        void SimulateKnockback(Vector3 incomingDir);
    }
}
