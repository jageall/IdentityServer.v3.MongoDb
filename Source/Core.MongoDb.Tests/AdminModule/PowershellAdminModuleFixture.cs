using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using IdentityServer.Core.MongoDb;
using IdentityServer.MongoDb.AdminModule;
using MongoDB.Driver;

namespace Core.MongoDb.Tests.AdminModule
{
    public class PowershellAdminModuleFixture : IDisposable
    {
        private readonly PowerShell _powerShell;
        private readonly string _database;
        private readonly MongoServer _server;
        private readonly ServiceFactory _serviceFactory;
        private readonly SimpleResolver _dependencyResolver;

        public PowershellAdminModuleFixture()
        {
            _powerShell = PowerShell.Create();
            _powerShell.AddCommand("Import-Module").AddParameter("Name", typeof(CreateScope).Assembly.Location);
            _database = Guid.NewGuid().ToString("N");
            var client = new MongoClient("mongodb://localhost");
            _server = client.GetServer();
            var settings = ServiceFactory.DefaultStoreSettings();
            settings.Database = _database;
            _serviceFactory = new ServiceFactory(null, settings);
            _dependencyResolver = new SimpleResolver();
            _dependencyResolver.Register<IProtectClientSecrets>(new DoNotProtectClientSecrets());
            
        }

        public PowerShell PowerShell
        {
            get { return _powerShell; }
        }

        public string Database
        {
            get { return _database; }
        }

        public MongoServer Server
        {
            get { return _server; }
        }

        public ServiceFactory Factory
        {
            get { return _serviceFactory; }
        }

        public SimpleResolver DependencyResolver
        {
            get { return _dependencyResolver; }
        }

        public void Dispose()
        {
            AggregateException failed = null;
            if (PowerShell.Streams.Error.Count > 0)
            {
                var exceptions = PowerShell.Streams.Error.Select(x => x.Exception).ToArray();
                foreach (var exception in exceptions)
                {
                    Console.WriteLine(exception);
                }
                failed = new AggregateException(exceptions);

            }
            PowerShell.Dispose();
            if (_server.DatabaseExists(Database))
                _server.DropDatabase(Database);
            if (failed != null) throw failed;
        }

        public string LoadScript(object o)
        {
            var type = o.GetType();

            var scriptResource = string.Format("{0}.Scripts.{1}.ps1", type.Namespace, type.Name);

            using (var stream = type.Assembly.GetManifestResourceStream(scriptResource))
            {
                if (stream == null)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("Could not find resource '{0}'", scriptResource);
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("Available Resources :");
                    foreach (var resourceName in type.Assembly.GetManifestResourceNames())
                    {
                        sb.AppendLine(resourceName);
                    }
                    throw new InvalidOperationException(sb.ToString());
                }
                return new StreamReader(stream).ReadToEnd();
            }

        }
    }
}
