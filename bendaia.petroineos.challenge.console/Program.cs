using System;
using Autofac;
using Services;

namespace bendaia.petroineos.challenge.console
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            container().Resolve<IReportScheduler>().Start();
            Console.ReadLine();
        }

        private static IContainer container()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ReportConfiguration>().As<IReportConfiguration>();
            builder.RegisterType<PowerService>().As<IPowerService>();
            builder.Register(b => new Utils());
            builder.RegisterType<IntraDayReportGenerator>().As<IIntraDayReportGenerator>();
            builder.RegisterType<ReportScheduler>().As<IReportScheduler>();

            return builder.Build();
        }
    }
}
