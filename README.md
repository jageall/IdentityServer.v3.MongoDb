# MongoDb Persistence for Thinktecture IdentityServer v3 #

**Current status: alpha**

All stores currently supported. Cleanup of expired tokens is done using mongodb ttl index.

Support for decrypting client secrets, encryption coming to powershell module soon - note this feature is likely to change as IdentityServer core will probably implement client secret protection

## Build Status ##
[![Build status](https://ci.appveyor.com/api/projects/status/gvfsmakv08fmxo68?svg=true)](https://ci.appveyor.com/project/jageall/identityserver-v3-mongodb)

## Credits ##
MongoDb Persistence for Thinktecture IdentityServer is built using the following great open source projects:
- [Thinktecture Identity Server v3](https://github.com/thinktecture/thinktecture.identityserver.v3)
- [MongoDb](http://www.mongodb.org/)
- [MongoDb C# Driver](https://github.com/mongodb/mongo-csharp-driver)
- [Katana](https://katanaproject.codeplex.com/)
- [xUnit](https://github.com/xunit)
