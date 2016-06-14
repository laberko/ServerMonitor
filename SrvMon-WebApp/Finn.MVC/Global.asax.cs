using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Finn.MVC
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
		//redirect to https
		protected void Application_BeginRequest()
		{
			if (!Context.Request.IsSecureConnection)
				Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
		}
	}
}
