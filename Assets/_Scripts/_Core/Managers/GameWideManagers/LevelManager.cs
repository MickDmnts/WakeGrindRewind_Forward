using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using WGR.UI;

namespace WGR.Core.Managers
{
    /// <summary>
    /// All the game scenes enumerated for ease of access, the index MUST be the same as 
    /// the build index.
    /// </summary>
    public enum GameScenes
    {
        AnyScene = -1,

        GameEntryScene = 0,
        MainMenu = 1,
        UI_Scene = 2,
        PlayerScene = 3,
        NewGameIntro = 4,
        PlayerHub = 5,
        AbilitiesTutorial = 6,
        Level1 = 7,
        Level2 = 8,
        Level3 = 9,
        Level4 = 10,
        Level5 = 11,
    }

    /* [CLASS DOCUMENTATION]
     * 
     * [Variable specific]
     * Inspector values: These values must be set from the inspector.
     * Dynamically changed: Theses values change in runtime.
     * 
     * [Class flow]
     * 1. The LoadNext() is called exactly when the game starts to load the MainMenu scene and let the game flow.
     * 
     * [Must Know]
     * 1. The levelsInLoadOrder list must be populated with LoadLevelPacket SOs that hold the scenes to load + unload.
     * 2. The ForceLoad method unloads EVERY active scene BUT the _GameEntry, UIRender and PlayerScene.
     * 3. The player cannot enter the MainMenuScene after firing a game sequence. This is used as a way to prevent manager duplication
     *          and data corruption.
     */

    [DefaultExecutionOrder(125)]
    public class LevelManager : MonoBehaviour
    {
        [Header("Set the desired essential game scenes.\n" +
            "These scenes will never get unloaded")]
        [SerializeField] List<GameScenes> essentialScenes;

        [Header("Populate the list with all the level load packets of the game.")]
        [SerializeField] List<LevelLoadPacket> levelsInLoadOrder;

        [Header("Set for manual index traveling")]
        [SerializeField] LevelLoadPacket toPlayerHubPacket;
        [SerializeField] LevelLoadPacket toNewGamePacket;
        [SerializeField] LevelLoadPacket toLoadGamePacket;
        [SerializeField] LevelLoadPacket toAbilitiesTests;

        //Dynamically changed
        List<GameScenes> currentlyLoadedScenes = new List<GameScenes>();
        int lastLoadedPacketIndex = 0;

        SceneFader fader;

        /// <summary>
        /// When the ActiveScene field is SET it automatically 
        /// calls the OnSceneChanged() event;
        /// </summary>
        private GameScenes[] _activeScenes;
        public GameScenes[] ActiveScene
        {
            get { return _activeScenes; }
            private set
            {
                _activeScenes = value;
            }
        }

        private GameScenes _focusedScene;
        public GameScenes FocusedScene
        {
            get { return _focusedScene; }
            private set
            {
                _focusedScene = value;
            }
        }

        public void SetFaderReference(SceneFader fader)
        {
            this.fader = fader;
        }

        private void Start()
        {
            //This boots up all the game scene loading system.
            LoadNext(false);
        }

        public void TransitToPlayerHub()
        {
            ForceLoad(toPlayerHubPacket.PacketIndex);
        }

        public void StartNewGameScene()
        {
            ForceLoad(toNewGamePacket.PacketIndex);
        }

        public void LoadGameScene()
        {
            ForceLoad(toLoadGamePacket.PacketIndex);
        }

        public void TransitToAbilitiesTests()
        {
            ForceLoad(toAbilitiesTests.PacketIndex);
        }

        #region NORMAL_PACKET_HANDLING
        /// <summary>
        /// Call to load the next scene load packet (order set from inspector).
        /// If fromForceLoad is true then a packet index must be passed
        /// so the packet indexer can automatically set itself in the correct List index.
        /// </summary>
        /// <param name="fromForceLoad"></param>
        /// <param name="packetIndex"></param>
        public void LoadNext(bool fromForceLoad, int packetIndex = 0)
        {
            StartCoroutine(InitiateUnloadLoadSequence(fromForceLoad, packetIndex));
        }

        IEnumerator InitiateUnloadLoadSequence(bool fromForceLoad, int packetIndex = 0)
        {
            //wait for the black to fade in
            if (fader != null) fader.FadeIn();

            if (GameManager.S.PlayerEntity != null)
                GameManager.S.PlayerEntity.IsActive = false;

            foreach (GameScenes scene in levelsInLoadOrder[lastLoadedPacketIndex].ScenesToUnload)
            {
                bool isSceneLoaded = SceneManager.GetSceneByBuildIndex((int)scene).isLoaded;

                if (isSceneLoaded)
                {
                    AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync((int)scene);

                    while (!asyncLoad.isDone)
                    {
                        yield return null;
                    }

                    if (GameManager.S.AIEntityManager != null)
                    {
                        GameManager.S.GameEventHandler.OnSceneUnloaded(scene);
                    }
                }
                else
                {
                    continue;
                }
            }

            _ClearCurrentlyLoadedList();

            StartCoroutine(_LoadPacketScenes(fromForceLoad, packetIndex));
            yield return null;
        }

        /// <summary>
        /// Call to create a new List<GameScenes>() and pass it to currentlyLoadedScenes.
        /// </summary>
        void _ClearCurrentlyLoadedList()
        {
            currentlyLoadedScenes = new List<GameScenes>();
        }

        /// <summary>
        /// Call to load every CURRENT packet scene with LoadSceneMode.Additive.
        /// Also adds every loaded scene to the currentlyLoadedScenes list.
        /// </summary>
        IEnumerator _LoadPacketScenes(bool fromForceLoad, int packetIndex = 0)
        {
            foreach (GameScenes scene in levelsInLoadOrder[lastLoadedPacketIndex].ScenesToLoad)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive);

                while (!asyncOperation.isDone)
                {
                    yield return null;
                }

                currentlyLoadedScenes.Add(scene);
            }

            OnSceneLoadingFinished(fromForceLoad, packetIndex);
        }

        void OnSceneLoadingFinished(bool fromForceLoad, int packetIndex = 0)
        {
            ActiveScene = currentlyLoadedScenes.ToArray();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)ActiveScene[ActiveScene.Length - 1]));
            FocusedScene = ActiveScene[ActiveScene.Length - 1];

            //If this is a normal loading sequence...
            if (!fromForceLoad)
            {
                //... calculate the next packet index
                int tempIndex = ++lastLoadedPacketIndex;
                SetPacketIndex(tempIndex);
            }
            else
            {
                //...if this a force load
                ReadjustPacketIndexOnForceLoad(packetIndex);
            }

            if (GameManager.S.GameEventHandler != null)
            {
                GameManager.S.GameEventHandler.OnSceneChanged(FocusedScene);

            }

            if (GameManager.S.PlayerEntity != null && FocusedScene != GameScenes.PlayerHub)
                GameManager.S.PlayerEntity.IsActive = true;

            if (fader != null) fader.FadeOut();
        }
        #endregion

        #region FORCE_LOAD
        /// <summary>
        /// Call to forcefully load a level, mainly used when the player dies to get back to the playerHub.
        /// </summary>
        /// <param name="levelPacketIndex">The packet index to load</param>
        public void ForceLoad(int levelPacketIndex)
        {
            StartCoroutine(_UnloadCurrentlyLoaded(levelPacketIndex));
        }

        IEnumerator _UnloadCurrentlyLoaded(int levelPacketIndex)
        {
            foreach (GameScenes scene in currentlyLoadedScenes)
            {
                if (essentialScenes.Contains(scene)) continue;

                AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync((int)scene);

                while (!asyncOperation.isDone)
                {
                    yield return null;
                }
            }

            SetPacketIndex(levelPacketIndex);

            LoadNext(true, levelPacketIndex);
        }
        #endregion

        #region UTILITIES
        /// <summary>
        /// Call to clamp the lastLoadedPacketIndex between 0 and levelsInLoadOrder.Count.
        /// </summary>
        /// <param name="index">The incremented packet index.</param>
        void SetPacketIndex(int index)
        {
            if (index >= levelsInLoadOrder.Count)
            {
                lastLoadedPacketIndex = 0;
            }
            else
            {
                lastLoadedPacketIndex = index;
            }
        }

        /// <summary>
        /// Call to correctly position the lastLoadedPacketIndex in the levelsInLoadOrder list. 
        /// </summary>
        /// <param name="levelPacketIndex">The force load packet index in the levelsInLoadOrder list</param>
        void ReadjustPacketIndexOnForceLoad(int levelPacketIndex)
        {
            //Store the passed packet
            LevelLoadPacket packet = levelsInLoadOrder[levelPacketIndex];

            //Store its MAIN SCENE - which is always the last in the ScenesToLoad list
            GameScenes packetMainScene = packet.ScenesToLoad[packet.ScenesToLoad.Length - 1];

            //Iterate through the levelsInLoadOrder and search for a packet that its MainScene
            //is the target scene of the passed load packet.
            foreach (LevelLoadPacket levelPacket in levelsInLoadOrder)
            {
                //if found, set the FOUND level packet index to be the current list indexer.
                if (levelPacket.PacketMainScene == packetMainScene)
                {
                    SetPacketIndex(levelPacket.PacketIndex);
                    return;
                }
            }
        }
        #endregion
    }
}