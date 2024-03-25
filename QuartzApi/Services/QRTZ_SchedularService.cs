using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzApi.Services
{
    public class QRTZ_SchedularService
    {
        private readonly IMongoCollection<QRTZ_Schedular> _QRTZ_Schedular;

        public QRTZ_SchedularService(DBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            #region InterfaceRegistration
            // var QRTZSerializer = BsonSerializer.LookupSerializer<QRTZ_Schedular>();
            // BsonSerializer.RegisterSerializer<QRTZ_Schedular>(new ImpliedImplementationInterfaceSerializer<QRTZ_Schedular, QRTZ_Schedular>(QRTZSerializer));
            #endregion

            _QRTZ_Schedular = database.GetCollection<QRTZ_Schedular>(settings.QRTZ_SchedularCollectionName);
        }

        public List<QRTZ_Schedular> Get() =>
            _QRTZ_Schedular.Find(qrtz_schedular => true).ToList();

        public QRTZ_Schedular Get(string id) =>
            _QRTZ_Schedular.Find<QRTZ_Schedular>(qrtz_schedular => qrtz_schedular.Id == id).FirstOrDefault();

        public DateTime GetLastExecuteDateByJobName(string _jobname)
        {
            var fields = Builders<QRTZ_Schedular>.Projection.Include(p => p.LastExcecutedate);
            var data = _QRTZ_Schedular.Find<QRTZ_Schedular>(qrtz_schedular => qrtz_schedular.JobName == _jobname).Project<QRTZ_Schedular>(fields).ToList().AsQueryable();
            DateTime _getLastExecutedate = data.FirstOrDefault().LastExcecutedate.Date;
            return _getLastExecutedate;
        }


        public List<QRTZ_Schedular> GetActiveJobs(string jobstatus)
        {
            var builder = Builders<QRTZ_Schedular>.Filter;
            FilterDefinition<QRTZ_Schedular> filter = builder.Eq("JobStatus", jobstatus) & builder.Gt("Enddate", DateTime.UtcNow);
            var dataresult = _QRTZ_Schedular.Find(filter).ToList();
            return dataresult;
        }
        public string GetJobIdByCronAndURL(string cronexp)
        {
            string JobID = string.Empty;
            var fields = Builders<QRTZ_Schedular>.Projection.Include(p => p.Id);
            var data = _QRTZ_Schedular.Find<QRTZ_Schedular>(qrtz_schedular => qrtz_schedular.CronExpression == cronexp).Project<QRTZ_Schedular>(fields).ToList().AsQueryable();
            if (data.Count() > 0)
            {
                JobID = data.FirstOrDefault().Id;
            }
            else
                JobID = "";
            return JobID;
        }

        public string InsertIntoQuartz(QRTZ_Schedular qrtz_schedular)
        {
            _QRTZ_Schedular.InsertOne(qrtz_schedular);
            return qrtz_schedular.Id;
        }

        public QRTZ_Schedular Create(QRTZ_Schedular qrtz_schedular)
        {
            _QRTZ_Schedular.InsertOne(qrtz_schedular);
            return qrtz_schedular;
        }

        public void Update(string id, QRTZ_Schedular qrtz_schedularIn) =>
            _QRTZ_Schedular.ReplaceOne(qrtz_schedular => qrtz_schedular.Id == id, qrtz_schedularIn);

        public void Update(List<QRTZ_Schedular> lstqrtz_schedularIn)
        {
            foreach (var item in lstqrtz_schedularIn)
            {
                _QRTZ_Schedular.ReplaceOne(qrtz_schedular => qrtz_schedular.Id == item.Id, item);
            }
        }
        public void UpdateExecuteDate(string jobid, DateTime LastExcecutedate)
        {
            var filter = Builders<QRTZ_Schedular>.Filter.Eq("Id", jobid);
            var update = Builders<QRTZ_Schedular>.Update.Set("LastExcecutedate", LastExcecutedate);
            _QRTZ_Schedular.UpdateOne(filter, update);

        }

        public void Remove(QRTZ_Schedular qrtz_schedularIn) =>
            _QRTZ_Schedular.DeleteOne(qrtz_schedular => qrtz_schedular.Id == qrtz_schedularIn.Id);

        public void Remove(string id) =>
            _QRTZ_Schedular.DeleteOne(qrtz_schedular => qrtz_schedular.Id == id);
        public void CompleteExpiredJobs()
        {
            var filter = Builders<QRTZ_Schedular>.Filter.Lt("Enddate", DateTime.UtcNow);
            var update = Builders<QRTZ_Schedular>.Update.Set("JobStatus", "Complete");
            _QRTZ_Schedular.UpdateMany(filter, update);

        }

        public void CompleteJobs(string jobid)
        {
            var filter = Builders<QRTZ_Schedular>.Filter.Eq("Id", jobid);
            var update = Builders<QRTZ_Schedular>.Update.Set("JobStatus", "Complete");
            _QRTZ_Schedular.UpdateOne(filter, update);

        }
    }
}
