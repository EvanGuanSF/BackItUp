using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackItUp.ViewModels.TaskManagement
{
    public static class ProgramOptionsManager
    {

        /// <summary>
        /// Toggles run on startup for the program by creating or removing a task in the task scheduler.
        /// </summary>
        /// <param name="isRunOnStartupEnabled"></param>
        /// <returns></returns>
        public static bool ToggleRunOnStartup(bool isRunOnStartupEnabled)
        {
            // Save the new option to the program's config file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["RunOnStartup"].Value = isRunOnStartupEnabled.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (isRunOnStartupEnabled)
            {
                Debug.WriteLine("Enabling startup task.");
            }
            else
            {
                Debug.WriteLine("Disabling startup task.");
            }



            return true;
        }
    }
}
