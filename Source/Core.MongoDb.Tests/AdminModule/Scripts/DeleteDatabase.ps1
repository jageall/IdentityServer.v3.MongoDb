Param(
	[string] $database
)

$connection = 'mongodb://localhost'

Uninstall-IdentityServerDb -connection $connection -Database $database