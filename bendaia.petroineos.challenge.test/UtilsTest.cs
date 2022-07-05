using System;
using NUnit.Framework;

namespace bendaia.petroineos.challenge.test
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        public void test_getPeriodIdToHourMap()
        {
            var utils = new Utils();
            var map = utils.getPeriodIdToHourMap();
            Assert.IsTrue(map.Count == 24);
            Assert.IsTrue(map[1] == "23:00");
            Assert.IsTrue(map[24] == "22:00");
            Assert.IsTrue(map[2] == "00:00");
            Assert.IsTrue(map[3] == "01:00");
        }

        [Test]
        public void test_getReportFullPath()
        {
            var utils = new Utils();
            var path = utils.getReportFullPath(DateTime.Parse("04-Jul-2022 18:40:59.765"), @"c:\temp\TradePositions");
            Assert.AreEqual(path, @"c:\temp\TradePositions\PowerPosition_20220704_1840.csv");
        }
    }
}
