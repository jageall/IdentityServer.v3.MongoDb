Import-Module .\IdentityServer.MongoDb.AdminModule.dll

Install-IdentityServerDb

Get-Scopes -Predefined | Set-Scope

$claims = @()
$claims += New-ScopeClaim -Name TestClaim

New-Scope -Name Test -DisplayName Test -Claims $claims | Set-Scope

New-Client -ClientName TestClient -ClientId TestClient | Set-Client

Get-Scopes

Get-Clients