using System.ServiceProcess;
using Autofac;
using Services;
using IContainer = Autofac.IContainer;

namespace bendaia.petroineos.challenge.WindowsService
{
    public partial class Service1 : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
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
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting power trades position report generation from windows service ...");
            container().Resolve<IReportScheduler>().Start();
        }

        protected override void OnStop()
        {
        }
    }
}
