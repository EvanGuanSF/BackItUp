using System;
using System.ComponentModel;
using System.Diagnostics;

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
                OnPropertyChanged("OriginPath", value.ToString());
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
                OnPropertyChanged("BackupPath", value.ToString());
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
                OnPropertyChanged("LastBackupDate", value.ToString());
            }
        }

        private int _BackupFrequency;
        public int BackupFrequency
        {
            get
            {
                return _BackupFrequency;
            }
            set
            {
                int temp = 1;
                try
                {
                    temp = (int)value;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                _BackupFrequency = temp;
                OnPropertyChanged("BackupFrequency", value.ToString());
            }
        }

        private int _BackupPeriod;
        public int BackupPeriod
        {
            get
            {
                return _BackupPeriod;
            }
            set
            {
                int temp = 1;
                try
                {
                    temp = (int)value;
                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                _BackupPeriod = temp;
                OnPropertyChanged("BackupPeriod", value.ToString());
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
                OnPropertyChanged("NextBackupDate", value.ToString());
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
                OnPropertyChanged("BackupEnabled", value.ToString());
            }
        }

        public BackupInfo()
        {
            OriginPath = "";
            BackupPath = "";
            LastBackupDate = DateTime.Now;
            BackupPeriod = 1;
            BackupFrequency = 1;
            NextBackupDate = DateTime.Now.AddDays(1);
            BackupEnabled = true;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine(string.Format("BackupInfo {0} Updated.", propertyName));
        }
        private void OnPropertyChanged(string propertyName, string newVal)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine(string.Format("BackupInfo {0} Updated to {1}.", propertyName, newVal));
        }
        #endregion
    }
}