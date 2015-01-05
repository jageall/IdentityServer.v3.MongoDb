Param(
	[string] $database,
	[string] $name
)

$connection = 'mongodb://localhost'

Remove-Scope -name $name  -connection $connection -Database $database