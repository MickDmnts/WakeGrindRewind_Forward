using System;
using System.IO;

using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// The AppInternal class caches app specific variables to be used from other managers.
    /// </summary>
    public class AppInternal
    {
        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.companyName);

        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        public string AppDataPath => appDataPath;

        public AppInternal()
        {
            HandleAppDataDirectory(appDataPath);
        }

        ///<summary>Call to create the application folder inside the sourcePath path.</summary>
        void HandleAppDataDirectory(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            { Directory.CreateDirectory(sourcePath); }
        }
    }
}