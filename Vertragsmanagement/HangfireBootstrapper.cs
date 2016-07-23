using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Vertragsmanagement.Models.Manager;

public class HangfireBootstrapper : IRegisteredObject
{
    public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

    private readonly object _lockObject = new object();
    private bool _started;

    private BackgroundJobServer _backgroundJobServer;

    private HangfireBootstrapper()
    {


    }

    public void Start()
    {
        lock (_lockObject)
        {
            if (_started) return;
            _started = true;

            HostingEnvironment.RegisterObject(this);

            try
            {
                GlobalConfiguration.Configuration.UseSqlServerStorage("Server=tcp:daten.database.windows.net,1433;Data Source=daten.database.windows.net;Initial Catalog=hangfire;Persist Security Info=False;User ID=sopro@daten;Password=R54!^&Kt;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                // Wenn du hier einen Fehler hast, dann hast du keine Datenbankverbindung
            }
            catch (Exception) { return; }
            _backgroundJobServer = new BackgroundJobServer();
            //RecurringJob.AddOrUpdate(() => fo(), Cron.Minutely);
            
            
        }
    }

    public static void fo()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Task ausgeführt");
        } catch(Exception e) { }
        new Manager();
    }

    public void Stop()
    {
        lock (_lockObject)
        {
            if (_backgroundJobServer != null)
            {
                _backgroundJobServer.Dispose();
            }

            HostingEnvironment.UnregisterObject(this);
        }
    }

    void IRegisteredObject.Stop(bool immediate)
    {
        Stop();
    }
}