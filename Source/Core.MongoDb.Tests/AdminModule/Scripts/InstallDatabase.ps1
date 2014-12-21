Param(
	[string] $database
)

$connection = 'mongodb://localhost'


Install-IdentityServerDb -connection $connection -Database $database
