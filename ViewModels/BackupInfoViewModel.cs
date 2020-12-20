using BackItUp.Models;
using BackItUp.ViewModels.Serialization;
using BackItUp.ViewModels.Commands;
using BackItUp.ViewModels.HelperMethods;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace BackItUp.ViewModels
{
    internal class BackupInfoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BackupItem> _BackupInfo;
        private ObservableCollection<BackupPeriodList> _BackupPeriodList;
        private int _SelectedBackupItemIndex;

        #region Member Methods

        /// <summary>
        /// Constructor for the view model.
        /// </summary>
        public BackupInfoViewModel()
        {
            // Prep the backupinfo for consumption.
            InitBackupInfo();
            // Prep the list of backup periods for consumption.
            InitBackupPeriodList();
            // Prep the commands for use.
            DeleteItemCmd = new DeleteBackupItemCommand(this);
            AddItemCmd = new AddBackupItemCommand(this);
            SelectOriginFileDialogCmd = new SelectOriginFileDialogCommand(this);
            SelectOriginFolderDialogCmd = new SelectOriginFolderDialogCommand(this);
            SelectBackupFolderDialogCmd = new SelectBackupFolderDialogCommand(this);
            SaveAndApplyConfigCmd = new SaveApplyConfigCommand(this);
            LoadConfigCmd = new LoadConfigCommand(this);
            ResetConfigCmd = new ResetConfigCommand(this);
        }

        /// <summary>
        /// Gets the backup information collection.
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
        /// <returns></returns>
        public bool IsBackupInfoValid()
        {
            foreach(BackupItem item in BackupInfo)
            {
                // Check the origin and then backup paths. If any irregularities are found, then do not allow the user to save/apply the config.
                if(!(!String.IsNullOrWhiteSpace(item.OriginPath)
                    && item.OriginPath.IndexOfAny(Path.GetInvalidPathChars()) == -1
                    && Path.IsPathRooted(item.OriginPath)
                    && !Path.GetPathRoot(item.OriginPath).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)) ||
                    !(!String.IsNullOrWhiteSpace(item.BackupPath)
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
        public ICommand SaveAndApplyConfigCmd { get; set; }
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
        }

        /// <summary>
        /// Update the NextBackupDate of the current item if a new frequency, perion, or time of day is specified.
        /// </summary>
        private void HandleBackupDateTimeChanged()
        {
            // Calculate the number of days to add, then create the new DateTime object.
            int daysToAdd = int.Parse(BackupInfo[SelectedBackupItemIndex].BackupFrequency) * BackupInfo[SelectedBackupItemIndex].BackupPeriod;
            DateTime newDateAndTime = new DateTime(BackupInfo[SelectedBackupItemIndex].LastBackupDate.Year,
                BackupInfo[SelectedBackupItemIndex].LastBackupDate.Month,
                BackupInfo[SelectedBackupItemIndex].LastBackupDate.Day,
                BackupInfo[SelectedBackupItemIndex].BackupTime.Hour,
                BackupInfo[SelectedBackupItemIndex].BackupTime.Minute,
                00).AddDays(daysToAdd);

            // Update the NextBackupDate with the new object/value and notify the UI.
            BackupInfo[SelectedBackupItemIndex].NextBackupDate = newDateAndTime;
            OnPropertyChanged("NextBackupDate");
        }

        #endregion

        #region Initializers

        private void InitBackupInfo()
        {
            LoadConfig();
            OnPropertyChanged("BackupList");
        }

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
        /// Indicates whether or not the datagrid can delete the row.
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

            BackupInfo[SelectedBackupItemIndex].BackupPath = folderPath;

            UpdateItemHash();
        }

        /// <summary>
        /// Delete the selected BackupInfo item from the collection if appicable.
        /// </summary>
        public void DeleteSelectedBackupItem()
        {
            if (_SelectedBackupItemIndex == -1)
                return;

            // Remove the selected row.
            _BackupInfo.RemoveAt(_SelectedBackupItemIndex);
            // Trigger updates.
            SelectedBackupItemIndex = BackupInfo.Count - 1;
            OnPropertyChanged("BackupList");
            OnPropertyChanged("SelectedBackupItem");
        }

        /// <summary>
        /// Adds a backup item to the backupitem collection.
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
        public void SaveAndApplyConfig()
        {
            Serializer.SaveConfigToFile(BackupInfo);
        }

        /// <summary>
        /// Attempt to load the config file from the .dat file.
        /// </summary>
        public void LoadConfig()
        {
            BackupInfo = Serializer.LoadConfigFromFile();
            if (BackupInfo.Count == 0)
                BackupInfo.Add(new BackupItem());
        }

        public void ResetConfig()
        {
            BackupInfo = new ObservableCollection<BackupItem>();
            BackupInfo.Add(new BackupItem());
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Updates the HashCode for the current item if the origin and backup fields are populated.
        /// </summary>
        /// <param name="index"></param>
        private void UpdateItemHash()
        {
            if(!string.IsNullOrWhiteSpace(BackupInfo[_SelectedBackupItemIndex].OriginPath) &&
               !string.IsNullOrWhiteSpace(BackupInfo[_SelectedBackupItemIndex].BackupPath))
            {
                BackupInfo[_SelectedBackupItemIndex].HashCode = Hasher.StringHasher(BackupInfo[_SelectedBackupItemIndex].OriginPath + BackupInfo[_SelectedBackupItemIndex].OriginPath);
                ReinitializeDuplicateBackups();
            }
        }

        /// <summary>
        /// Resets the current backinfo item if the same hash code already exists (same origin/backup paths as an existing item).
        /// </summary>
        /// <param name="newHashCode"></param>
        private void ReinitializeDuplicateBackups()
        {
            if (BackupInfo.Count == 0)
                return;

            string newHashCode = BackupInfo[BackupInfo.Count - 1].HashCode;

            for (int i = 0; i < BackupInfo.Count - 1; i++)
            {
                if(newHashCode == BackupInfo[i].HashCode)
                {
                    BackupInfo[_SelectedBackupItemIndex].LoadDefaultValues();
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
            VistaOpenFileDialog selectFileDialog = new VistaOpenFileDialog();
            selectFileDialog.Multiselect = false;
            selectFileDialog.ValidateNames = true;
            selectFileDialog.AddExtension = true;
            selectFileDialog.CheckFileExists = true;
            selectFileDialog.CheckPathExists = true;
            selectFileDialog.Title = "Select a file";
            selectFileDialog.ShowDialog();

            return selectFileDialog.FileName;
        }

        /// <summary>
        /// Launch a dialog window so that the user can select a path.
        /// </summary>
        private string ShowSelectPathDialog()
        {
            VistaFolderBrowserDialog selectFolderDialog = new VistaFolderBrowserDialog();
            selectFolderDialog.Description = "Select a path";
            selectFolderDialog.UseDescriptionForTitle = true;
            selectFolderDialog.ShowDialog();

            return selectFolderDialog.SelectedPath;
        }

        #endregion
    }
}
