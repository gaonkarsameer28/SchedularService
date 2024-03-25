using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using QuartzService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace QuartzService
{
    [DisallowConcurrentExecution]
    public class APIJob : IJob
    {
        private readonly ILogger<APIJob> _logger;
        public string Id { get; set; }
        public string APIUrl { get; set; }
       // public string APIHeaders { get; set; }
        public string APIBody { get; set; }
        // public string Body { get; set; }
        public string APIType { get; set; }
        private HttpUtility httpUtility;
        private string MongoApiHost = string.Empty;
        public APIJob(ILogger<APIJob> logger, IConfiguration _configuration)
        {
            _logger = logger;
            httpUtility = new HttpUtility(_configuration);
            MongoApiHost = _configuration.GetSection("SchedularBaseUrl").Value;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("ApiJob!" + ((Quartz.Impl.JobDetailImpl)((Quartz.Impl.JobExecutionContextImpl)context).JobDetail).Name + " " + DateTime.Now.ToString());
            APIUrl = context.JobDetail.JobDataMap.Get("APIUrl").ToString();
           // APIHeaders = context.JobDetail.JobDataMap.Get("APIBody").ToString();
            APIBody = ReplaceVariables(context.JobDetail.JobDataMap.Get("APIBody").ToString(),context);
            APIType = context.JobDetail.JobDataMap.Get("APIType").ToString();
            Id = context.JobDetail.JobDataMap.Get("Id").ToString();
            _logger.LogInformation("APIURL : " + APIUrl + ";;APIBody : " + APIBody + " ;;APIType : " + APIType);
            if (APIType == "Get")
            { Get(APIUrl); }
            else if (APIType == "Put")
            { Put(APIUrl, APIBody); }
            else if (APIType == "Post")
            {Post(APIUrl,APIBody); }
            else
            { throw new Exception("Invlaid API type for API job "); }
            string UpdateexecuteDate = "\"{'id': '" + Id + "','LastExcecutedate': '" + context.FireTimeUtc.ToString() + "'}\"";
           // "\"{'id': '60709248d099cc0f8c2cb78b','LastExcecutedate': '2021-05-20T10:47:07.410+00:00'}\""
            Post(MongoApiHost + "/api/QRTZ_Schedular/UpdateExecuteDate", UpdateexecuteDate);

                return Task.CompletedTask;
        }

        private string ReplaceVariables(string str, IJobExecutionContext context)
        {
            JobSchedule jobSchedule= (JobSchedule) context.JobDetail.JobDataMap.Get("JobSchedule");
            string _str = str.Replace("[id]", jobSchedule.Id);
            _str = _str.Replace("[LastExcecutedate]", jobSchedule.LastExcecutedate.ToString());
            return _str;
        }
        private void Get(string url)
        {
            var apiResponse = httpUtility.HttpGet(url);
        }

        private void Post(string url, string content)
        {
            var response =  httpUtility.HttpPost(url, content);
        }

        private void Put(string url, string content)
        {
            var response = httpUtility.HttpPost(url, content);
        }

        private void Delete()
        {

        }
    }
}
