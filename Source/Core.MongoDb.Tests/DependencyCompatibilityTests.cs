/*
 * Copyright 2014, 2015 James Geall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Reflection;
using IdentityServer3.MongoDb;
using Xunit;

namespace Core.MongoDb.Tests
{
    public class DependencyCompatibilityTests
    {
        static readonly Assembly[] _bannedAssemblies = new Assembly[]
        {
            typeof(IMongoClient).Assembly,
            typeof(MongoException).Assembly,
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
