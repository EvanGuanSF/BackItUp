using BackItUp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BackItUp.ViewModels
{
    internal class BackupInfoViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BackupInfo> _BackupInfo;

        public BackupInfoViewModel()
        {
            _BackupInfo = new ObservableCollection<BackupInfo>();
            OnPropertyChanged("BackupList");
        }

        public ObservableCollection<BackupInfo> BackupInfo
        {
            get
            {
                return _BackupInfo;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
}
}
