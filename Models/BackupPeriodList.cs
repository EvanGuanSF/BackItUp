using System.Collections.Generic;
using System.Diagnostics;

namespace BackItUp.Models
{
    /// <summary>
    /// Holds a key/value pair indicating the number of days (value) per period (key).
    /// </summary>
    public class BackupPeriodList
    {
        public int PeriodKey { get; set; }
        public string PeriodValue { get; set; }
        public BackupPeriodList(int key, string value)
        {
            PeriodKey = key;
            PeriodValue = value;
            //Debug.WriteLine(string.Format("Added {0} = {1}", key, value));
        }
    }
}
