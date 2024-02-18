using System;
using System.IO;

using UnityEngine;

namespace WGRF.Core
{
    /// <summary>
    /// The AppInternal class caches app specific variables to be used from other managers.
    /// </summary>
    public sealed class AppInternal
    {
        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.companyName);
        ///<summary>The WGR: Legacy application path</summary>
        string wgrLegacyPath = Path.Combine(Application.dataPath, "legacy");
        ///<summary>The WGR: Legacy executable name</summary>
        string wgrAppName = "WakeGrindRewind_project.exe";

        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        public string AppDataPath => appDataPath;
        ///<summary>Returns the WGR: Legacy application path (contains executable)</summary>
        public string LegacyExecutable => Path.Combine(wgrLegacyPath, wgrAppName);

        /// <summary>
        /// Creates an app internal instance
        /// </summary>
        public AppInternal()
        { HandleAppDataDirectory(appDataPath); }

        ///<summary>Call to create the application folder inside the sourcePath path.</summary>
        void HandleAppDataDirectory(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            { Directory.CreateDirectory(sourcePath); }
        }
    }
}