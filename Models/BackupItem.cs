using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using static BackItUp.Models.Validation.BackupItemValidation;

namespace BackItUp.Models
{
    public class BackupItem : IDataErrorInfo, INotifyPropertyChanged, ISerializable
    {
        #region Properties

        private string _HashCode;
        public string HashCode
        {
            get
            {
                return _HashCode;
            }
            set
            {
                _HashCode = value;
                OnPropertyChanged("HashCode", value.ToString());
            }
        }

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

        private string _BackupFrequency;
        public string BackupFrequency
        {
            get
            {
                return _BackupFrequency.ToString();
            }
            set
            {
                if (value != null &&
                    value.ToString() == "")
                {
                    _BackupFrequency = "";
                }
                else
                {
                    _BackupFrequency = value.ToString();
                }
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

        private DateTime _BackupTime;
        public DateTime BackupTime
        {
            get
            {
                return _BackupTime;
            }
            set
            {
                _BackupTime = value;
                OnPropertyChanged("BackupTime", value.ToString());
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

        #endregion

        #region Member Methods

        public BackupItem()
        {
            LoadDefaultValues();
        }

        /// <summary>
        /// Loads the default values for a backupinfo item.
        /// </summary>
        public void LoadDefaultValues()
        {
            HashCode = "";
            OriginPath = "";
            BackupPath = "";
            LastBackupDate = DateTime.Now;
            BackupFrequency = "1";
            BackupPeriod = 1;
            BackupTime = DateTime.Now;
            NextBackupDate = DateTime.Now.AddDays(1);
            BackupEnabled = true;
        }

        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Debug.WriteLine(string.Format("BackupInfo {0} Updated.", propertyName));
        }
        private void OnPropertyChanged(string propertyName, string newVal)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //Debug.WriteLine(string.Format("BackupInfo {0} Updated to {1}.", propertyName, newVal));
        }
        #endregion

        #region IDataErrorInfo Members
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }
        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HashCode", HashCode);
            info.AddValue("OriginPath", OriginPath);
            info.AddValue("BackupPath", BackupPath);
            info.AddValue("LastBackupDate", LastBackupDate);
            info.AddValue("BackupFrequency", BackupFrequency);
            info.AddValue("BackupPeriod", BackupPeriod);
            info.AddValue("BackupTime", BackupTime);
            info.AddValue("NextBackupDate", NextBackupDate);
            info.AddValue("BackupEnabled", BackupEnabled);
        }

        /// <summary>
        /// Intialize with serialized data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public BackupItem(SerializationInfo info, StreamingContext context)
        {
            HashCode = (string)info.GetValue("HashCode", typeof(string));
            OriginPath = (string)info.GetValue("OriginPath", typeof(string));
            BackupPath = (string)info.GetValue("BackupPath", typeof(string));
            LastBackupDate = (DateTime)info.GetValue("LastBackupDate", typeof(DateTime));
            BackupFrequency = (string)info.GetValue("BackupFrequency", typeof(string));
            BackupPeriod = (int)info.GetValue("BackupPeriod", typeof(int));
            BackupTime = (DateTime)info.GetValue("BackupTime", typeof(DateTime));
            NextBackupDate = (DateTime)info.GetValue("NextBackupDate", typeof(DateTime));
            BackupEnabled = (bool)info.GetValue("BackupEnabled", typeof(bool));
        }

        #endregion

        #region Validation

        // Holds the names of the properties that need to be validated.
        static readonly string[] ValidatedProperties =
        {
            "OriginPath",
            "BackupPath",
            //"LastBackupDate",
            //"NextBackupDate",
            "BackupFrequency"
            //"BackupPeriod",
            //"BackupEnabled"
        };

        /// <summary>
        /// Goes through the properties and checks for all valid.
        /// Returns true if all properties are valid and false if any is invalid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperties)
                {
                    if(GetValidationError(property) != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks the given propery for validity.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "OriginPath":
                    {
                        error = ValidateOriginPath(OriginPath, BackupPath);
                        break;
                    }
                case "BackupPath":
                    {
                        error = ValidateBackupPath(OriginPath, BackupPath);
                        break;
                    }
                case "BackupFrequency":
                    {
                        error = ValidateBackupFrequency(BackupFrequency);
                        break;
                    }
            }

            return error;
        }
        #endregion
    }
}