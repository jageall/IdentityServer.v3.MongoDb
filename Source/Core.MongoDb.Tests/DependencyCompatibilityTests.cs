using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Core.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class DependencyCompatibilityTests
    {
        static readonly Assembly[] _bannedAssemblies = new Assembly[]
        {
            typeof(MongoDatabase).Assembly,
            typeof(BsonDocument).Assembly,
        }; 
        [Fact]
        public void NoTypesShouldExposeAnyIlMergedAssemblies()
        {
            var assembly = typeof (ServiceFactory).Assembly;
            foreach (var type in assembly.GetExportedTypes())
            {
                CheckConstructor(type);
                CheckProperties(type);
                CheckMethods(type);
            }
        }

        private void CheckMethods(Type type)
        {
            foreach (var method in type.GetMethods())
            {
                foreach (var parameterInfo in method.GetParameters())
                {
                    Assert.False(_bannedAssemblies.Any(x=> x == parameterInfo.ParameterType.Assembly), 
                        string.Format("ILMERGED TYPE EXPOSED {3} {0}.{1}({2}",
                        type.FullName,
                        method.Name,
                        FormatParameters(method.GetParameters()), method.ReturnType.Name));
                }
                Assert.False(_bannedAssemblies.Any(x => x == method.ReturnType.Assembly),
                        string.Format("ILMERGED TYPE EXPOSED {3} {0}.{1}({2}",
                        type.FullName,
                        method.Name,
                        FormatParameters(method.GetParameters()), method.ReturnType.Name));
            }
        }

        private void CheckProperties(Type type)
        {
            foreach (var propInfo in type.GetProperties())
            {
                Assert.False(_bannedAssemblies.Any(x => x == propInfo.PropertyType.Assembly), 
                    string.Format("ILMERGED TYPE EXPOSED: {0}.{1}",type.FullName,propInfo.Name));
            }
        }

        private void CheckConstructor(Type type)
        {
            foreach (var ctor in type.GetConstructors())
            {
                foreach (var parameterInfo in ctor.GetParameters())
                {
                    Assert.False(_bannedAssemblies.Any(x => x == parameterInfo.ParameterType.Assembly),
                        string.Format("ILMERGED TYPE EXPOSED {0}.ctor({1})", type.FullName, 
                        FormatParameters(ctor.GetParameters())));
                }
            }
        }

        private static string FormatParameters(ParameterInfo[] parameters)
        {
            return string.Join(",",parameters.Select(x=>x.ParameterType.Name));
        }
    }
}
