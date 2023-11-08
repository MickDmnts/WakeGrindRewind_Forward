using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace WGRF.Core
{
    /// <summary>
    /// A class containing every bit of logic about Addressable asset/scene loading.
    /// </summary>
    public static class UnityAssets
    {
        /// <summary>Holds the addressable paths of the globaly loaded assets. Note: Global assets are released from memory on the termination of the application.</summary>
        public static HashSet<string> GlobalLoadedAssets = new HashSet<string>();
        /// <summary>Holds the addressable paths of the loaded scene assets.
        /// Note: This list is cleaned every time a new scene is loaded unless the automatic scene assets release flag is cleared
        /// when the LoadSceneAsync method is called. In that case the ReleaseSceneAssets method should be called manualy.</summary>
        public static Dictionary<string, AsyncOperationHandle> LoadedScenes = new Dictionary<string, AsyncOperationHandle>();

        /// <summary>
        /// Releases all the currently loaded scene assets.
        /// </summary>
        public static void ReleaseSceneAssets()
        {
            // Makes sure that any loaded scene asset is released before loading the requested scene.
            foreach (AsyncOperationHandle obj in LoadedScenes.Values)
            {
                Addressables.Release(obj);
            }

            // Clears the addressable paths of the loaded scene assets.
            LoadedScenes.Clear();
        }

        /// <summary>
        /// Performs the registration process of a newly added asset.
        /// </summary>
        public static void RegisterAsset(string path, bool global, AsyncOperationHandle obj)
        {
            // Registers a global asset.
            if (global && !GlobalLoadedAssets.Contains(path))
            {
                GlobalLoadedAssets.Add(path);

                return;
            }

            // Registers a scene asset.
            LoadedScenes.TryAdd(path, obj);
        }

        /// <summary>
        /// Releases a registered scene asset from the memory.
        /// </summary>
        public static void ReleaseAsset(string path)
        {
            // Makes sure that the given path doesn't belongs to a global asset.
            if (GlobalLoadedAssets.Contains(path) || !LoadedScenes.ContainsKey(path)) { return; }

            // Releases the scene asset from the memory.
            Addressables.Release(LoadedScenes[path]);
            LoadedScenes.Remove(path);
        }

        /// <summary>
        /// Checks if a given addressabled path exists within the asset bundles.
        /// Note: This method should be used with care due to the performance impact.
        /// </summary>
        public static bool PathExists(object key)
        {
            foreach (IResourceLocator lst in Addressables.ResourceLocators)
            {
                return lst.Locate(key, typeof(GameObject), out IList<IResourceLocation> lctrs);
            }

            return false;
        }

        /// <summary>
        /// Loads a scene from a given addressables path.
        /// </summary>
        /// <param name="path">The addressables path of the scene.</param>
        /// <param name="delay">The time delay before loading the desired scene in seconds(s).</param>
        /// <param name="asar">The automatic scene assets release flag.</param>
        /// <param name="cb">The callback function that holds the scene instance.
        /// Note: If the callback is not set then a scene is automaticaly loaded.</param>
        public async static void LoadSceneAsync(string path, LoadSceneMode mode, int delay = 0, bool asar = true, Action<SceneInstance> cb = null)
        {
            bool ald = true;

            if (delay > 0) { await Task.Delay(delay * 1000); }

            // If a callback method is not assigned then the activate on load flag is raised.
            if (cb != null) { ald = false; }

            // Makes sure to release the loaded scene assets if the automatic scene assets release flag is raised.
            if (asar) { ReleaseSceneAssets(); }

            // Loads the new scene asynchronously.
            Addressables.LoadSceneAsync(path, mode, ald).Completed += (AsyncOperationHandle<SceneInstance> obj) =>
            {
                if (cb != null)
                {
                    obj.Result.ActivateAsync().completed += (AsyncOperation op) =>
                    {
                        cb(obj.Result);
                    };
                }
            };
        }

        /// <summary>
        /// Loads a GameObject asset from a given addressables path asynchronusly and calls a callback that contains the asset.
        /// </summary>
        /// <param name="path">The addressables path of the target asset.</param>
        /// <param name="global">States whether the loaded asset should be preserved in memory
        /// for the life of the application or the scene. Note: In case of the former,
        /// the asset should be disposed manualy from the memory.</param>
        /// <param name="cb">The callback method of this operation.</param>
        public static void LoadAsync(string path, bool global, Action<GameObject> cb)
        {
            Addressables.LoadAssetAsync<GameObject>(path).Completed += (AsyncOperationHandle<GameObject> obj) =>
            {
                // Registers the loaded asset.
                RegisterAsset(path, global, obj);

                cb(obj.Result);
            };
        }

        /// <summary>
        /// Loads a GameObject asset from a given addressables path synchronously.
        /// </summary>
        public static GameObject Load(string path, bool global)
        {
            AsyncOperationHandle<GameObject> obj = Addressables.LoadAssetAsync<GameObject>(path);

            return obj.WaitForCompletion();
        }

        /// <summary>
        /// Loads a GameObject asset from a given addressables path asynchronusly and returns an AsyncGameObjectLoaderHandle with the asset.
        /// </summary>
        public static AsyncGameObjectLoaderHandle LoadAsync(string path, bool global)
        {
            return new AsyncGameObjectLoaderHandle(Addressables.LoadAssetAsync<GameObject>(path), path, global);
        }
    }
}

