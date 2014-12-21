$v = new-object System.Version($env:APPVEYOR_BUILD_VERSION)
$version = "{0}.{1}.{2}-alpha-{3}" -f ($v.Major, $v.Minor, $v.Revision, $v.Build)

if($env:APPVEYOR_REPO_TAG -eq 'True'){
  $version = $env:APPVEYOR_REPO_BRANCH
}
#Nuget packages created every build for inspection before publishing
nuget pack core.nuspec -version $version
nuget pack AdminModule.nuspec -version $version

#Publish packages to project feed
appveyor PushArtifact IdentityServer.v3.MongoDb.nupkg
appveyor PushArtifact IdentityServer.MongoDb.AdminModule.nupkg