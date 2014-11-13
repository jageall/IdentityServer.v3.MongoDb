Import-Module .\IdentityServer.MongoDb.AdminModule.dll
#add just the open id scope
Get-Scopes -Predefined | where {$_.Name -eq "openid"} | Set-Scope
#add all predefined scopes
Get-Scopes -Predefined | Set-Scope