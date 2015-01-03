
Param(
	[string] $database,
	[string] $clientId
)

$connection = 'mongodb://localhost'

Remove-Client -ClientId $clientId -connection $connection -Database $database