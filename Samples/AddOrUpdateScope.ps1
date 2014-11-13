Import-Module .\IdentityServer.MongoDb.AdminModule.dll
$claims = @()
$claims += New-ScopeClaim -Name TestClaim
New-Scope -Name scopeName -DisplayName displayName -Claims $claims -ClaimsRule customRuleName -Description "claim description" -Emphasize $true -Enabled $true -IncludeAllClaimsForUser $true -Required $true -ShowInDiscoveryDocument $true -Type Identity | Set-Scope