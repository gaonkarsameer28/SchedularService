using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzService;

namespace QuartzService
{
    public class QuartzHostedService : IHostedService, IDisposable
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<JobSchedule> _jobSchedules;
        private readonly IConfiguration _configuration;
        private string SchedularBaseUrl = string.Empty;
        private string GetScheduledJobsapi = string.Empty;
        private string AddNewJobsapi = string.Empty;
        private string UpdateJobsapi = string.Empty;
        private string CompleteExpiredJobsapi = string.Empty;      
        private HttpUtility httpUtility;
        int PickJobMiliSecFreq;
        public QuartzHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<JobSchedule> jobSchedules,
            IConfiguration configuration
            )
        {
            _schedulerFactory = schedulerFactory;
            _jobSchedules = jobSchedules;
            _jobFactory = jobFactory;
            _configuration = configuration;
            SchedularBaseUrl = _configuration.GetSection("SchedularBaseUrl").Value;
            GetScheduledJobsapi = _configuration.GetSection("GetScheduledJobsapi").Value;
            AddNewJobsapi = _configuration.GetSection("AddNewJobsapi").Value;
            UpdateJobsapi = _configuration.GetSection("UpdateJobsapi").Value;
            CompleteExpiredJobsapi = _configuration.GetSection("CompleteExpiredJobsapi").Value;
            PickJobMiliSecFreq = int.Parse(_configuration.GetSection("PickJobMiliSecFreq").Value);
            httpUtility = new HttpUtility(_configuration);
        }

        public IScheduler Scheduler { get; set; }
        private IMongoCollection<BsonDocument> JobCollection { get; set; }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //IMongoCollection<BsonDocument> JobCollection = null;
            Intiatejobs(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {

                CompleteJobs();
                Addnewjobs(cancellationToken);
                await Task.Delay(PickJobMiliSecFreq, cancellationToken);
            }

        }


        private async void Intiatejobs(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;
            List<QRTZ_Schedular> lstQRTZ_Schedular = new List<QRTZ_Schedular>();
            var apiResponse = await httpUtility.HttpGet(SchedularBaseUrl + GetScheduledJobsapi);
            // dynamic dynJson = JsonConvert.DeserializeObject(result);
            lstQRTZ_Schedular = JsonConvert.DeserializeObject<List<QRTZ_Schedular>>(apiResponse);
            foreach (var item in lstQRTZ_Schedular)
            {
                string JobName = item.JobName;
                string CronExpression = item.CronExpression;
                string APIUrl = item.APIURL;
                string APIBody = item.APIBody;
                string APIType = item.APIType;
                string Id = item.Id;

                JobSchedule JobSchedule3 = new JobSchedule(
                jobType: typeof(APIJob),
                cronExpression: CronExpression, name: JobName,
                apiurl: APIUrl, apibody: APIBody, apitype: APIType,id:Id);

                var job = CreateJob(JobSchedule3);
                job.JobDataMap.Put("APIUrl", APIUrl);
                job.JobDataMap.Put("APIBody", APIBody);
                job.JobDataMap.Put("APIType", APIType);
                job.JobDataMap.Put("Id", Id);
                job.JobDataMap.Put("JobSchedule", JobSchedule3);
                var trigger = CreateTrigger(JobSchedule3);
                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            if (Scheduler != null)
            {
                await Scheduler.Start(cancellationToken);
            }
        }
        private async void Addnewjobs(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            List<QRTZ_Schedular> lstQRTZ_Schedular = new List<QRTZ_Schedular>();
            var apiResponse = await httpUtility.HttpGet(SchedularBaseUrl + AddNewJobsapi);

            lstQRTZ_Schedular = JsonConvert.DeserializeObject<List<QRTZ_Schedular>>(apiResponse);
            foreach (var item in lstQRTZ_Schedular)
            {
                string JobName = item.JobName;
                string CronExpression = item.CronExpression;
                string APIUrl = item.APIURL;
                string APIBody = item.APIBody;
                string APIType = item.APIType;
                string Id = item.Id;
                JobSchedule JobSchedule3 = new JobSchedule(
                    jobType: typeof(APIJob),
                    cronExpression: CronExpression, name: JobName,
                    apiurl: APIUrl, apibody: APIBody, apitype: APIType,id: Id);

                var job = CreateJob(JobSchedule3);
                var trigger = CreateTrigger(JobSchedule3);
                job.JobDataMap.Put("APIUrl", APIUrl);
                job.JobDataMap.Put("APIBody", APIBody);
                job.JobDataMap.Put("APIType", APIType);
                job.JobDataMap.Put("Id", Id);
                job.JobDataMap.Put("JobSchedule", JobSchedule3);
                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
                item.JobStatus = "Scheduled";
            }

            var requst = JsonConvert.SerializeObject(lstQRTZ_Schedular);            
            var response1 = await httpUtility.HttpPut(SchedularBaseUrl + UpdateJobsapi, requst);
            if (response1)
            {
                Console.Write("Success");
            }
            else
            { Console.Write("Error"); }



            if (Scheduler != null && !Scheduler.IsStarted)
            {
                await Scheduler.Start(cancellationToken);
            }


        }
        private void CompleteJobs()
        {
            httpUtility.HttpPut(SchedularBaseUrl + CompleteExpiredJobsapi, "");

        }
        private async void SchduleJob(object state)
        {
            CancellationToken cancellationToken = (CancellationToken)state;

            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobSchedule in _jobSchedules)
            {
                var job = CreateJob(jobSchedule);
                var trigger = CreateTrigger(jobSchedule);

                await Scheduler.ScheduleJob(job, trigger, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);

        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }

        private static IJobDetail CreateJob(JobSchedule schedule)
        {
            var jobType = schedule.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(schedule.Name)
                .WithDescription(jobType.Name)
                .Build();
        }

        private static ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.Name}.trigger")
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
