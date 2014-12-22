Param(
	[string] $database
)

$connection = 'mongodb://localhost'


Get-Scopes -Predefined | Set-Scope -connection $connection -Database $database