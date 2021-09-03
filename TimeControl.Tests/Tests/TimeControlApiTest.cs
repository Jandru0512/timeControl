using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using TimeControl.Functions;
using Xunit;

namespace TimeControl.Tests
{
    public class TimeControlApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void Create_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Record record = TestFactory.GetRecordRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(record);

            // Act
            IActionResult response = await TimeControlApi.Create(request, mockRecord, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void Update_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Record record = TestFactory.GetRecordRequest();
            Guid id = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(id, record);

            // Act
            IActionResult response = await TimeControlApi.Update(request, mockRecord, id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void List_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Common.Record> records = TestFactory.GetRecordsRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(records);

            // Act
            IActionResult response = await TimeControlApi.List(request, mockRecord, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void Get_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Record record = TestFactory.GetRecordRequest();
            Guid id = Guid.NewGuid();

            TableOperation findOperation = TableOperation.Retrieve<RecordEntity>("RECORD", id.ToString());
            TableResult operation = await mockRecord.ExecuteAsync(findOperation);
            RecordEntity entity = (RecordEntity)operation.Result;

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(id, record);

            // Act
            IActionResult response = TimeControlApi.Get(request, entity, id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void Delete_Should_Return_200()
        {
            // Arrange
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Common.Record record = TestFactory.GetRecordRequest();
            Guid id = Guid.NewGuid();

            TableOperation findOperation = TableOperation.Retrieve<RecordEntity>("RECORD", id.ToString());
            TableResult operation = await mockRecord.ExecuteAsync(findOperation);
            RecordEntity entity = (RecordEntity)operation.Result;

            DefaultHttpRequest request = TestFactory.CreateHttpRequest(id, record);

            // Act
            IActionResult response = await TimeControlApi.Delete(request, entity, mockRecord, id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
