using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            Common.Record recordRequest = TestFactory.GetRecordRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(recordRequest);

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
            Common.Record recordRequest = TestFactory.GetRecordRequest();
            Guid id = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(id, recordRequest);

            // Act
            IActionResult response = await TimeControlApi.Update(request, mockRecord, id.ToString(), logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
