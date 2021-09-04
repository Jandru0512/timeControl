using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace TimeControl.Functions
{
    public static class ConsolidatedJob
    {
        [FunctionName("ConsolidatedJob")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation($"Starting job to consolidate information at: {DateTime.Now}");

            // Records not consolidated yet
            List<RecordEntity> records = await NotConsolidated(recordTable);

            // Created registers
            int created = 0;
            // Updated Registers
            int updated = 0;

            // Employees to update
            List<int> employees = records.Select((record) => record.EmployeeId).Distinct().ToList();

            foreach (int employee in employees)
            {
                // Records by employee
                List<RecordEntity> employeeRecords = records.Where((record) => record.EmployeeId == employee).ToList();
                List<string> result = new List<string>();

                // if employee haven't check out delete the last in
                if (employeeRecords.Count % 2 == 1)
                {
                    employeeRecords.RemoveAt(employeeRecords.Count - 1);
                }

                foreach (RecordEntity record in employeeRecords)
                {
                    DateTime recordDate = record.CreatedAt.Date.ToUniversalTime();

                    // if date already has been storaged continue
                    if (result.FirstOrDefault((date) => date.Equals(recordDate.ToString())) != null)
                    {
                        continue;
                    }

                    // Consolidated by employee and date
                    ConsolidatedEntity consolidated = await Consolidated(consolidatedTable, employee, recordDate);
                    List<RecordEntity> timeRecords = records.Where((x) => x.EmployeeId == employee && x.CreatedAt.Date == recordDate).ToList();

                    // reduce records getting minutes
                    int minutes = timeRecords.Aggregate(new string[] { "", "0" }, (acum, row) => { return GetMinutes(acum, row); }, (acum) => int.Parse(acum[1]));

                    // Write into consolidated table
                    if (consolidated == null)
                    {
                        await CreateConsolidated(consolidatedTable, created, employee, recordDate, minutes);
                        created++;
                    }
                    else
                    {
                        await UpdateConsolidated(consolidatedTable, consolidated, minutes);
                        updated++;
                    }

                    await UpdateRecords(recordTable, timeRecords);

                    // Add to processed list
                    result.Add(recordDate.ToString());
                }
            }

            log.LogInformation($"{created} new records have been consolidated and {updated} have been updated.");
        }

        private static async Task UpdateRecords(CloudTable recordTable, List<RecordEntity> timeRecords)
        {
            foreach (RecordEntity timeRecord in timeRecords)
            {
                // Update consolidated propperty
                timeRecord.Consolidated = true;

                TableOperation update = TableOperation.Replace(timeRecord);
                await recordTable.ExecuteAsync(update);
            }
        }

        private static async Task UpdateConsolidated(CloudTable consolidatedTable, ConsolidatedEntity consolidated, int minutes)
        {
            consolidated.Minutes += minutes;
            TableOperation update = TableOperation.Replace(consolidated);

            await consolidatedTable.ExecuteAsync(update);
        }

        private static async Task<int> CreateConsolidated(CloudTable consolidatedTable, int created, int employee, DateTime recordDate, int minutes)
        {
            TableOperation create = TableOperation.Insert(new ConsolidatedEntity
            {
                Date = recordDate,
                ETag = "*",
                EmployeeId = employee,
                Minutes = minutes,
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString()
            });

            await consolidatedTable.ExecuteAsync(create);
            created++;
            return created;
        }

        private static string[] GetMinutes(string[] acum, RecordEntity row)
        {
            if (row.Type == (int)RecordTypes.In)
            {
                return new string[] { row.CreatedAt.ToString(), acum[1] };
            }

            DateTime date = DateTime.Parse(acum[0]);
            int counter = int.Parse(acum[1]);

            TimeSpan time = row.CreatedAt - date;
            counter += (int)time.TotalMinutes;

            return new string[] { "", counter.ToString() };
        }

        private static async Task<ConsolidatedEntity> Consolidated(CloudTable consolidatedTable, int employee, DateTime recordDate)
        {
            string consolidatedDateFilter = TableQuery.GenerateFilterConditionForDate(nameof(ConsolidatedEntity.Date), QueryComparisons.GreaterThanOrEqual, recordDate);
            string consolidatedEmployeeFilter = TableQuery.GenerateFilterConditionForInt(nameof(ConsolidatedEntity.EmployeeId), QueryComparisons.Equal, employee);

            TableQuery<ConsolidatedEntity> consolidatedQuery = new TableQuery<ConsolidatedEntity>().Where(TableQuery.CombineFilters(consolidatedEmployeeFilter, TableOperators.And, consolidatedDateFilter));
            TableQuerySegment<ConsolidatedEntity> consolidateds = await consolidatedTable.ExecuteQuerySegmentedAsync(consolidatedQuery, null);
            return consolidateds.FirstOrDefault();
        }

        private static async Task<List<RecordEntity>> NotConsolidated(CloudTable recordTable)
        {
            string recordFilter = TableQuery.GenerateFilterConditionForBool(nameof(RecordEntity.Consolidated), QueryComparisons.Equal, false);
            TableQuery<RecordEntity> recordQuery = new TableQuery<RecordEntity>().Where(recordFilter);
            TableQuerySegment<RecordEntity> records = await recordTable.ExecuteQuerySegmentedAsync(recordQuery, null);
            return records.OrderBy((x) => x.CreatedAt).ToList();
        }
    }
}
