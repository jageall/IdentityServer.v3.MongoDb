using System;
using Autofac;
using IdentityServer.Admin.MongoDb;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.MongoDb.AdminModule
{
    public class Factory
    {
        private readonly IContainer _container;

        public Factory(ServiceFactory config, AdminServiceRegistry admin)
        {
            var cb = new ContainerBuilder();
            Register(cb, config.AuthorizationCodeStore);
            Register(cb, config.ClientStore);
            Register(cb, config.ConsentStore);
            Register(cb, config.RefreshTokenStore);
            Register(cb, config.ScopeStore);

            Register(cb, config.TokenHandleStore);
            Register(cb, admin.AdminService);
            Register(cb, admin.TokenCleanupService);
            foreach (var registration in config.Registrations)
            {
                Register(cb, registration);
            }
            _container = cb.Build();
        }

        private void Register(ContainerBuilder cb, Registration registration, string name = null)
        {
            if (registration.Instance != null)
            {
                var reg = cb.Register(ctx => registration.Instance).SingleInstance();
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                } else
                {
                    reg.As(registration.DependencyType);
                }
            } else if (registration.Type != null)
            {
                var reg = cb.RegisterType(registration.Type);
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                } else
                {
                    reg.As(registration.DependencyType);
                }
            } else if (registration.Factory != null)
            {
                var reg = cb.Register(ctx => registration.Factory(new AutofacDependencyResolver(ctx)));
                if (name != null)
                {
                    reg.Named(name, registration.DependencyType);
                } else
                {
                    reg.As(registration.DependencyType);
                }
            } else
            {
                var message = "No type or factory found on registration " + registration.GetType().FullName;
                throw new InvalidOperationException(message);
            }
        }

        private class AutofacDependencyResolver : IDependencyResolver
        {
            private readonly IComponentContext _ctx;

            public AutofacDependencyResolver(IComponentContext ctx)
            {
                _ctx = ctx;
            }

            public T Resolve<T>(string name = null)
            {
                if(name == null)
                    return _ctx.Resolve<T>();
                return _ctx.ResolveNamed<T>(name);
            }
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}