﻿using BackItUp.Models;
using BackItUp.ViewModels.TaskManagement.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BackItUp.ViewModels.TaskManagement
{
    public static class TaskManager
    {
        private static readonly StdSchedulerFactory _SchedulerFactory = new StdSchedulerFactory();

        public static StdSchedulerFactory SchedulerFactory
        {
            get
            {
                return _SchedulerFactory;
            }
        }

        #region Init

        /// <summary>
        /// Starts a job that checks for orphans and removes them from the scheduler.
        /// </summary>
        public static async void StartOrphanCheckerJob()
        {
            //Debug.WriteLine(string.Format("Orphan checker started at: ", DateTime.Now));

            // Grab the Scheduler instance from the Factory
            IScheduler scheduler = await _SchedulerFactory.GetScheduler();

            // Check if the job exists.
            JobKey jobID = new JobKey("OrphanCheckerJob", "OrphanChecker");

            // Define the job and tie it to our OrphanCheckerJob class
            IJobDetail job = JobBuilder.Create<OrphanCheckerJob>()
                .WithIdentity(jobID)
                .Build();

            // Trigger the job to run now, and then repeat every 10 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("OrphanCheckerJob", "OrphanChecker")
                .StartAt(DateTime.Now)
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
        }

        #endregion

        #region En/Dequeue

        public static async Task QueueBackupJob(BackupItem backupItem)
        {
            //Debug.WriteLine(string.Format("'{0}' Started at: {1}", backupItem.HashCode.Substring(0, 5), DateTime.Now));
            //Debug.WriteLine(string.Format("'{0}' should run at: {1} and tick every {2} days(s)", backupItem.HashCode.Substring(0, 5), DateTime.Now.AddSeconds(10), backupItem.BackupInterval.Days));
            try
            {
                // First, check the HashCode of the BackupItem to make sure we have the info to make a job.
                if (string.IsNullOrWhiteSpace(backupItem.HashCode) || backupItem.HashCode.Length != 64)
                    return;

                // Grab the Scheduler instance from the Factory
                IScheduler scheduler = await _SchedulerFactory.GetScheduler();

                // Check if the job exists.
                JobKey jobID = new JobKey(backupItem.HashCode, "ActiveBackups");

                if (await scheduler.CheckExists(jobID))
                {
                    await scheduler.DeleteJob(jobID);
                }

                // Define the CopyJob.
                IJobDetail job = JobBuilder.Create<BackupJob>()
                    .WithIdentity(jobID)
                    .UsingJobData("originPath", backupItem.OriginPath)
                    .UsingJobData("backupPath", backupItem.BackupPath)
                    .Build();

                // Setup the job trigger.
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(backupItem.HashCode, "ActiveBackups")
                    .StartAt(backupItem.NextBackupDate)
                    .WithSimpleSchedule(x => x
                        .WithIntervalInHours(backupItem.BackupInterval.Days * 24)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger.
                await scheduler.ScheduleJob(job, trigger);

                // Update the BackupItem to indicate that its BackupJob has been successfully queued.
                BackupInfoViewModel.SetBackupItemActive(backupItem.HashCode, true);

                BackupInfoViewModel.SaveConfig();

                //Debug.WriteLine("Job queued, saving config...");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Remove a backup job from the jobs pool with given BackupItem HashCode.
        /// </summary>
        /// <param name="backupItemHashCode"></param>
        public static async void RemoveBackupJob(string backupItemHashCode)
        {
            //Debug.WriteLine(string.Format("'{0}' removed at: {1}", backupItemHashCode.Substring(0, 5), DateTime.Now));
            // Grab the Scheduler instance from the Factory
            try
            {
                IScheduler scheduler = await _SchedulerFactory.GetScheduler();

                JobKey jobToCheck = new JobKey(backupItemHashCode, "ActiveBackups");

                if (await scheduler.CheckExists(jobToCheck))
                {
                    await scheduler.DeleteJob(jobToCheck);
                    BackupInfoViewModel.SetBackupItemActive(backupItemHashCode, false);
                }

                BackupInfoViewModel.SaveConfig();
                //Debug.WriteLine("Job de-queued, saving config...");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        #endregion

        #region Init and shutdown

        /// <summary>
        /// Initialize the scheduler for use.
        /// </summary>
        public static async void InitScheduler()
        {
            // Grab the Scheduler instance from the Factory
            IScheduler scheduler = await _SchedulerFactory.GetScheduler();
            // Start the scheduler.
            await scheduler.Start();
        }

        /// <summary>
        /// Shut clean up and shut down the schduler.
        /// </summary>
        public static async void ShutDownScheduler()
        {
            // Grab the Scheduler instance from the Factory
            IScheduler scheduler = await _SchedulerFactory.GetScheduler();
            Debug.WriteLine(string.Format("TaskManager shutting down at: {0}", DateTime.Now));
            await scheduler.Shutdown();
        }

        #endregion
    }
}
