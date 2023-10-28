using System.IO;
using System.Threading.Tasks;

namespace WGRF.Core
{
    public class SettingsHandler
    {
        ///<summary>The path to the settings folder</summary>
        string settingsFolderPath = string.Empty;

        ///<summary>Returns the path to the settings folder.</summary>
        public string SettingsFolderPath => settingsFolderPath;

        public SettingsHandler()
        {
            Task.Run(() =>
            {
                string path = ManagerHub.S.Globals.AppDataPath;

                settingsFolderPath = CreateSettingsFolder(path);
            });
        }

        /// <summary>
        /// Genarates the settings absolute path and its associated folders
        /// </summary>
        /// <returns>The generated folder path.</returns>
        string CreateSettingsFolder(string _appDataPath)
        {
            string temp = Path.Combine(_appDataPath, "Settings", "wgrfDb.db");

            if (!Directory.Exists(Path.Combine(_appDataPath, "Settings")))
            { Directory.CreateDirectory(Path.Combine(_appDataPath, "Settings")); }

            return temp;
        }

        //@TODO: Create json file with settings
        //@TODO: Create struct that acts as a data container for settings
    }
}