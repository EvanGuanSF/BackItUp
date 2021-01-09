using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using static BackItUp.Models.Validation.BackupItemValidation;
using static BackItUp.Models.BackupItemStatusCodePairs;

namespace BackItUp.Models
{
    [Serializable()]
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

        private bool _HasBeenBackedUp;
        public bool HasBeenBackedUp
        {
            get
            {
                return _HasBeenBackedUp;
            }
            set
            {
                _HasBeenBackedUp = value;
                OnPropertyChanged("HasBeenBackedUp", value.ToString());
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

        private int _BackupFrequency;
        public int BackupFrequency
        {
            get
            {
                return _BackupFrequency;
            }
            set
            {
                _BackupFrequency = value;
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
                _BackupPeriod = value;
                OnPropertyChanged("BackupPeriod", value.ToString());
            }
        }

        private TimeSpan _BackupInterval;
        public TimeSpan BackupInterval
        {
            get
            {
                return _BackupInterval;
            }
            set
            {
                _BackupInterval = value;
                OnPropertyChanged("BackupInterval", value.ToString());
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

        [field: NonSerialized]
        private int _StatusCode;
        public int StatusCode
        {
            get
            {
                return _StatusCode;
            }
            set
            {
                _StatusCode = value;
                StatusMessage = GetBackupItemStatusCodePairs()[value].Value;
                StatusColor = GetBackupItemStatusCodeColors()[value].Value;
                OnPropertyChanged("StatusCode", value.ToString());
            }
        }

        [field: NonSerialized]
        private string _StatusMessage;
        public string StatusMessage
        {
            get
            {
                return _StatusMessage;
            }
            set
            {
                _StatusMessage = value;
                OnPropertyChanged("StatusMessage", value.ToString());
            }
        }

        [field: NonSerialized]
        private string _StatusColor;
        public string StatusColor
        {
            get
            {
                return _StatusColor;
            }
            set
            {
                _StatusColor = value;
                OnPropertyChanged("StatusColor", value.ToString());
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
            HasBeenBackedUp = false;
            OriginPath = "";
            BackupPath = "";
            LastBackupDate = DateTime.Now;
            BackupFrequency = 1;
            BackupPeriod = 1;
            BackupInterval = TimeSpan.FromDays(1);
            BackupTime = DateTime.Now;
            NextBackupDate = DateTime.Now.AddDays(1);
            BackupEnabled = true;
            StatusCode = (int)StatusCodes.UNQUEUED;
        }

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}\n{9}\n{10}\n{11}",
                HashCode,
                HasBeenBackedUp,
                OriginPath,
                BackupPath,
                LastBackupDate,
                BackupFrequency,
                BackupPeriod,
                BackupInterval,
                BackupTime,
                NextBackupDate,
                BackupEnabled,
                StatusCode
                );
        }

        #endregion

        #region INotifyPropertyChanged Members
        [field: NonSerialized]
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

        /// <summary>
        /// Serialize class data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("HashCode", HashCode);
            info.AddValue("HasBeenBackedUp", HasBeenBackedUp);
            info.AddValue("OriginPath", OriginPath);
            info.AddValue("BackupPath", BackupPath);
            info.AddValue("LastBackupDate", LastBackupDate);
            info.AddValue("BackupFrequency", BackupFrequency);
            info.AddValue("BackupPeriod", BackupPeriod);
            info.AddValue("BackupTime", BackupTime);
            info.AddValue("BackupInterval", BackupInterval);
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
            HasBeenBackedUp = (bool)info.GetValue("HasBeenBackedUp", typeof(bool));
            OriginPath = (string)info.GetValue("OriginPath", typeof(string));
            BackupPath = (string)info.GetValue("BackupPath", typeof(string));
            LastBackupDate = (DateTime)info.GetValue("LastBackupDate", typeof(DateTime));
            BackupFrequency = (int)info.GetValue("BackupFrequency", typeof(int));
            BackupPeriod = (int)info.GetValue("BackupPeriod", typeof(int));
            BackupTime = (DateTime)info.GetValue("BackupTime", typeof(DateTime));
            BackupInterval = (TimeSpan)info.GetValue("BackupInterval", typeof(TimeSpan));
            NextBackupDate = (DateTime)info.GetValue("NextBackupDate", typeof(DateTime));
            BackupEnabled = (bool)info.GetValue("BackupEnabled", typeof(bool));
            StatusCode = (int)StatusCodes.UNQUEUED;

            //Debug.WriteLine("");
            //Debug.WriteLine(ToString());
            //Debug.WriteLine("");
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
                        error = ValidateOriginPath(OriginPath);
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