using System.IO;
using UnityEngine;

namespace WGRF.Core
{
    public class Database
    {
        static string dbPath = "";

        public Database()
        {
            if (dbPath == "")
            {
#if UNITY_EDITOR
                dbPath = Path.Combine("URI=file:", Application.dataPath, "Resources", "wgrfDb.db");
#endif

#if UNITY_STANDALONE && !UNITY_EDITOR
                dbPath = "URI=file:" + Application.dataPath +  Path.DirectorySeparatorChar + "akbPlayerDb.db";
#endif
            }
        }
    }
}