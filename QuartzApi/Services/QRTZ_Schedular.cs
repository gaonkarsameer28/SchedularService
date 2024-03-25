using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzApi.Services
{
    public class QRTZ_Schedular
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime LastExcecutedate { get; set; }
        public string CronExpression { get; set; }
        public string JobName { get; set; }
        public string JobStatus { get; set; }
        public string APIBody { get; set; }
        public string APIType { get; set; }
        public string APIURL { get; set; }
    }
}
