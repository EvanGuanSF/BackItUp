using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32.TaskScheduler;

namespace BackItUp.ViewModels.TaskManagement
{
    public static class ProgramOptionsManager
    {
        /// <summary>
        /// Toggles run on startup for the program by creating or removing a task in the task scheduler.
        /// </summary>
        /// <param name="isRunOnStartupEnabled"></param>
        public static void ToggleRunOnStartup(bool isRunOnStartupEnabled)
        {
            // Save the new option to the program's config file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["RunOnStartup"].Value = isRunOnStartupEnabled.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            if (isRunOnStartupEnabled)
            {
                // Remove any existing task in case the .exe location has moved.
                DisableTask();
                // Then re-enable the scheduled task.
                EnableTask();
            }
            else
            {
                DisableTask();
            }
        }

        /// <summary>
        /// Add the "Run on Startup" task to the Windows Task Scheduler.
        /// </summary>
        private static void EnableTask()
        {
            string scheduledTaskName = "[BackItUp]";

            try
            {
                using (TaskService taskService = new TaskService())
                {
                    // Create a new task definition and assign properties
                    TaskDefinition taskDefinition = taskService.NewTask();
                    taskDefinition.RegistrationInfo.Description = "Runs the BackItUp application on startup.";
                    taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;

                    // Create a trigger that will fire the task on startup
                    taskDefinition.Triggers.Add(new LogonTrigger());

                    // Create an action that will launch Notepad whenever the trigger fires
                    taskDefinition.Actions.Add(new ExecAction(path: System.Reflection.Assembly.GetEntryAssembly().Location, arguments: "-hidden"));

                    // Register the task in the root folder
                    taskService.RootFolder.RegisterTaskDefinition(scheduledTaskName, taskDefinition);

                    // Remove the task we just created
                    //taskService.RootFolder.DeleteTask("Test");
                }
                //Debug.WriteLine("Enabling startup task.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("ToggleRunOnStartup add: " + e.Message);
            }
        }

        /// <summary>
        /// Remove the "Run on Startup" task from the Windows Task Scheduler.
        /// </summary>
        private static void DisableTask()
        {
            string scheduledTaskName = "[BackItUp]";

            try
            {
                using (TaskService taskService = new TaskService())
                {
                    // Try to find the task.
                    Microsoft.Win32.TaskScheduler.Task task = taskService.FindTask(scheduledTaskName);

                    // If it exists, remove it.
                    if (task != null)
                    {
                        taskService.RootFolder.DeleteTask(scheduledTaskName);
                    }
                }
                //Debug.WriteLine("Disabling startup task.");
            }
            catch (Exception e)
            {
                Debug.WriteLine("RemoveBackupJob remove: " + e.Message);
            }
        }
    }
}
