using BackItUp.Models;
using System;
using System.Diagnostics;
using System.IO;

namespace BackItUp.ViewModels.TaskManagement
{
    public static class TaskManager
    {
        public static void TaskTest(BackupItem backupItem)
        {
            Debug.WriteLine(string.Format("TestTask Started. {0}", backupItem.BackupInterval.ToString()));


        }

        /// <summary>
        /// Return different args depending on whether we are backing up a file or a folder.
        /// </summary>
        /// <param name="originPath"></param>
        /// <param name="backupPath"></param>
        /// <returns></returns>
        private static string FormatRobocopyParameters(string originPath, string backupPath)
        {
            if(originPath.EndsWith(@"\"))
            {
                Debug.WriteLine("Making a new folder backup task.");
                string rcs =
                    string.Format("robocopy \"{0}\" \"{1}\" {2} /mir /copyall /r:5 /w:5 /MT:2",
                    Path.GetDirectoryName(originPath),
                    Path.Combine(Path.GetDirectoryName(backupPath), Path.GetFileName(Path.GetDirectoryName(originPath))),
                    "/LOG:F:\\log.txt");
                Debug.WriteLine(rcs);
                return rcs;
            }
            else
            {
                Debug.WriteLine("Making a new file backup task.");
                string rcs = 
                    string.Format("\"{0}\" \"{1}\" \"{2}\" /copyall /PURGE /r:5 /w:5 /MT:2",
                    Path.GetDirectoryName(originPath),
                    backupPath,
                    Path.GetFileName(originPath));
                Debug.WriteLine(rcs);
                return rcs;
            }
        }
    }
}
