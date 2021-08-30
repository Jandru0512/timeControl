using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TimeControl.Functions
{
    public class ConsolidatedEntity : TableEntity
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public int Minutes { get; set; }
    }
}
