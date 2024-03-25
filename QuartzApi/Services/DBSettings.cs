using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzApi.Services
{
    public class DBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string QRTZ_SchedularCollectionName { get; set; }

        public string ThresholdManagerCollectionName { get; set; }
        public string APIManagerCollectionName { get; set; }
        public string HourlyStatisticsCollectionName { get; set; }

        public string ThresholdManagementCollectionName { get; set; }

    }
}
