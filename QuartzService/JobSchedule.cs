using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzService
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression, string name, string apiurl, string apibody, string apitype, string id)
        {
            JobType = jobType;
            CronExpression = cronExpression;
            Name = name;
            APIUrl = apiurl;
            APIBody = apibody;
            APIType = apitype;
            Id = id;
        }

        public Type JobType { get; }
        public string CronExpression { get; set; }

        public string Name { get; }
        public string APIUrl { get; }
        public string APIBody { get; }
        public string APIType { get; }
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public DateTime LastExcecutedate { get; set; }
        public string JobName { get; set; }
        public string JobStatus { get; set; }

    }
}
