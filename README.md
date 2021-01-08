# BackItUp
 File and folder backup utility. WPF with MVVM architectural pattern.
 
 Utilizes Ookii Dialogs WPF for file and folder selection dialog windows, Xceed Software's Extended WPF Toolkit™ for DateTime controls, RoboSharp for directory/file copy operations, Quartz.net for job scheduling, and Task Scheduler Managed Wrapper for run-on-startup Windows Scheduler task management.

# Usage
While program usage has been made as simple to use as possible, there are a few interactions that can be non-intuitive.
  - The frequency, period, and time can be set independently. Setting any one of these will automatically update the next backup date. However, the next backup date can be seperately updated without affecting frequency, period, or time (the interval between backups).
    - This means that if you set the interval of updates first, you can then select a date and time from the next backup date control. The interval will then be applied from the next time the backup runs according to next backup date.
  - Trying to set a time using the next backup date field that is prior to the current DateTime.Now will simply increment the next backup date via last backup date (or DateTime.Now if backup has never run) plus the interval that you have set without triggering a copy job.
  - The program will save its settings automatically any time valid changes are made to an existing backup item and whenever a new item is added with valid origin and backup paths.

# To Do
  - Add logging for job completions and errors.