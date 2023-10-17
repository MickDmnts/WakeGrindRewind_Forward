using UnityEngine;

namespace WGRF.Interactions
{
    /// <summary>
    /// An interface used on kickable objects of the game for the kick interaction
    /// </summary>
    public interface IKickable
    {   
        void SimulateKnockback(Vector3 incomingDir);
    }
}
