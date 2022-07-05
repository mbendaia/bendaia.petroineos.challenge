using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Services;

namespace bendaia.petroineos.challenge.test
{
    [TestFixture]
    public class IntraDayReportGeneratorTest
    {
        private IReportConfiguration _config;
        private IPowerService _powerService;
        private IEnumerable<PowerTrade> _powerTrades;
        
        [SetUp]
        public void setup()
        {
            _config = Substitute.For<IReportConfiguration>();
            _config.IoRetryCount.Returns(2);
            _config.ServiceRetryCount.Returns(2);
            _config.ReportsLocation.Returns(@"c:\temp\dummy");
            _config.IoRetryCount.Returns(2);

            _powerService = Substitute.For<IPowerService>();
            var date = DateTime.Parse("04-July-2022 18:59");
            _powerTrades = generateTestPowerTrades(date);

            _powerService.GetTradesAsync(Arg.Any<DateTime>()).ReturnsForAnyArgs(Task.FromResult(_powerTrades));

            
        }

       

        [Test]
        public void test_aggregatePowerTradesPerPeriod()
        {
            IntraDayReportGenerator generator = new IntraDayReportGenerator(new Utils(), _powerService, _config);
            var aggregate = generator.aggregatePowerTradesPerPeriod(_powerTrades);
            
            Assert.AreEqual(aggregate.First().Period , 1);
            Assert.AreEqual(aggregate.First().Volume, 1*10*1 + 1*10*2);

            Assert.AreEqual(aggregate.ToArray()[2].Period, 3);
            Assert.AreEqual(aggregate.ToArray()[2].Volume, 3 * 10 * 1 + 3 * 10 * 2);

            Assert.AreEqual(aggregate.ToArray()[15].Period, 16);
            Assert.AreEqual(aggregate.ToArray()[15].Volume, 16 * 10 * 1 + 16 * 10 * 2);

            Assert.AreEqual(aggregate.ToArray()[19].Period, 20);
            Assert.AreEqual(aggregate.ToArray()[19].Volume, 20 * 10 * 1 + 20 * 10 * 2);

            Assert.AreEqual(aggregate.ToArray()[23].Period, 24);
            Assert.AreEqual(aggregate.ToArray()[23].Volume, 24 * 10 * 1 + 24 * 10 * 2);

        }

        [Test]
        public void test_buildCsv()
        {
            IntraDayReportGenerator generator = new IntraDayReportGenerator(new Utils(), _powerService, _config);
            var date = DateTime.Parse("04-July-2022 18:59");
            var powerPeriods = new List<PowerPeriod>() { new PowerPeriod(){Period = 1, Volume = 150} , new PowerPeriod(){Period = 2, Volume = 300}};

            var expected = "Local Time, Volume\n" +
                           "23:00,150\n" +
                           "00:00,300\n";
            Assert.AreNotEqual(generator.buildCsv(powerPeriods), expected);

        }
         
        [Test]
        public void test_run()
        {
            String filePath = "";
            String fileContent = "";

            var utils = Substitute.ForPartsOf<Utils>();
            utils.WriteToFileAsy(Arg.Do<String>(f=>filePath=f), Arg.Do<String>(c=>fileContent=c))
                .Returns(Task.FromResult((object)null));
            

            IntraDayReportGenerator generator = new IntraDayReportGenerator(utils, _powerService, _config);

            generator.Run().GetAwaiter().GetResult();

            Assert.IsTrue(filePath.StartsWith(_config.ReportsLocation));
            Assert.IsTrue(fileContent.StartsWith("Local Time, Volume"));
        }

        IEnumerable<PowerTrade> generateTestPowerTrades(DateTime date)
        {
            var powerTrades = Enumerable.Range(0, 2).Select(n => PowerTrade.Create(date, 24)).ToList();
            int i = 1;
            foreach (PowerTrade trade in powerTrades)
            {
                foreach (var period in trade.Periods)
                {
                    period.Volume = period.Period * 10 *i ;
                }

                i++;
            }

            return powerTrades;
        }
    }
}
