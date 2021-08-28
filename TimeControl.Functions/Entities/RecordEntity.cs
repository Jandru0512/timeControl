using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimeControl.Functions
{
    public class RecordEntity : TableEntity
    {
        public int EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Type { get; set; }
        public bool Consolidated { get; set; }
    }
}
