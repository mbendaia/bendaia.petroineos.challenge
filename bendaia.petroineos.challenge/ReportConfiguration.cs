using System;
using System.Configuration;

namespace bendaia.petroineos.challenge
{
    public class ReportConfiguration : IReportConfiguration
    {
        public int ServiceRetryCount
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["serviceRetryCount"]);
            }
        }
        public int IoRetryCount
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["ioRetryCount"]);
            }
        }

        public string ReportsLocation
        {
            get
            {
                return ConfigurationManager.AppSettings["reportsLocation"];
            }

        }

        public double SchedulerIntervalInSeconds
        {
            get
            {
                return Double.Parse(ConfigurationManager.AppSettings["SchedulerIntervalInSeconds"]);
            }
        }
    }
}
