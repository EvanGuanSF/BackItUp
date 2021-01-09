using System.Collections.Generic;

namespace BackItUp.Models
{
    /// <summary>
    /// Holds key/value pairs indicating the number of days (key) per period (value).
    /// </summary>
    public static class BackupPeriodPairs
    {
        public static KeyValuePair<int, string>[] GetBackupPeriodPairs()
        {
            return new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(1, "Day(s)"),
                new KeyValuePair<int, string>(7, "Week(s)"),
                new KeyValuePair<int, string>(30, "Month(s)")
            };
        }
    }
}
