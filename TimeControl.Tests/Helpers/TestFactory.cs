using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TimeControl.Common;
using TimeControl.Functions;

namespace TimeControl.Tests
{
    public class TestFactory
    {
        public static ConsolidatedEntity GetConsolidatedEntity()
        {
            return new ConsolidatedEntity
            {
                Date = DateTime.UtcNow,
                Minutes = 15,
                EmployeeId = 1,
                ETag = "*",
                PartitionKey = "CONSOLIDATED",
                RowKey = Guid.NewGuid().ToString()
            };
        }
        public static List<ConsolidatedEntity> GetConsolidatedsEntity()
        {
            return new List<ConsolidatedEntity>
            {
                new ConsolidatedEntity
                {
                    Date = DateTime.UtcNow,
                    Minutes = 20,
                    EmployeeId = 1,
                    ETag = "*",
                    PartitionKey = "CONSOLIDATED",
                    RowKey = Guid.NewGuid().ToString()
                },
                new ConsolidatedEntity
                {
                    Date = DateTime.UtcNow,
                    Minutes = 45,
                    EmployeeId = 2,
                    ETag = "*",
                    PartitionKey = "CONSOLIDATED",
                    RowKey = Guid.NewGuid().ToString()
                }
            };
        }
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
        public static List<RecordEntity> GetRecordsEntity()
        {
            return new List<RecordEntity>
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

        public static DefaultHttpRequest CreateHttpRequest(List<Record> recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest, Formatting.Indented);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid id, Consolidated recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{id}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Consolidated recordRequest)
        {
            string request = JsonConvert.SerializeObject(recordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(List<Consolidated> recordRequest)
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

        public static Consolidated GetConsolidatedRequest()
        {
            return new Consolidated
            {
                Date = DateTime.UtcNow,
                Minutes = 15,
                EmployeeId = 1,
            };
        }

        public static List<Consolidated> GetConsolidatedsRequest()
        {
            return new List<Consolidated>
            {
                new Consolidated
                {
                    Date = DateTime.UtcNow,
                    Minutes = 20,
                    EmployeeId = 1
                },
                new Consolidated
                {
                    Date = DateTime.UtcNow,
                    Minutes = 45,
                    EmployeeId = 2
                }
            };
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

        public static List<Record> GetRecordsRequest()
        {
            return new List<Record>
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
