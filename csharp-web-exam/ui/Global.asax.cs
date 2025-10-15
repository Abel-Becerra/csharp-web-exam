using log4net;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace csharp_web_exam
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Application starting...");

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log.Info("Application started successfully");
        }

        protected void Application_Error()
        {
            System.Exception exception = Server.GetLastError();
            log.Error("Unhandled exception occurred", exception);
        }
    }
}
