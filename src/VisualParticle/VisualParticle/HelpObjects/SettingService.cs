using SoundCollector.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCollector.HelpObjects
{
    class SettingService : Interfaces.ISettingService
    {
        private Settings _settings;

        private const String SETTINGS_FILE_NAME = "Settings.xml";

        private readonly String SettingsPath;

        public SettingService()
        {
            this.SettingsPath = System.IO.Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                MainGame.APPLICATION_ROAMING_FOLDER + "\\");
        }

        public Settings GetSettings()
        {
            try
            {
                if (!System.IO.Directory.Exists(this.SettingsPath) || !System.IO.File.Exists(this.SettingsPath + SETTINGS_FILE_NAME))
                {
                    this._settings = this.GetDefaultSettings();
                    this.SaveSettings();
                }
                else
                {
                    this._settings = DataUtils.LoadObjectAsXmlFromFile<Settings>(this.SettingsPath + SETTINGS_FILE_NAME);
                }
            }
            catch
            {
                this._settings = this.GetDefaultSettings();
            }
            return this._settings;
        }

        public void SaveSettings()
        {
            if (!System.IO.Directory.Exists(this.SettingsPath))
            {
                System.IO.Directory.CreateDirectory(this.SettingsPath);
            }

            DataUtils.SaveObjectAsXmlToFile(this._settings, this.SettingsPath + SETTINGS_FILE_NAME);
        }

        private Settings GetDefaultSettings()
        {
            return new Settings() { WindowHeight = 720, WindowWidth = 1280 };
        }
    }
}
