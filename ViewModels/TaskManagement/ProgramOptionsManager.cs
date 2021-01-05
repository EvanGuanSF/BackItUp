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
        /// <returns></returns>
        public static bool ToggleRunOnStartup(bool isRunOnStartupEnabled)
        {
            // Save the new option to the program's config file.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["RunOnStartup"].Value = isRunOnStartupEnabled.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            string scheduledTaskName = "[BackItUp]";

            if (isRunOnStartupEnabled)
            {
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
            else
            {
                try
                {
                    using (TaskService taskService = new TaskService())
                    {
                        Microsoft.Win32.TaskScheduler.Task task = taskService.FindTask(scheduledTaskName);

                        if(task == null)
                        {
                            return true;
                        }
                        else
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



            return true;
        }
    }
}
