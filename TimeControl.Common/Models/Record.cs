using System;

namespace TimeControl.Common
{
    public class Record
    {
        public int EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Type { get; set; }
        public bool Consolidated { get; set; }
    }
}
