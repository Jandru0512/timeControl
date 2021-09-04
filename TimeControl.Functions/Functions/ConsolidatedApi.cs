using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using TimeControl.Common;

namespace TimeControl.Functions
{
    public static class ConsolidatedApi
    {
        [FunctionName(nameof(Consolidateds))]
        public static async Task<IActionResult> Consolidateds(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated/{date}")] HttpRequest req,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            string date,
            ILogger log)
        {
            log.LogInformation("Listing consolidateds.");

            // All consolidateds ignore date, searching by date in utc
            string min = TableQuery.GenerateFilterConditionForDate(nameof(ConsolidatedEntity.Date), QueryComparisons.GreaterThanOrEqual, DateTime.Parse(date).ToUniversalTime().Date);
            string max = TableQuery.GenerateFilterConditionForDate(nameof(ConsolidatedEntity.Date), QueryComparisons.LessThanOrEqual, DateTime.Parse(date).Date.ToUniversalTime().AddDays(1));
            TableQuery<ConsolidatedEntity> query = new TableQuery<ConsolidatedEntity>().Where(TableQuery.CombineFilters(min, TableOperators.And, max));
            TableQuerySegment<ConsolidatedEntity> records = await consolidatedTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all consolidated registers.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = records
            });
        }

        [FunctionName(nameof(Consolidate))]
        public static async Task<IActionResult> Consolidate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "consolidated")] HttpRequest req,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation("Executiong consolidated job.");

            await ConsolidatedJob.Run(null, recordTable, consolidatedTable, log);

            string message = "Consolidated executed succesfully.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message
            });
        }
    }
}
