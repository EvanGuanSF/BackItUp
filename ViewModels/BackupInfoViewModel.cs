using BackItUp.Models;
using BackItUp.ViewModels.Commands;
using Ookii.Dialogs.Wpf;
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
            SelectOriginFileDialogCmd = new SelectOriginFileDialogCommand(this);
            SelectOriginFolderDialogCmd = new SelectOriginFolderDialogCommand(this);
            SelectBackupFolderDialogCmd = new SelectBackupFolderDialogCommand(this);
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

                Debug.WriteLine("Number of BackupInfo items: ", _BackupInfo.Count);
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

                Debug.WriteLine("New selected row index: " + _SelectedBackupItemIndex.ToString());
            }
        }

        #region Commands

        public ICommand DeleteItemCmd { get; set; }
        public ICommand SelectOriginFileDialogCmd { get; set; }
        public ICommand SelectOriginFolderDialogCmd { get; set; }
        public ICommand SelectBackupFolderDialogCmd { get; set; }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Initializers
        private void InitBackupInfo()
        {
            _BackupInfo = new ObservableCollection<BackupItem>();
            OnPropertyChanged("BackupList");

            Debug.WriteLine("BackupList initilaized.");
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

            Debug.WriteLine("BackupPeriodList initilaized.");
        }
        #endregion

        #region Command Helper Methods

        /// <summary>
        /// Launch a dialog window so that the user can select an origin file path.
        /// </summary>
        public void GetNewOriginFilePath()
        {
            //Debug.WriteLine("GetOriginFilePath");
            //Debug.WriteLine(string.Format("Count: {0} Index: {1}", BackupInfo.Count, SelectedBackupItemIndex));

            string filePath = ShowSelectOriginFileDialog();

            if (!File.Exists(filePath))
                return;

            if (BackupInfo.Count == 0)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = 0;
            }
            else if (BackupInfo.Count == SelectedBackupItemIndex)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = BackupInfo.Count - 1;
            }

            BackupInfo[SelectedBackupItemIndex].OriginPath = filePath;
            Debug.WriteLine(filePath);
        }

        /// <summary>
        /// Launch a dialog window so that the user can select an origin folder path.
        /// </summary>
        public void GetNewOriginFolderPath()
        {
            //Debug.WriteLine("GetOriginFolderPath");
            //Debug.WriteLine(string.Format("Count: {0} Index: {1}", BackupInfo.Count, SelectedBackupItemIndex));

            string folderPath = ShowSelectPathDialog();

            if (!Directory.Exists(folderPath))
                return;

            if (BackupInfo.Count == 0)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = 0;
            }
            else if (BackupInfo.Count == SelectedBackupItemIndex)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = BackupInfo.Count - 1;
            }

            BackupInfo[SelectedBackupItemIndex].OriginPath = folderPath;
            Debug.WriteLine(folderPath);
        }

        /// <summary>
        /// Launch a dialog window so that the user can select a destination folder path.
        /// </summary>
        public void GetNewBackupFolderPath()
        {
            //Debug.WriteLine("GetBackupFolderPath");
            //Debug.WriteLine(string.Format("Count: {0} Index: {1}", BackupInfo.Count, SelectedBackupItemIndex));

            string folderPath = ShowSelectPathDialog();

            if (!Directory.Exists(folderPath))
                return;

            if (BackupInfo.Count == 0)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = 0;
            }
            else if (BackupInfo.Count == SelectedBackupItemIndex)
            {
                BackupInfo.Add(new BackupItem());
                SelectedBackupItemIndex = BackupInfo.Count - 1;
            }

            BackupInfo[SelectedBackupItemIndex].BackupPath = folderPath;
            Debug.WriteLine(folderPath);
        }

        #endregion

        #region UI Business logic
        /// <summary>
        /// Delete the selected BackupInfo item from the collection if appicable.
        /// </summary>
        public void DeleteSelectedBackupItem ()
        {
            // Remove the selected row.
            _BackupInfo.RemoveAt(_SelectedBackupItemIndex);
            // Trigger updates.
            OnPropertyChanged("BackupList");
            OnPropertyChanged("SelectedBackupItem");

            Debug.WriteLine("Delete command has been executed.");
        }

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

            Debug.WriteLine(selectFolderDialog.SelectedPath);

            return selectFolderDialog.SelectedPath;
        }

        #endregion
    }
}
