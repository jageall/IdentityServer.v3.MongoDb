using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer.Core.MongoDb;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace Core.MongoDb.Tests
{
    public class RequireAdminService
    {
        private readonly ServiceFactory _factory;
        private readonly IAdminService _adminService;
        private readonly IDependencyResolver _dependencyResolver;

        public RequireAdminService()
        {
            var storeSettings = ServiceFactory.DefaultStoreSettings();
            storeSettings.Database = "testidentityserver";
            _factory = new ServiceFactory(
                null,
                storeSettings);
            var protector = new ReverseDataProtector();
            _factory.ProtectClientSecretWith(protector);
            var resolver = new TestResolver();
            resolver.Register<IProtectClientSecrets>(new ProtectClientSecretWithDataProtector(protector));
            _dependencyResolver = resolver;
            _adminService = Factory.AdminService.TypeFactory(resolver);
            _adminService.CreateDatabase(expireUsingIndex:false);
        }

        public IAdminService AdminService
        {
            get { return _adminService; }
        }

        public ServiceFactory Factory
        {
            get { return _factory; }
        }

        public IDependencyResolver DependencyResolver
        {
            get { return _dependencyResolver; }
        }

        class ReverseDataProtector : IDataProtector
        {
            public byte[] Protect(byte[] data, string entropy = "")
            {
                return data.Reverse().ToArray();
            }

            public byte[] Unprotect(byte[] data, string entropy = "")
            {
                return data.Reverse().ToArray();
            }
        }

        class TestResolver : IDependencyResolver
        {
            private readonly Dictionary<Type, object> _instances;

            public TestResolver()
            {
                _instances = new Dictionary<Type, object>();
            }

            public T Resolve<T>()
            {
                object instance;
                if (!_instances.TryGetValue(typeof(T), out instance))
                {
                    throw new InvalidOperationException("requested type " + typeof(T).FullName + " not found");
                }
                return (T)instance;
            }

            public void Register<T>(T instance)
            {
                if (_instances.ContainsKey(typeof(T)))
                    throw new InvalidOperationException("Type already registered " + typeof(T).FullName);
                _instances[typeof(T)] = instance;
            }
        }
    }
}