using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TimeControl.Functions;

namespace TimeControl.Tests
{
    public class MockCloudTableRecord : CloudTable
    {
        public MockCloudTableRecord(Uri tableAddress) : base(tableAddress)
        {
        }

        public MockCloudTableRecord(Uri tableAbsoluteUri, StorageCredentials credentials) : base(tableAbsoluteUri, credentials)
        {
        }

        public MockCloudTableRecord(StorageUri tableAddress, StorageCredentials credentials) : base(tableAddress, credentials)
        {
        }

        public override async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            return await Task.FromResult(new TableResult
            {
                HttpStatusCode = 200,
                Result = TestFactory.GetRecordEntity()
            });
        }

        //public override async Task<TableQuerySegment<RecordEntity>> ExecuteQuerySegmentedAsync(TableQuery<RecordEntity> query, TableContinuationToken token)
        //{
        //    ConstructorInfo constructor = typeof(TableQuerySegment<RecordEntity>)
        //           .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
        //           .FirstOrDefault(c => c.GetParameters().Count() == 1);

        //    return await Task.FromResult(constructor.Invoke(TestFactory.GetRecordsEntity()) as TableQuerySegment<RecordEntity>);
        //}
    }
}
