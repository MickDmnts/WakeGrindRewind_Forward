using UnityEngine;

namespace WGR.Core
{
    /// <summary>
    /// This interface can be attached to any gameObject to 
    /// make it rewindable with the Rewind ability.
    /// </summary>
    public interface IRewindable
    {
        void SetPosition(Vector3 newPos);

        Vector3 GetPosition();
    }
}