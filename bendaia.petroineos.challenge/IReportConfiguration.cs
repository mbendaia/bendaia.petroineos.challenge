using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendaia.petroineos.challenge
{
    public interface IReportConfiguration
    {
        int ServiceRetryCount { get;  }
        int IoRetryCount { get; }
        string ReportsLocation { get; }
        double SchedulerIntervalInSeconds { get; }
    }
}
