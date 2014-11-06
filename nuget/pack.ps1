
$version = $env:APPVEYOR_BUILD_VERSION + "-alpha-" + $env:APPVEYOR_BUILD_NUMBER

nuget pack core.nuspec -version $version
nuget pack AdminModule.nuspec -version $version