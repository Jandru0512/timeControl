using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using TimeControl.Common;
using TimeControl.Functions;
using Xunit;

namespace TimeControl.Tests
{
    public class ConsolidatedApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void List_Should_Return_200()
        {
            // Arrange
            MockCloudTableConsolidated mockConsolidate = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<Consolidated> records = TestFactory.GetConsolidatedsRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(records);

            // Act
            IActionResult response = await ConsolidatedApi.Consolidateds(request, mockConsolidate, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void Consolidate_Should_Return_200()
        {
            // Arrange
            MockCloudTableConsolidated mockConsolidate = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockCloudTableRecord mockRecord = new MockCloudTableRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            //List<Common.Record> records = TestFactory.GetRecordsRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest();

            // Act
            IActionResult response = await ConsolidatedApi.Consolidate(request, mockRecord, mockConsolidate, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
