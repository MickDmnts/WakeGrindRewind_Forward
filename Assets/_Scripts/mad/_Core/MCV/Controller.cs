using System;
using System.Collections.Generic;
using UnityEngine;

namespace WGRF.Core
{
    [DefaultExecutionOrder(-399)]
    public class Controller : CoreBehaviour
    {
        ///<summary>Caches the script references of this Controller.</summary>
        Dictionary<(Type, string), System.Object> scripts = new Dictionary<(Type, string), System.Object>();
        ///<summary>Caches the component references of this Controller</summary>
        Dictionary<string, HashSet<(Type, string)>> components = new Dictionary<string, HashSet<(Type, string)>>();

        /// <summary>
        /// Registers a CoreBehaviour Unity script component into this controller.
        /// </summary>
        /// <param name="script">The CoreBehaviour script component that should be registered.</param>
        public void Register(CoreBehaviour script)
        {
            Type theType = script.GetType();
            Register(script.ID, theType, script);
        }

        /// <summary>
        /// Registers a MonoBehaviour Unity script component into this controller.
        /// </summary>
        /// <param name="id">The controller ID of the MonoBehaviour script component.</param>
        /// <param name="theType">The Class type of the MonoBehaviour script component.</param>
        /// <param name="script">The MonoBehaviour script component that should be registered.</param>
        public void Register(string _id, Type theType, MonoBehaviour script)
        {
            string typeName = theType.FullName;

            // Makes sure that an id has been set before registering to the controller.
            if (_id == "") { return; }

            (Type, string) id = (theType, _id);

            if (!scripts.TryAdd(id, script))
            {
                Debug.LogWarning("Script of the type " + typeName + " is already registered on this controller!");
                return;
            }

            components.TryAdd(typeName, new HashSet<(Type, string)>());
            components[typeName].Add(id);
        }

        /// <summary>
        /// Clears a CoreBehaviour Unity script component from this controller.
        /// </summary>
        /// <param name="script">The CoreBehaviour script component that should be registered.</param>
        public void Clear(CoreBehaviour script)
        {
            Type theType = script.GetType();
            Clear(script.ID, theType, script);
        }

        /// <summary>
        /// Clears a MonoBehaviour Unity script component from this controller.
        /// </summary>
        /// <param name="id">The controller ID of the MonoBehaviour script component.</param>
        /// <param name="theType">The Class type of the MonoBehaviour script component.</param>
        /// <param name="script">The MonoBehaviour script component that should be registered.</param>
        public void Clear(string _id, Type theType, MonoBehaviour script)
        {
            string typeName = theType.FullName;

            // Makes sure that an id has been set before registering to the controller.
            if (_id == "") { return; }

            (Type, string) id = (theType, _id);
            scripts.Remove(id);

            if (!components.TryGetValue(typeName, out var cmpnt)) { return; }
            cmpnt.Remove(id);
        }

        /// <summary>
        /// Gives access to Unity script component of a specific Class that is registered into this controller. 
        /// </summary>
        ///
        /// <param name="id">The Unity script component's ID that is registered into this controller.</param>
        public T Access<T>(string id) where T : class
        {
            Type theType = typeof(T);
            (Type, string) hid = (theType, id);

            return AccessByKey<T>(hid) as T;
        }

        /// <summary>
        /// Gives access to Unity script component of a specific Class that is registered into this controller. 
        /// </summary>
        /// <param name="key">The hashset key used as an ID to get access of the registred Unity script component.</param>
        T AccessByKey<T>((Type, string) key) where T : class
        {
            if (!scripts.TryGetValue(key, out var scrpt))
            {
                string theType = typeof(T).ToString();
                Debug.LogWarning("Script of the type " + theType + " with ID \"" + key.Item2 + "\" is not registered on this controller!");

                return null;
            }

            return scrpt as T;
        }
    }
}