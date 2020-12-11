using BackItUp.Models;
using BackItUp.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            DeleteItemCommand = new DeleteBackupItemCommand(this);
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

        public ICommand DeleteItemCommand { get; set; }

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

        #region UI Business logic
        public void DeleteSelectedBackupItem ()
        {
            // Remove the selected row.
            _BackupInfo.RemoveAt(_SelectedBackupItemIndex);
            // Trigger updates.
            OnPropertyChanged("BackupList");
            OnPropertyChanged("SelectedBackupItem");

            Debug.WriteLine("Delete command has been executed.");
        }

        #endregion
    }
}
