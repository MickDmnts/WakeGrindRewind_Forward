using System;
using System.IO;

using UnityEngine;

namespace WGRF.Core
{
    public class Globals
    {
        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        static string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.companyName);

        ///<summary>A static string which stores the App Data folder absolute path of the app.</summary>
        public static string AppDataPath => appDataPath;

        public Globals()
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