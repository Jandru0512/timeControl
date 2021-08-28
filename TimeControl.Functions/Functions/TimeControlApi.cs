using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using TimeControl.Common;

namespace TimeControl.Functions
{
    public static class TimeControlApi
    {
        [FunctionName(nameof(Create))]
        public static async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "timeControl")] HttpRequest req,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            ILogger log)
        {
            log.LogInformation("Received a new time record.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Record record = JsonConvert.DeserializeObject<Record>(requestBody);

            if (record.EmployeeId == 0)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Invalid employee id."
                });
            }

            if (!Enum.IsDefined(typeof(RecordTypes), record.Type))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The time record must have a valid type."
                });
            }

            RecordEntity recordEntity = new RecordEntity
            {
                Consolidated = false,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = record.EmployeeId,
                ETag = "*",
                PartitionKey = "RECORD",
                RowKey = Guid.NewGuid().ToString(),
                Type = record.Type
            };

            TableOperation addOperation = TableOperation.Insert(recordEntity);
            await recordTable.ExecuteAsync(addOperation);

            string message = "New time record stored in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordEntity
            });
        }

        [FunctionName(nameof(Update))]
        public static async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "timeControl/{id}")] HttpRequest req,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Received an update for {id} time record.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Record record = JsonConvert.DeserializeObject<Record>(requestBody);

            // Validate record id
            TableOperation findOperation = TableOperation.Retrieve<RecordEntity>("RECORD", id);
            TableResult findResult = await recordTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Time record not found."
                });
            }

            // Update entity
            RecordEntity recordEntity = (RecordEntity)findResult.Result;
            recordEntity.Consolidated = record.Consolidated;
            if (record.EmployeeId != 0)
            {
                recordEntity.EmployeeId = record.EmployeeId;
            }

            if (Enum.IsDefined(typeof(RecordTypes), record.Type))
            {
                recordEntity.Type = record.Type;
            }

            TableOperation updateOperation = TableOperation.Replace(recordEntity);
            await recordTable.ExecuteAsync(updateOperation);

            string message = $"Time record: {id} updated in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = recordEntity
            });
        }


        [FunctionName(nameof(List))]
        public static async Task<IActionResult> List(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "timeControl")] HttpRequest req,
            [Table("record", Connection = "AzureWebJobsStorage")] CloudTable recordTable,
            ILogger log)
        {
            log.LogInformation("List records.");

            TableQuery<RecordEntity> query = new TableQuery<RecordEntity>();
            TableQuerySegment<RecordEntity> records = await recordTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all time records.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = records
            });
        }
    }
}
