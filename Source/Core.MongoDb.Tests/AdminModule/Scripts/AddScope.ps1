Param(
	[string] $database
)

$connection = 'mongodb://localhost'


$claims = @()
$claims += New-ScopeClaim -Name unit_test_claim1 -AlwaysIncludeInIdToken $true -Description "Sample description for unit test"
$claims += New-ScopeClaim -Name unit_test_claim2 -AlwaysIncludeInIdToken $false -Description "Sample description"
New-Scope -Name unit_test_scope -DisplayName displayName -Claims $claims -ClaimsRule customRuleName -Description "claim description" -Emphasize $true -Enabled $true -IncludeAllClaimsForUser $true -Required $true -ShowInDiscoveryDocument $true -Type Identity | Set-Scope -connection $connection -Database $database