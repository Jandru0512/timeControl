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
            log.LogInformation("Received a new record.");

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
                    Message = "The record must have a valid type."
                });
            }

            RecordEntity todoEntity = new RecordEntity
            {
                Consolidated = false,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = record.EmployeeId,
                ETag = "*",
                PartitionKey = "RECORD",
                RowKey = Guid.NewGuid().ToString(),
                Type = record.Type
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await recordTable.ExecuteAsync(addOperation);

            string message = "New record stored in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });
        }
    }
}
