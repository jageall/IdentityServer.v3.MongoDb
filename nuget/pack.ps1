param($version="invalid")

nuget pack ..\core.nuspec -version $version
nuget pack ..\AdminModule.nuspec -version $version