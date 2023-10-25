using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WGRF.Core
{
    /// <summary>
    /// Asynchronous event handler for the LoadSignatureAsync and LoadAsync methods.
    /// </summary>
    public class AsyncGameObjectLoaderHandle
    {
        ///<summary>Called when the LoadAsync procedure is completed.</summary>
        public event Action<GameObject> Completed;

        /// <summary>
        /// Construct an AsyncGameObjectLoaderHandle instance.
        /// </summary>
        /// <param name="asset">The instance's loaded asset</param>
        /// <param name="path">The asset's path</param>
        /// <param name="global">Is the asset global?</param>
        public AsyncGameObjectLoaderHandle(AsyncOperationHandle<GameObject> asset, string path, bool global)
        {
            asset.Completed += (AsyncOperationHandle<GameObject> obj) =>
            {
                // Registers the loaded asset.
                UnityAssets.RegisterAsset(path, global, obj);

                Completed(obj.Result);
            };
        }
    }
}