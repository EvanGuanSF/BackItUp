using System.Collections.Generic;

namespace BackItUp.Models
{
    class BackupFrequencyList : List<string>
    {
        public BackupFrequencyList()
        {
            Add("Day(s)");
            Add("Week(s)");
            Add("Month(s)");
        }
    }
}
