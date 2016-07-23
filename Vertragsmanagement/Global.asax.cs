using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Http;
using System.Web.Routing;
using Hangfire;
using System.Web.Helpers;
using System.IdentityModel.Claims;

namespace Vertragsmanagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*
             * Ausnahmedetails: System.IO.FileLoadException: Die Datei oder Assembly "Newtonsoft.Json, Version=6.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed"
             * oder eine Abhängigkeit davon wurde nicht gefunden. Die gefundene Manifestdefinition der Assembly stimmt nicht mit dem Assemblyverweis überein. (Ausnahme von HRESULT: 0x80131040)
             */
            //GlobalConfiguration.Configure(WebApiConfig.Register);            

            AreaRegistration.RegisterAllAreas();

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            HangfireBootstrapper.Instance.Start();
        }
        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }
    }
}
