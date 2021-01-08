using System.Collections.Generic;

namespace BackItUp.Models
{
    /// <summary>
    /// Holds key/value pairs indicating the number of days (key) per period (value).
    /// </summary>
    public static class BackupItemStatusCodePairs
    {
        public enum StatusCodes : int
        {
            UNQUEUED,
            QUEUED,
            RUNNING,
            ERROR
        };

        public static KeyValuePair<int, string>[] GetBackupItemStatusCodePairs()
        {
            return new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(0, "Un-queued"),
                new KeyValuePair<int, string>(1, "Queued"),
                new KeyValuePair<int, string>(2, "Running"),
                new KeyValuePair<int, string>(4, "Error")
            };
        }
    }
}
