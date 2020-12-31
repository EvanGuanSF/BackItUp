using Quartz;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BackItUp.ViewModels.TaskManagement.Jobs
{
    public class BackupJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //Debug.WriteLine(string.Format("'{0}' tick: {1}", context.JobDetail.Key.Name.ToString().Substring(0, 5), DateTime.Now));

            JobDataMap dataMap = context.JobDetail.JobDataMap;

            string originPath = dataMap.GetString("originPath");
            string backupPath = dataMap.GetString("backupPath");

            //Debug.WriteLine(string.Format("{0} {1}", originPath, backupPath));

            // Copy directory/file
            if (originPath.EndsWith(@"\"))
            {
                //Debug.WriteLine(string.Format("Copying directory: '{0}' to '{1}'", originPath, backupPath));
                //Debug.WriteLine(string.Format("Fixed directory paths: '{0}' to '{1}'", originPath, Path.Combine(Path.GetDirectoryName(backupPath), Path.GetFileName(Path.GetDirectoryName(originPath)))));

                try
                {
                    // Check for cyclic path names.
                    if (originPath.Contains(backupPath))
                    {
                        // Update the viewmodel data.
                        return;
                    }

                    // Create the backup directory path.
                    // OriginalDirectory: C:\OriginalDirectory and BackupDirectory: E:\Backups
                    // would create new backupDirectoryPath of E:\Backups\OriginalDirectory
                    string backupDirectoryPath = Path.Combine(Path.GetDirectoryName(backupPath), Path.GetFileName(Path.GetDirectoryName(originPath)));

                    // Check for and remove the backup directory if it already exists.
                    if (Directory.Exists(backupDirectoryPath))
                    {
                        Directory.Delete(backupDirectoryPath, recursive: true);
                    }

                    // Now create the backup directory.
                    Directory.CreateDirectory(backupDirectoryPath);

                    // Create all subfolders.
                    foreach (string dir in Directory.GetDirectories(originPath, "*", SearchOption.AllDirectories))
                    {
                        //Debug.WriteLine(string.Format("Creating subdir '{0}' from '{1}'", Path.Combine(backupDirectoryPath, dir.Substring(originPath.Length).TrimStart('\\')), dir));
                        Directory.CreateDirectory(Path.Combine(backupDirectoryPath, dir.Substring(originPath.Length).TrimStart('\\')));
                    }

                    // Copy files into the subfolders.
                    foreach (string file_name in Directory.GetFiles(originPath, "*", SearchOption.AllDirectories))
                    {
                        //Debug.WriteLine(string.Format("Copying file {0}", Path.Combine(backupDirectoryPath, file_name.Substring(originPath.Length).TrimStart('\\'))));
                        File.Copy(file_name, Path.Combine(backupDirectoryPath, file_name.Substring(originPath.Length).TrimStart('\\')));
                    }

                    //Debug.WriteLine("");
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return;
                }
            }
            else
            {
                //Debug.WriteLine(string.Format("Copying file: '{0}' to '{1}'", originPath, Path.Combine(backupPath, Path.GetFileName(originPath))));
                //Debug.WriteLine("");

                try
                {
                    // Remove the file if it already exists at the backup location.
                    if (File.Exists(Path.Combine(backupPath, Path.GetFileName(originPath).TrimStart('\\'))))
                    {
                        File.Delete(Path.Combine(backupPath, Path.GetFileName(originPath).TrimStart('\\')));
                    }

                    // Copy the file.
                    File.Copy(originPath, Path.Combine(backupPath, Path.GetFileName(originPath).TrimStart('\\')));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    return;
                }
            }

            // If the backup was successful, then update information for the item and save the BackupItem collection config to file.
            BackupInfoViewModel.NotifyItemHasBeenBackedUp(context.JobDetail.Key.Name);
            BackupInfoViewModel.UpdateNextBackupDate(context.JobDetail.Key.Name);
            BackupInfoViewModel.SaveConfig();
        }
    }
}
