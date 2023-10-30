using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WGRF.Core
{
    /// <summary>
    /// A struct representing the game's settings
    /// </summary>
    public struct UserSettings
    {
        ///<summary>Is gore enabled?</summary>
        public bool goreVFX;
        ///<summary>Master volume float value</summary>
        public float masterVolume;
        ///<summary>Soundtrack float value</summary>
        public float ostVolume;
        ///<summary>SFX float value</summary>
        public float sfxVolume;
    }

    /// <summary>
    /// The file handler of the game's settings file.
    /// </summary>
    public class SettingsHandler
    {
        ///<summary>The path to the settings folder</summary>
        string settingsFolderPath = string.Empty;
        ///<summary>Path to the settings file of the game</summary>
        string settingsFilePath = string.Empty;
        ///<summary>Game's default settings</summary>
        UserSettings defaultSettings;
        ///<summary>Currently cached user settings</summary>
        UserSettings userSettings;

        ///<summary>Returns the path to the settings folder.</summary>
        public string SettingsFolderPath => settingsFolderPath;
        ///<summary>Returns the path to the settings file of the game</summary>
        public string SettingsFilePath => settingsFilePath;
        ///<summary>Returns the currently cached user settings</summary>
        public UserSettings UserSettings => userSettings;

        /// <summary>
        /// Construct a settings handler reference
        /// </summary>
        public SettingsHandler()
        {
            Task.Run(async () =>
            {
                //App data path handling
                string path = ManagerHub.S.Globals.AppDataPath;

                settingsFolderPath = CreateSettingsFolder(path);

                //Construct default settings
                defaultSettings = new UserSettings()
                {
                    goreVFX = true,
                    masterVolume = 8,
                    ostVolume = 7,
                    sfxVolume = 8,
                };

                settingsFilePath = await HandleSettingsFileOnConstruction(settingsFolderPath);
            });
        }

        /// <summary>
        /// Genarates the settings absolute path and its associated folders
        /// </summary>
        /// <returns>The generated folder path.</returns>
        string CreateSettingsFolder(string _appDataPath)
        {
            string temp = Path.Combine(_appDataPath, "Settings");

            if (!Directory.Exists(temp))
            { Directory.CreateDirectory(Path.Combine(_appDataPath, "Settings")); }

            return temp;
        }

        /// <summary>
        /// Creates or loads the settings file inside the _settingsFolderPath passed.
        /// </summary>
        /// <param name="_settingsFolderPath">Where should the file be saved</param>
        /// <returns>Returns the settings file path</returns>
        async Task<string> HandleSettingsFileOnConstruction(string _settingsFolderPath)
        {
            string temp = Path.Combine(_settingsFolderPath, "settings.config");

            if (!File.Exists(temp))
            {
                try
                {
                    using (FileStream fs = File.Create(temp))
                    {
                        await fs.FlushAsync();
                        await fs.DisposeAsync();
                    }

                    bool writeSucceded = await WriteSettingsFile(temp, defaultSettings);

                    if (writeSucceded)
                    { userSettings = await ReadSettingsFile(temp); }
                    else
                    { throw new Exception("Could not write default settings"); }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
            }
            else
            {
                try
                {
                    userSettings = await ReadSettingsFile(temp);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }

            }

            return temp;
        }


        /// <summary>
        ///Writes the passed package to the settings config file.
        /// </summary>
        /// <param name="_settingsFilePath">The location of the settings config file</param>
        /// <param name="package">The new settings package</param>
        /// <returns>Whether the write was successfull or not.</returns>
        async Task<bool> WriteSettingsFile(string _settingsFilePath, UserSettings package)
        {
            using (FileStream fs = File.OpenWrite(_settingsFilePath))
            {
                string jsonStr = JsonConvert.SerializeObject(package, Formatting.Indented);

                await fs.WriteAsync(Encoding.Unicode.GetBytes(jsonStr));

                await fs.FlushAsync();
                await fs.DisposeAsync();
            }
            return true;
        }

        /// <summary>
        /// Constructs a new UserSettings package from the passed _settingsFilePath.
        /// </summary>
        /// <param name="_settingsFilePath">Where to read the already saved settings file json</param>
        /// <returns>A UserPackage with the read settings</returns>
        async Task<UserSettings> ReadSettingsFile(string _settingsFilePath)
        {
            string jsonStr = await File.ReadAllTextAsync(_settingsFilePath, Encoding.Unicode);
            return JsonConvert.DeserializeObject<UserSettings>(jsonStr); ;
        }

        /// <summary>
        /// Updates the settings config file
        /// </summary>
        /// <param name="newUserSettings">The new user settings.</param>
        public void UpdateUserSettings(UserSettings newUserSettings)
        {
            Task.Run(async () =>
            {
                await WriteSettingsFile(settingsFilePath, newUserSettings);
            });
        }
    }
}