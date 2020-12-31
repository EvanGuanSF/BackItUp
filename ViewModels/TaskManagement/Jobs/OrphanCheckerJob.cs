using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BackItUp.ViewModels.TaskManagement.Jobs
{
    public class OrphanCheckerJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            //Debug.WriteLine(string.Format("'{0}' tick: {1}", context.JobDetail.Key.Name.ToString().Substring(0, 5), DateTime.Now));

            // Grab the Scheduler instance from the Factory
            IScheduler scheduler = await TaskManager.SchedulerFactory.GetScheduler();

            // Get the list of all active hash codes according to the view model.
            List<string> jobHashCodes = BackupInfoViewModel.GetAllActiveHashCodes();

            // Get all running job names aka hash codes from the scheduler.
            GroupMatcher<JobKey> groupMatcher = GroupMatcher<JobKey>.GroupContains("ActiveBackups");
            // Get all JobKeys using groupMatcher.
            IReadOnlyCollection<JobKey> keys = await scheduler.GetJobKeys(groupMatcher);

            // Loop through the active jobs according to the viewmodel and and add orphaned job hash codes to a list to be removed.
            List<string> jobsToRemove = new List<string>();
            foreach (JobKey jobKeys in keys)
            {
                if(!jobHashCodes.Contains(jobKeys.Name))
                {
                    jobsToRemove.Add(jobKeys.Name);
                }
            }

            // Now loop through the jobsToRemove list and tell the TaskManager to remove those jobs.
            foreach(string jobHashCode in jobsToRemove)
            {
                TaskManager.RemoveBackupJob(jobHashCode);
            }

            // Finally, check to make sure BackupItems in the main view are scheduled and running.
            BackupInfoViewModel.ToggleAllJobs();
        }
    }
}
