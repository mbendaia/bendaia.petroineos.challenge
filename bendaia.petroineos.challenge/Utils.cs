using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bendaia.petroineos.challenge
{
    public class Utils
    {

        public virtual IDictionary<int, String> getPeriodIdToHourMap()
        {
            var d = DateTime.Now.Date;
            var periodIdToHourMap = Enumerable.Range(1, 24).ToDictionary(i => i, i => d.AddHours(i - 2).ToString("HH:00"));
            return periodIdToHourMap;
        }

        public virtual async Task WriteToFileAsy(String filePath, String content)
        {
            const int BUFFER_SIZE = 4096;
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None, BUFFER_SIZE))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                var writeTask = Task.Factory.FromAsync(
                    stream.BeginWrite, stream.EndWrite, buffer, 0, buffer.Length, null);

                await writeTask;
            }
        }

        public String getReportFullPath(DateTime dateTime, String reportsLocation)
        {
            reportsLocation = reportsLocation.EndsWith("\\")? reportsLocation: reportsLocation+"\\";
            return String.Format("{0}PowerPosition_{1}.csv",reportsLocation ,dateTime.ToString("yyyyMMdd_HHmm"));
        }

    }
}
