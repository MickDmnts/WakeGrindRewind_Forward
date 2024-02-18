using System.Collections.Generic;
using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// All the available mouse sprite selections
    /// </summary>
    public enum MouseSprite
    {
        Cursor,
        Hand,
        Crosshair
    }

    /// <summary>
    /// Struct used to associate a MouseSprite with its texture
    /// </summary>
    [System.Serializable]
    public struct MouseSpritePair
    {
        ///<summary>The mouse sprite selection</summary>
        [SerializeField, Tooltip("The mouse sprite selection")]
        public MouseSprite MouseSprite;
        ///<summary>The mouse texture</summary>
        [SerializeField, Tooltip("The mouse texture")]
        public Texture2D MouseTexture;
    }

    /// <summary>
    /// This handler is responsible for every cursor specific setting.
    /// </summary>
    public class CursorHandler
    {
        ///<summary>MouseSprite and mouse textures pairs</summary>
        Dictionary<int, Texture2D> mouseTextures;

        /// <summary>
        /// Creates a Cursor handler instance.
        /// </summary>
        /// <param name="mouseTextures">The mouse pairs to sort</param>
        public CursorHandler(MouseSpritePair[] mousePairs)
        {
            this.mouseTextures = new Dictionary<int, Texture2D>();
            GroupMousePairs(ref mousePairs, ref mouseTextures);
        }

        /// <summary>
        /// Groups the passed mouse pairs into the passed mouseTextures dictionary
        /// </summary>
        /// <param name="mousePairs">The mouse pairs to create the groups from</param>
        /// <param name="mouseTextures">The mouse selection-mouse texture pairs.</param>
        void GroupMousePairs(ref MouseSpritePair[] mousePairs, ref Dictionary<int, Texture2D> mouseTextures)
        {
            for (int i = 0; i < mousePairs.Length; i++)
            { mouseTextures.Add((int)mousePairs[i].MouseSprite, mousePairs[i].MouseTexture); }
        }

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

        /// <summary>
        /// Sets the mouse in-game cursor to the passed sprite selection
        /// </summary>
        /// <param name="sprite">The new mouse selection</param>
        public void SetMouseSprite(MouseSprite sprite)
        { Cursor.SetCursor(mouseTextures[(int)sprite], Vector2.zero, CursorMode.Auto); }
    }
}