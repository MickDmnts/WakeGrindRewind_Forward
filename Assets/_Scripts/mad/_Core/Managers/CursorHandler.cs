using UnityEngine;

namespace WGRF.Core
{
    public class CursorHandler
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CursorHandler() { }

        /// <summary>
        /// Sets the cursor visible state to the passed state.
        /// </summary>
        /// <param name="state">The new state</param>
        public void SetCursorVisible(bool state) 
        { Cursor.visible = state; }

        /// <summary>
        /// Sets the cursor lock mode to the passed mode.
        /// </summary>
        /// <param name="lockMode">The new lock mode</param>
        public void SetCursorLockMode(CursorLockMode lockMode)
        { Cursor.lockState = lockMode; }
    }
}