Param(
	[string] $database
)

$connection = 'mongodb://localhost'

Remove-Tokens -connection $connection -database $database -types AuthorizationCode