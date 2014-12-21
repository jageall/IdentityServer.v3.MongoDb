using System;
using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer.MongoDb.AdminModule
{
    public class SimpleResolver : IDependencyResolver
    {
        private readonly Dictionary<Type, object> _instances;

        public SimpleResolver()
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
            _instances[typeof(T)] = instance;
        }
    }
}
