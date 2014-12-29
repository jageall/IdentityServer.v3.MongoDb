
Param(
	[string] $database
)

$connection = 'mongodb://localhost'

New-Client -AbsoluteRefreshTokenLifetime 10 -AccessTokenLifetime 20 -AccessTokenType Reference -AllowLocalLogin $false -AllowRememberConsent $true -AuthorizationCodeLifetime 30 -ClientId test -ClientName unittest -Password secret -Enabled $true -Flow AuthorizationCode -IdentityProviderRestrictions restriction1, restriction2 -IdentityTokenLifetime 40 -IdentityTokenSigningKeyType ClientSecret -LogoUri uri:logo -PostLogoutRedirectUris uri:logout1,uri:logout2 -RedirectUris uri:redirect1, uri:redirect2 -RefreshTokenExpiration Sliding -RefreshTokenUsage ReUse -RequireConsent $true -ScopeRestrictions openid,email,roles -SlidingRefreshTokenLifetime 50 | Set-Client -connection $connection -Database $database