﻿using BackItUp.Models;
using BackItUp.ViewModels.Commands;
using BackItUp.ViewModels.HashCodeGenerator;
using BackItUp.ViewModels.Serialization;
using BackItUp.ViewModels.TaskManagement;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackItUp.ViewModels
{
    internal class BackupInfoViewModel : INotifyPropertyChanged
    {
        private static ObservableCollection<BackupItem> _BackupInfo;
        private static ObservableCollection<BackupPeriodList> _BackupPeriodList;
        private static int _SelectedBackupItemIndex;
        private static BackupInfoViewModel _ActiveViewModel;

        #region Member Methods

        /// <summary>
        /// Constructor for the view model.
        /// </summary>
        public BackupInfoViewModel()
        {
            // Set a ref to self for static member usage.
            _ActiveViewModel = this;
            TaskManager.InitScheduler();

            // For testing purposes.
            //_BackupInfo = new ObservableCollection<BackupItem>();

            // Prep the backupinfo for consumption.
            InitBackupInfo();
            // Reactivate any previously active jobs from the serialized data.
            ToggleAllJobs();
            // Prep the list of backup periods for consumption.
            InitBackupPeriodList();
            // Prep the commands for use.
            DeleteItemCmd = new DeleteBackupItemCommand(this);
            AddItemCmd = new AddBackupItemCommand(this);
            SelectOriginFileDialogCmd = new SelectOriginFileDialogCommand(this);
            SelectOriginFolderDialogCmd = new SelectOriginFolderDialogCommand(this);
            SelectBackupFolderDialogCmd = new SelectBackupFolderDialogCommand(this);
            SaveConfigCmd = new SaveConfigCommand();
            LoadConfigCmd = new LoadConfigCommand(this);
            ResetConfigCmd = new ResetConfigCommand(this);

            // For testing purposes.
            //TestTasks();
        }

        /// <summary>
        /// For testing purposes.
        /// </summary>
        /// <returns></returns>
        private async Task TestTasks()
        {
            BackupItem testItem = new BackupItem
            {
                OriginPath = @"E:\Test Origin\test 1 2 3\",
                BackupPath = @"E:\Test Backup\",
                BackupInterval = TimeSpan.FromSeconds(10),
                NextBackupDate = DateTime.Now.AddSeconds(3)
            };
            testItem.HashCode = Hasher.StringHasher(testItem.OriginPath + testItem.BackupPath);

            _BackupInfo.Add(testItem);
            _BackupInfo[0].PropertyChanged += ModelPropertyChanged;
            await TaskManager.QueueBackupJob(testItem);


            testItem = new BackupItem()
            {
                OriginPath = @"E:\Test Origin\The Viewer.exe",
                BackupPath = @"E:\Test Backup\",
                BackupInterval = TimeSpan.FromSeconds(10),
                NextBackupDate = DateTime.Now.AddSeconds(4)
            };
            testItem.HashCode = Hasher.StringHasher(testItem.OriginPath + testItem.BackupPath);

            _BackupInfo.Add(testItem);
            _BackupInfo[1].PropertyChanged += ModelPropertyChanged;
            await TaskManager.QueueBackupJob(testItem);
        }

        /// <summary>
        /// Getter/setter for the BackupInfo collection.
        /// </summary>
        public ObservableCollection<BackupItem> BackupInfo
        {
            get
            {
                return _BackupInfo;
            }
            set
            {
                // Set value and trigger updates.
                _BackupInfo = value;
                OnPropertyChanged("BackupInfo");
            }
        }

        /// <summary>
        /// Gets the backup period list.
        /// </summary>
        public ObservableCollection<BackupPeriodList> BackupPeriodList
        {
            get
            {
                return _BackupPeriodList;
            }
        }

        /// <summary>
        /// Getter/setter for the currently selected row index of the datagrid.
        /// </summary>
        public int SelectedBackupItemIndex
        {
            get
            {
                return _SelectedBackupItemIndex;
            }
            set
            {
                _SelectedBackupItemIndex = value;
                OnPropertyChanged("SelectedBackupItemIndex");
            }
        }

        /// <summary>
        /// Check all BackupInfo items to see if they are valid.
        /// Rules:
        /// Do not allow empty path fields.
        /// Paths that do not exist are still valid.
        /// Do not worry about permissions issues.
        /// </summary>
        /// <returns>bool indicating validity of both paths</returns>
        public bool IsBackupInfoValid()
        {
            foreach(BackupItem item in BackupInfo)
            {
                // Check the origin and then backup paths. If any irregularities are found, then do not allow the user to save/apply the config.
                if(!(!string.IsNullOrWhiteSpace(item.OriginPath)
                    && item.OriginPath.IndexOfAny(Path.GetInvalidPathChars()) == -1
                    && Path.IsPathRooted(item.OriginPath)
                    && !Path.GetPathRoot(item.OriginPath).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) ||
                    !(!string.IsNullOrWhiteSpace(item.BackupPath)
                    && item.BackupPath.IndexOfAny(Path.GetInvalidPathChars()) == -1
                    && Path.IsPathRooted(item.BackupPath)
                    && !Path.GetPathRoot(item.BackupPath).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Commands

        public ICommand DeleteItemCmd { get; set; }
        public ICommand AddItemCmd { get; set; }
        public ICommand SelectOriginFileDialogCmd { get; set; }
        public ICommand SelectOriginFolderDialogCmd { get; set; }
        public ICommand SelectBackupFolderDialogCmd { get; set; }
        public ICommand SaveConfigCmd { get; set; }
        public ICommand LoadConfigCmd { get; set; }
        public ICommand ResetConfigCmd { get; set; }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Event Handlers

        public void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BackupFrequency" ||
                e.PropertyName == "BackupPeriod" ||
                e.PropertyName == "BackupTime")
            {
                HandleBackupDateTimeChanged();
            }
            if(e.PropertyName == "BackupEnabled")
            {
                ToggleJobBySelectedIndex();
            }
        }

        /// <summary>
        /// Update the NextBackupDate of the current item if a new frequency, period, or time of day is specified.
        /// </summary>
        private void HandleBackupDateTimeChanged()
        {
            BackupItem selectedItem = BackupInfo[SelectedBackupItemIndex];

            // Remove any backup jobs associated with the old hash.
            TaskManager.RemoveBackupJob(selectedItem.HashCode);

            // Calculate the number of days to add, then create the new DateTime object.
            int daysToAdd = selectedItem.BackupFrequency * selectedItem.BackupPeriod;

            DateTime newDateAndTime = new DateTime(selectedItem.LastBackupDate.Year,
                selectedItem.LastBackupDate.Month,
                selectedItem.LastBackupDate.Day,
                selectedItem.BackupTime.Hour,
                selectedItem.BackupTime.Minute,
                00).AddDays(daysToAdd);

            // Update the NextBackupDate with the new object/value and notify the UI.
            selectedItem.NextBackupDate = newDateAndTime;
            // Also update the BackupInterval.
            selectedItem.BackupInterval = new TimeSpan(
                daysToAdd,
                selectedItem.BackupTime.Hour,
                selectedItem.BackupTime.Minute,
                0
                );
            OnPropertyChanged("BackupInterval");
            OnPropertyChanged("NextBackupDate");

            // Call ToggleJobBySelectedIndex to re-queue the job if the BackEnabled is set to true.
            ToggleJobBySelectedIndex();
        }

        #endregion

        #region Initializers

        /// <summary>
        /// Default initializer.
        /// </summary>
        private void InitBackupInfo()
        {
            LoadConfig();
            foreach(BackupItem item in BackupInfo)
            {
                item.PropertyChanged += ModelPropertyChanged;
            }
            OnPropertyChanged("BackupList");
        }

        /// <summary>
        /// Default initializer for the period selection combobox.
        /// </summary>
        private void InitBackupPeriodList()
        {
            _BackupPeriodList = new ObservableCollection<BackupPeriodList>
            {
                new BackupPeriodList(1, "Day(s)"),
                new BackupPeriodList(7, "Week(s)"),
                new BackupPeriodList(30, "Month(s)")
            };
            OnPropertyChanged("BackupPeriodList");
        }

        #endregion

        #region Command Helper Methods

        /// <summary>
        /// Retrun a bool indicating whether or not the datagrid can delete the selected row.
        /// </summary>
        public bool CanDeleteItem
        {
            get
            {
                return _SelectedBackupItemIndex < _BackupInfo.Count;
            }
        }

        /// <summary>
        /// Launch a dialog window so that the user can select an origin file path.
        /// </summary>
        public void GetNewOriginFilePath()
        {
            string filePath = ShowSelectOriginFileDialog();

            if (!File.Exists(filePath))
                return;

            // Remove any backup jobs associated with the old hash.
            TaskManager.RemoveBackupJob(BackupInfo[SelectedBackupItemIndex].HashCode);

            BackupInfo[SelectedBackupItemIndex].OriginPath = filePath;

            UpdateItemHash();
        }

        /// <summary>
        /// Launch a dialog window so that the user can select an origin folder path.
        /// </summary>
        public void GetNewOriginFolderPath()
        {
            string folderPath = ShowSelectPathDialog();

            if (!Directory.Exists(folderPath))
                return;

            // Remove any backup jobs associated with the old hash.
            TaskManager.RemoveBackupJob(BackupInfo[SelectedBackupItemIndex].HashCode);

            if (!folderPath.EndsWith("\\"))
                folderPath += "\\";

            BackupInfo[SelectedBackupItemIndex].OriginPath = folderPath;

            UpdateItemHash();
        }

        /// <summary>
        /// Launch a dialog window so that the user can select a destination folder path.
        /// </summary>
        public void GetNewBackupFolderPath()
        {
            string folderPath = ShowSelectPathDialog();

            if (!Directory.Exists(folderPath))
                return;

            // Remove any backup jobs associated with the old hash.
            TaskManager.RemoveBackupJob(BackupInfo[SelectedBackupItemIndex].HashCode);

            if (!folderPath.EndsWith("\\"))
                folderPath += "\\";

            BackupInfo[SelectedBackupItemIndex].BackupPath = folderPath;

            UpdateItemHash();
        }

        /// <summary>
        /// Delete the selected BackupInfo item via index from the GUI from the collection if applicable.
        /// </summary>
        public void DeleteBackupItemBySelectedIndex()
        {
            if (_SelectedBackupItemIndex == -1)
                return;

            // Remove the selected row.
            TaskManager.RemoveBackupJob(_BackupInfo[_SelectedBackupItemIndex].HashCode);
            _BackupInfo.RemoveAt(_SelectedBackupItemIndex);
            SelectedBackupItemIndex = BackupInfo.Count - 1;
        }

        /// <summary>
        /// Delete the selected BackupInfo item via HashCode from the collection if applicable.
        /// If the code does not match anything in the list, then we have an orphan; stop the job.
        /// </summary>
        public static void DeleteBackupItemByHashCode(string hashCode)
        {
            Debug.WriteLine("Attempting to remove job with hash: " + hashCode);

            if (hashCode.Length != 64)
                return;

            BackupItem itemToRemove = null;

            foreach(BackupItem backupItem in _ActiveViewModel.BackupInfo)
            {
                Debug.WriteLine(string.Format("Checking against hash: ", backupItem.HashCode.Substring(0, 5)));
                if(backupItem.HashCode == hashCode)
                {
                    itemToRemove = backupItem;
                    break;
                }
            }

            // If the item exists, remove it.
            if(itemToRemove != null)
            {
                Debug.WriteLine("Removing item from list.");
                _ActiveViewModel.BackupInfo.Remove(itemToRemove);
                _ActiveViewModel.SelectedBackupItemIndex = _BackupInfo.Count - 1;
            }
        }

        /// <summary>
        /// Add a backup item to the BackupInfo collection.
        /// </summary>
        public void AddBackupItem()
        {
            BackupInfo.Add(new BackupItem());
            SelectedBackupItemIndex = BackupInfo.Count - 1;
            BackupInfo[SelectedBackupItemIndex].PropertyChanged += ModelPropertyChanged;
        }

        /// <summary>
        /// Command helper for saving the current BackupInfo.
        /// </summary>
        public static void SaveConfig()
        {
            //Debug.WriteLine("Saving config...");
            Serializer.SaveConfigToFile(_ActiveViewModel.BackupInfo);
        }

        /// <summary>
        /// Load the backup config file from the .dat file.
        /// </summary>
        public void LoadConfig()
        {
            BackupInfo = Serializer.LoadConfigFromFile();
            if (BackupInfo.Count == 0)
                AddBackupItem();
        }

        /// <summary>
        /// Discard the old BackupInfo collection and create a new one, then save it.
        /// </summary>
        public void ResetConfig()
        {
            BackupInfo = new ObservableCollection<BackupItem>();
            AddBackupItem();
            SaveConfig();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Updates the HashCode for the current item if the origin and backup fields are populated.
        /// If either is invalid, reset the hashCode on the item.
        /// </summary>
        /// <param name="index"></param>
        private void UpdateItemHash()
        {
            if(!string.IsNullOrWhiteSpace(BackupInfo[_SelectedBackupItemIndex].OriginPath) &&
               !string.IsNullOrWhiteSpace(BackupInfo[_SelectedBackupItemIndex].BackupPath))
            {
                //Debug.WriteLine(string.Format("Hashing: '{0}' and '{1}'", BackupInfo[_SelectedBackupItemIndex].OriginPath, BackupInfo[_SelectedBackupItemIndex].BackupPath));

                // Remove the hashcode associated with the current hash before we get rid of it.
                TaskManager.RemoveBackupJob(BackupInfo[_SelectedBackupItemIndex].HashCode);

                // Then calculate the new hash for the object.
                BackupInfo[_SelectedBackupItemIndex].HashCode = Hasher.StringHasher(
                    BackupInfo[_SelectedBackupItemIndex].OriginPath +
                    BackupInfo[_SelectedBackupItemIndex].BackupPath);

                // Check for duplicate origin and backup path items.
                if(!ReinitializeDuplicateBackups())
                {
                    // If there are no duplicates, then check if the item is enabled.
                    // If it is, we need to queue a new job with the new hash.
                    if(BackupInfo[_SelectedBackupItemIndex].BackupEnabled)
                    {
                        TaskManager.QueueBackupJob(BackupInfo[_SelectedBackupItemIndex]);
                    }
                }
            }
            else
            {
                BackupInfo[_SelectedBackupItemIndex].HashCode = "";
            }
        }

        /// <summary>
        /// Checks for and resets the current BackupItem if the same hash code already exists (same origin/backup paths as an existing item).
        /// </summary>
        /// <returns>Bool indicating if there was a duplicate.</returns>
        private bool ReinitializeDuplicateBackups()
        {
            if (BackupInfo.Count == 0)
                return false;

            string newHashCode = BackupInfo[BackupInfo.Count - 1].HashCode;

            for (int i = 0; i < BackupInfo.Count - 1; i++)
            {
                if(newHashCode == BackupInfo[i].HashCode)
                {
                    BackupInfo[_SelectedBackupItemIndex].LoadDefaultValues();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Update the NextBackupDate of the BackupItem according to HashCode.
        /// Increments the NextBackupDate by one BackupInterval of time.
        /// </summary>
        public static void UpdateNextBackupDate(string hashCode)
        {
            BackupItem itemToUpdate = null;

            foreach(BackupItem itm in _BackupInfo)
            {
                if(itm.HashCode == hashCode)
                {
                    itemToUpdate = itm;
                    break;
                }
            }

            if(itemToUpdate == null)
            {
                return;
            }

            // Calculate the number of days to add, then create the new DateTime object.
            int daysToAdd = itemToUpdate.BackupFrequency * itemToUpdate.BackupPeriod;
            DateTime newDateAndTime = new DateTime(itemToUpdate.NextBackupDate.Year,
                itemToUpdate.NextBackupDate.Month,
                itemToUpdate.NextBackupDate.Day,
                itemToUpdate.BackupTime.Hour,
                itemToUpdate.BackupTime.Minute,
                00).AddDays(daysToAdd);

            // Update the NextBackupDate with the new object/value and notify the UI.
            itemToUpdate.NextBackupDate = newDateAndTime;

            _ActiveViewModel.OnPropertyChanged("BackupInfo");
        }

        /// <summary>
        /// Checks the BackupInfo collection for an item with the given hash code.
        /// If it does not exist, then notify the TaskManager to stop the job with that hash code.
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns>Returns true if the hash code is an orphan.</returns>
        public static bool IsHashCodeOrphaned(string hashCode)
        {
            bool itemExists = false;

            foreach (BackupItem backupItem in _ActiveViewModel.BackupInfo)
            {
                if (backupItem.HashCode == hashCode)
                {
                    itemExists = true;
                    break;
                }
            }

            // If we did not find the item in the collection, then delete the job.
            if (!itemExists)
            {
                TaskManager.RemoveBackupJob(hashCode);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toggles the job associated to the BackupItem in BackupInfo[SelectedIndex].
        /// </summary>
        public static void ToggleJobBySelectedIndex()
        {
            BackupItem currentItem = _ActiveViewModel.BackupInfo[_SelectedBackupItemIndex];

            if(currentItem.BackupEnabled)
            {
                // We do not know how long ago the previous job was de-activated.
                // If the job is enabled and the NextBackupDate has passed, the copy job will run immediately.
                // To prevent this, recalculate the next backup date of the newly enabled item if its NextBackupDate is before the current date and time.
                // Keep going until the NextBackupDate is after the current date and time if necessary.
                while(currentItem.NextBackupDate < DateTime.Now)
                {
                    UpdateNextBackupDate(currentItem.HashCode);
                }
                TaskManager.QueueBackupJob(currentItem);
            }
            else
            {
                TaskManager.RemoveBackupJob(currentItem.HashCode);
            }
        }

        /// <summary>
        /// Toggles all jobs in the BackupInfo collection.
        /// Primarily used to reactivate all previous jobs on application start.
        /// </summary>
        public static void ToggleAllJobs()
        {
            foreach (BackupItem currentItem in _ActiveViewModel.BackupInfo)
            {
                if (currentItem.BackupEnabled)
                {
                    TaskManager.QueueBackupJob(currentItem);
                }
                else
                {
                    TaskManager.RemoveBackupJob(currentItem.HashCode);
                }
            }
        }

        #endregion

        #region TaskManager and Job Helpers

        /// <summary>
        /// Returns a list of all "enabled" BackupItem hash codes.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllActiveHashCodes()
        {
            List<string> hashCodeList = new List<string>();

            foreach (BackupItem item in _ActiveViewModel.BackupInfo)
            {
                if (item.BackupEnabled &&
                    !string.IsNullOrWhiteSpace(item.HashCode) &&
                    item.HashCode.Length == 64)
                {
                    hashCodeList.Add(item.HashCode);
                }
            }

            return hashCodeList;
        }

        /// <summary>
        /// Sets a property on a BackupItem indicating whether or not it has a running job associated with it.
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="isActive"></param>
        public static void SetBackupItemActive(string hashCode, bool isActive)
        {
            foreach (BackupItem item in _ActiveViewModel.BackupInfo)
            {
                if (!string.IsNullOrWhiteSpace(item.HashCode) &&
                    item.HashCode.Length == 64 &&
                    item.HashCode == hashCode)
                {
                    //Debug.WriteLine(string.Format("{0} is now {1}", hashCode, isActive == true ? "active" : "inactive"));
                    item.BackupActive = isActive;
                    break;
                }
            }
        }

        /// <summary>
        /// Changes the BackupItem.HasBeenBackedUp prop via associated hashCode to true,
        /// indicating that the item has been backed up at least once.
        /// </summary>
        /// <param name="hashCode"></param>
        public static void NotifyItemHasBeenBackedUp(string hashCode)
        {
            //Debug.WriteLine(string.Format("NotifyItemHasBeenBackedUp {0}", hashCode));

            foreach (BackupItem item in _ActiveViewModel.BackupInfo)
            {
                if (!string.IsNullOrWhiteSpace(item.HashCode) &&
                    item.HashCode.Length == 64 &&
                    item.HashCode == hashCode)
                {
                    // Update the last backup date
                    item.LastBackupDate = item.NextBackupDate;

                    item.HasBeenBackedUp = true;
                    break;
                }
            }
        }

        #endregion

        #region UI Business logic

        /// <summary>
        /// Launches a select file dialog and returns the selected path.
        /// </summary>
        /// <returns></returns>
        private string ShowSelectOriginFileDialog()
        {
            VistaOpenFileDialog selectFileDialog = new VistaOpenFileDialog
            {
                Multiselect = false,
                ValidateNames = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select a file"
            };
            selectFileDialog.ShowDialog();

            return selectFileDialog.FileName;
        }

        /// <summary>
        /// Launches a select path dialog and returns the selected path.
        /// </summary>
        private string ShowSelectPathDialog()
        {
            VistaFolderBrowserDialog selectFolderDialog = new VistaFolderBrowserDialog
            {
                Description = "Select a path",
                UseDescriptionForTitle = true
            };
            selectFolderDialog.ShowDialog();

            return selectFolderDialog.SelectedPath;
        }

        #endregion
    }
}
