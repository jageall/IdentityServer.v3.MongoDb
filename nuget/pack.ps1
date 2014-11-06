
$v = new-object System.Version($env:APPVEYOR_BUILD_VERSION)
$version = "{0}.{1}.{2}-alpha-{4}" -f ($v.Major, $v.Minor, $v.Revision, $env:APPVEYOR_BUILD_NUMBER)

nuget pack core.nuspec -version $version
nuget pack AdminModule.nuspec -version $version