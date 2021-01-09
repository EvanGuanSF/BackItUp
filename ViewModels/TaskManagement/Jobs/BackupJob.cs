using Quartz;
using RoboSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static BackItUp.Models.BackupItemStatusCodePairs;

namespace BackItUp.ViewModels.TaskManagement.Jobs
{
    public class BackupJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Debug.WriteLine(string.Format("'{0}' tick: {1}", context.JobDetail.Key.Name.ToString().Substring(0, 5), DateTime.Now));

            // Notify the UI that the job is now running.
            BackupInfoViewModel.SetBackupItemStatus(context.JobDetail.Key.Name.ToString(), (int)StatusCodes.RUNNING);

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string originPath = dataMap.GetString("originPath");
            string backupPath = dataMap.GetString("backupPath");

            if (originPath.EndsWith(@"\"))
            {
                // Copy directory
                try
                {
                    Debug.WriteLine(string.Format("{0} Copying directory: '{1}' to '{2}'", DateTime.Now, originPath, backupPath));

                    RoboCommand roboCopy = new RoboCommand();

                    // Copy options
                    roboCopy.CopyOptions.Source = originPath;
                    roboCopy.CopyOptions.Destination = Path.Combine(backupPath, Path.GetFileName(Path.GetDirectoryName(originPath)));
                    roboCopy.CopyOptions.CopySubdirectories = true;
                    roboCopy.CopyOptions.UseUnbufferedIo = true;
                    roboCopy.CopyOptions.Mirror = true;

                    // Selection options
                    roboCopy.SelectionOptions.IncludeSame = false;
                    roboCopy.SelectionOptions.IncludeTweaked = true;
                    roboCopy.SelectionOptions.ExcludeOlder = true;

                    // Retry options
                    roboCopy.RetryOptions.RetryCount = 3;
                    roboCopy.RetryOptions.RetryWaitTime = 5;

                    // Start and wait for the robocopy to finish.
                    await roboCopy.Start();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in BackupJob.Execute() folder " + e.Message);

                    // Notify the UI that the job has errored.
                    BackupInfoViewModel.SetBackupItemStatus(context.JobDetail.Key.Name.ToString(), (int)StatusCodes.ERROR);
                }
            }
            else
            {
                // Copy file
                try
                {
                    Debug.WriteLine(string.Format("{0} Copying file: '{1}' to '{2}'", DateTime.Now, originPath, backupPath));

                    RoboCommand roboCopy = new RoboCommand();

                    //Debug.WriteLine(Path.GetDirectoryName(originPath));
                    //Debug.WriteLine(backupPath);
                    //Debug.WriteLine(Path.GetFileName(originPath));

                    // Copy options
                    roboCopy.CopyOptions.Source = Path.GetDirectoryName(originPath);
                    roboCopy.CopyOptions.Destination = backupPath;
                    roboCopy.CopyOptions.FileFilter = new string[] { Path.GetFileName(originPath) };

                    // Selection options
                    roboCopy.SelectionOptions.IncludeSame = false;
                    roboCopy.SelectionOptions.IncludeTweaked = true;
                    roboCopy.SelectionOptions.ExcludeOlder = true;

                    // Retry options
                    roboCopy.RetryOptions.RetryCount = 3;
                    roboCopy.RetryOptions.RetryWaitTime = 5;

                    // Start and wait for the robocopy to finish.
                    await roboCopy.Start();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error in BackupJob.Execute() file" + e.Message);

                    // Notify the UI that the job has errored.
                    BackupInfoViewModel.SetBackupItemStatus(context.JobDetail.Key.Name.ToString(), (int)StatusCodes.ERROR);
                }
            }
            Debug.WriteLine(string.Format("Copy job '{0}' completed at: {1}", context.JobDetail.Key.Name.ToString().Substring(0, 5), DateTime.Now));

            // If the backup was successful, then update information for the item and save the BackupItem collection config to file.
            BackupInfoViewModel.NotifyItemHasBeenBackedUp(context.JobDetail.Key.Name);
            BackupInfoViewModel.UpdateNextBackupDate(context.JobDetail.Key.Name);
            BackupInfoViewModel.SaveConfig();
        }
    }
}
