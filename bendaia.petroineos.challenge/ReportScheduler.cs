using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace bendaia.petroineos.challenge
{
    public class ReportScheduler : IReportScheduler, IDisposable
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IIntraDayReportGenerator _intraDayReportGenerator;
        private readonly IReportConfiguration _configuration;
        private Timer _timer;


        public ReportScheduler(IIntraDayReportGenerator intraDayReportGenerator, IReportConfiguration configuration)
        {
            _intraDayReportGenerator = intraDayReportGenerator;
            _configuration = configuration;
        }

        public void Start()
        {
            var intervalInSeconds = _configuration.SchedulerIntervalInSeconds;
            var startTimeSpan = TimeSpan.Zero;

            _timer = new Timer((e) =>
            {
                try
                {
                    _intraDayReportGenerator.Run().GetAwaiter().GetResult();;
                }
                catch (Exception exception)
                {
                    Logger.Fatal(exception);
                    Environment.Exit(-2);
                }
            }, null, startTimeSpan, TimeSpan.FromSeconds(intervalInSeconds));
            
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }
    }
}
