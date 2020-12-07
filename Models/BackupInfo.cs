using System;
using System.ComponentModel;

namespace BackItUp.Models
{
    public class BackupInfo : INotifyPropertyChanged
    {
        private string _OriginPath;
        public string OriginPath
        {
            get
            {
                return _OriginPath;
            }
            set
            {
                _OriginPath = value;
                OnPropertyChanged("OriginPath");
            }
        }

        private string _BackupPath;
        public string BackupPath
        {
            get
            {
                return _BackupPath;
            }
            set
            {
                _BackupPath = value;
                OnPropertyChanged("BackupPath");
            }
        }

        private DateTime _LastBackupDate;
        public DateTime LastBackupDate
        {
            get
            {
                return _LastBackupDate;
            }
            set
            {
                _LastBackupDate = value;
                OnPropertyChanged("LastBackupDate");
            }
        }

        private string _BackupFrequencyMult;
        public string BackupFrequencyMult
        {
            get
            {
                return _BackupFrequencyMult;
            }
            set
            {
                _BackupFrequencyMult = value;
                OnPropertyChanged("BackupFrequencyMult");
            }
        }

        private string _BackupFrequency;
        public string BackupFrequency
        {
            get
            {
                return _BackupFrequency;
            }
            set
            {
                _BackupFrequency = value;
                OnPropertyChanged("BackupFrequency");
            }
        }

        private DateTime _NextBackupDate;
        public DateTime NextBackupDate
        {
            get
            {
                return _NextBackupDate;
            }
            set
            {
                _NextBackupDate = value;
                OnPropertyChanged("NextBackupDate");
            }
        }

        private bool _BackupEnabled;
        public bool BackupEnabled
        {
            get
            {
                return _BackupEnabled;
            }
            set
            {
                _BackupEnabled = value;
                OnPropertyChanged("BackupEnabled");
            }
        }

        public BackupInfo()
        {
            OriginPath = "";
            BackupPath = "";
            LastBackupDate = DateTime.Now;
            BackupFrequency = "";
            BackupFrequencyMult = "";
            NextBackupDate = DateTime.Now.AddDays(1);
            BackupEnabled = true;
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
