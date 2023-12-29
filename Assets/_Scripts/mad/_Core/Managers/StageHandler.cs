using UnityEngine.SceneManagement;

namespace WGRF.Core
{
    /// <summary>
    /// Stage handler is responsible for scene loading and unloading in the game.
    /// </summary>
    public sealed class StageHandler
    {
        ///<summary>The addressables path prefix for the scenes group</summary>
        string scenesPathPrefix = "scenes/";
        ///<summary>The games available scenes</summary>
        string[] gameScenePaths;

        ///<summary>The addressables path prefix for the scenes group</summary>
        public string ScenesPathPrefix => scenesPathPrefix;
        ///<summary>The games available scenes</summary>
        public string[] GameScenePaths => (string[])gameScenePaths.Clone();

        ///<summary>Constructs a StageHandler instance</summary>
        public StageHandler(bool loadFromBoot = true)
        {
            GenerateScenes(ref this.gameScenePaths);

            if (loadFromBoot)
            { LoadFromBoot(); }
        }

        /// <summary>
        /// Populates the passed scenes array with Scene structs containing addressablePath-GameScenes pairs.
        /// </summary>
        /// <param name="gameScenes">The array to populate.</param>
        void GenerateScenes(ref string[] gameScenes)
        {
            gameScenes = new string[]
            {
                this.scenesPathPrefix + "entry",
                this.scenesPathPrefix + "mainMenu",
                this.scenesPathPrefix + "run",
            };
        }

        ///<summary>Loads the main menu scene when the game first boots.</summary>
        public void LoadFromBoot()
        {
            UnityAssets.LoadSceneAsync(gameScenePaths[1], LoadSceneMode.Single);
        }

        ///<summary>Loads the run scene</summary>
        public void LoadRun()
        {
            UnityAssets.LoadSceneAsync(gameScenePaths[2], LoadSceneMode.Single);
        }
    }
}