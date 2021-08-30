using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using TimeControl.Common;
using TimeControl.Functions;

namespace TimeControl.Tests
{
    public class TestFactory
    {
        public static RecordEntity GetRecordEntity()
        {
            return new RecordEntity
            {
                Consolidated = false,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = 1,
                ETag = "*",
                PartitionKey = "RECORD",
                RowKey = Guid.NewGuid().ToString(),
                Type = 0
            };
        }
        public static RecordEntity[] GetRecordsEntity()
        {
            return new RecordEntity[]
            {
                new RecordEntity
                {
                    Consolidated = false,
                    CreatedAt = DateTime.UtcNow,
                    EmployeeId = 2,
                    ETag = "*",
                    PartitionKey = "RECORD",
                    RowKey = Guid.NewGuid().ToString(),
                    Type = 0
                },
                new RecordEntity
                {
                    Consolidated = false,
                    CreatedAt = DateTime.UtcNow,
                    EmployeeId = 2,
                    ETag = "*",
                    PartitionKey = "RECORD",
                    RowKey = Guid.NewGuid().ToString(),
                    Type = 1
                }
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id, Record recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Record recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Record[] recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest, Formatting.Indented);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Record GetRecordRequest()
        {
            return new Record
            {
                Consolidated = false,
                CreatedAt = DateTime.UtcNow,
                EmployeeId = 1,
                Type = 1
            };
        }

        public static Record[] GetRecordsRequest()
        {
            return new Record[]
            {
                new Record {
                    Consolidated = false,
                    CreatedAt = DateTime.UtcNow,
                    EmployeeId = 1,
                    Type = 0
                },
                new Record {
                    Consolidated = false,
                    CreatedAt = DateTime.UtcNow,
                    EmployeeId = 1 ,
                    Type =1
                }
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            if (type == LoggerTypes.List)
            {
                return new ListLogger();
            }

            return NullLoggerFactory.Instance.CreateLogger("Null Logger");
        }
    }
}
