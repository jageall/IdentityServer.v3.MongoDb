# MongoDb Persistence for IdentityServer3 #

**Current status: RC1**

This package supports the IdentityServer functionality. For administrative functions see the [admin project](https://github.com/jageall/IdentityServer3.Admin.MongoDb). There is also a [powershell module](https://github.com/jageall/IdentityServer3.AdminModule) available.

## Build Status ##
[![Build status](https://ci.appveyor.com/api/projects/status/gvfsmakv08fmxo68?svg=true)](https://ci.appveyor.com/project/jageall/identityserver-v3-mongodb)

## Usage ##

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Register your IUserService implementation
            var userService = new Registration<IUserService>(/*...*/);

            // Create and modify default settings
            var settings = IdentityServerMongoDb.StoreSettings.DefaultSettings();
            settings.ConnectionString = "mongodb://localhost";

            // Create the MongoDB factory
            var factory = new IdentityServerMongoDb.ServiceFactory(userService, settings);

            // Overwrite services, e.g. with in memory stores
            factory.Register(new Registration<IEnumerable<Client>>(MyClients.Get()));
            factory.ClientStore = new Registration<IClientStore>(typeof(InMemoryClientStore));

            var options = new IdentityServerOptions()
            {
                Factory = factory,
            };

            app.Map("/idsrv", idServer =>
            {
                idServer.UseIdentityServer(options);
            });
        }

## Credits ##
MongoDb Persistence for Thinktecture IdentityServer is built using the following great open source projects:
- [Thinktecture Identity Server v3](https://github.com/identityserver/identityserver3)
- [MongoDb](http://www.mongodb.org/)
- [MongoDb C# Driver](https://github.com/mongodb/mongo-csharp-driver)
- [Katana](https://katanaproject.codeplex.com/)
- [xUnit](https://github.com/xunit)
- [Autofac](http://autofac.org/)
- [LibLog](https://github.com/damianh/liblog)
thanks to all [contributors](https://github.com/jageall/IdentityServer3.MongoDb/graphs/contributors)!
