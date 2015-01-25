
Param(
	[string] $database
)

$connection = 'mongodb://localhost'

$clientSecrets = @()
$clientSecrets += New-ClientSecret -Value secret1 -Description testsecret -Expiration "2000/01/01T01:01:01" -Hash SHA256
$clientSecrets += New-ClientSecret -Value secret2 -Hash SHA512

$clientClaims = @()
$clientClaims += New-ClientClaim -Type claimtype1 -Value claimvalue1
$clientClaims += New-ClientClaim -Type claimtype2 -Value claimvalue2

New-Client -AbsoluteRefreshTokenLifetime 10 -AccessTokenLifetime 20 -AccessTokenType Reference -EnableLocalLogin $false -AllowRememberConsent $true -AuthorizationCodeLifetime 30 -ClientId test -ClientName unittest -Enabled $true -Flow AuthorizationCode -IdentityProviderRestrictions restriction1, restriction2 -IdentityTokenLifetime 40 -LogoUri uri:logo -PostLogoutRedirectUris uri:logout1,uri:logout2 -RedirectUris uri:redirect1, uri:redirect2 -RefreshTokenExpiration Sliding -RefreshTokenUsage ReUse -RequireConsent $true -ScopeRestrictions openid,email,roles -SlidingRefreshTokenLifetime 50 -ClientSecrets $clientSecrets -AlwaysSendClientClaims $true -IncludeJwtId $true -PrefixClientClaims $true -CustomGrantTypeRestrictions grantrestriction1, grantrestriction2, grantrestriction3 -Claims $clientClaims | Set-Client -connection $connection -Database $database