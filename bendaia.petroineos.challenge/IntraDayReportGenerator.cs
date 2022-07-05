using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Services;

namespace bendaia.petroineos.challenge
{
    public class IntraDayReportGenerator: IIntraDayReportGenerator
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Policy _serviceRetryPolicy;
        private readonly Policy _ioRetryPolicy;
        private readonly Utils _utils;
        private readonly IDictionary<int, String> _periodIdToHourMap;
        private readonly IPowerService _powerService;
        private readonly String _reportsLocation;



        public IntraDayReportGenerator(Utils utils, IPowerService powerService, IReportConfiguration configuration)
        {
            _utils = utils;
            _powerService = powerService;

            var serviceRetryCount = configuration.ServiceRetryCount;
            var ioRetryCount = configuration.IoRetryCount;

            String reportsLocation = configuration.ReportsLocation;
            DirectoryInfo dir = new DirectoryInfo(reportsLocation);
            if (!dir.Exists)
                dir.Create();
            _reportsLocation = reportsLocation;

            _serviceRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(serviceRetryCount,
                    attemptCount => TimeSpan.FromSeconds(Math.Pow(2, attemptCount)),
                    LogOnRetryAction());

            _ioRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(ioRetryCount,
                    attemptCount => TimeSpan.FromSeconds(attemptCount),
                    LogOnRetryAction());

            _periodIdToHourMap = _utils.getPeriodIdToHourMap();
        }

        public async Task Run()
        {
            Logger.Info("Starting report production ...");
            var reportDate = DateTime.Now;

            String reprortFullPath = _utils.getReportFullPath(reportDate, _reportsLocation);

            var trades = await _serviceRetryPolicy.ExecuteAsync(async () => await _powerService.GetTradesAsync(reportDate));

            var tradesAggregate = aggregatePowerTradesPerPeriod(trades);

            String reportContent = buildCsv(tradesAggregate);

            await _ioRetryPolicy.ExecuteAsync(async () =>
                await _utils.WriteToFileAsy(reprortFullPath, reportContent));

            Logger.Info("Successfully generated and saved report to {0}", reprortFullPath);
        }


        private Action<Exception, TimeSpan, int, Context> LogOnRetryAction()
        {
            return (exception, timespan, retryCount, context) =>
            {
                Logger.Error("try: {0}, will retry after {1},  Exception: {2}", retryCount, timespan,
                    exception.Message);
                Logger.Debug(exception.StackTrace);
            };
        }

        public IEnumerable<PowerPeriod> aggregatePowerTradesPerPeriod(IEnumerable<PowerTrade> powerTrades)
        {
            return from t in powerTrades
                   from p in t.Periods
                   group p by p.Period
                       into periodGroup
                       select new Services.PowerPeriod { Period = periodGroup.Key, Volume = periodGroup.Sum(x => x.Volume) };
        }

        public String buildCsv(IEnumerable<PowerPeriod> powerPeriods)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Local Time, Volume");
            powerPeriods.ToList().ForEach(x => strBuilder.AppendLine(_periodIdToHourMap[x.Period] + "," + x.Volume));
            return strBuilder.ToString();
        }
    }
}
