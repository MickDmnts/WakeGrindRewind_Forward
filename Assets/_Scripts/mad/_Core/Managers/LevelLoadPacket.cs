using UnityEngine;

namespace WGRF.Core
{
    [CreateAssetMenu(fileName = "Level Load Packet", menuName = "Level Load Packet/Packet")]
    public class LevelLoadPacket : ScriptableObject
    {
        ///<summary>The packet index the LevelSceneManager list.</summary>
        public int PacketIndex;

        ///<summary>The scene this packet represents (mainly the one that it unloads first.)</summary>
        public GameScenes PacketMainScene;

        ///<summary>The scenes to load</summary>
        public GameScenes[] ScenesToLoad;

        ///<summary>The scenes to unload</summary>
        public GameScenes[] ScenesToUnload;
    }
}