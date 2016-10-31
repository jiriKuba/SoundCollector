using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundCollector.HelpObjects.Interfaces
{
    interface ISettingService
    {
        Settings GetSettings();

        void SaveSettings();
    }
}
