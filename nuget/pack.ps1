$v = new-object System.Version($env:APPVEYOR_BUILD_VERSION)
$version = "{0}.{1}.{2}-alpha-{3}" -f ($v.Major, $v.Minor, $v.Build, $v.Revision)

nuget pack core.nuspec -version $version
nuget pack AdminModule.nuspec -version $version