#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#addin Cake.GitVersioning
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Deploy");
var slnPath = "../TGBotFramework/TGBotFramework.sln";
var buildDirPath = "./bin";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDir = Directory(buildDirPath);

var buildSettings = new DotNetCoreBuildSettings
     {
         Framework = "netcoreapp3.1",
         Configuration = "Deploy"
       
     };
var packSettings = new DotNetCorePackSettings 
     {
         NoBuild = true,
         Configuration = "Deploy"
     };
var nugetSettings = new DotNetCoreNuGetPushSettings {
ApiKey = EnvironmentVariable("NugetAPIKey"),
Source = "https://api.nuget.org/v3/index.json"
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////



Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
      DotNetCoreBuild(slnPath, buildSettings);
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(()=>
{
     DotNetCorePack(slnPath, packSettings);
});

Task("PushNuget")
    .IsDependentOn("Pack")
    .Does(()=>{
    var package = buildDirPath + "/AleXr64.BotFramework."+ GitVersioningGetVersion().NuGetPackageVersion  + ".nupkg";
    DotNetCoreNuGetPush(package, nugetSettings);
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3(buildDirPath + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default").IsDependentOn("PushNuget");
   // .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
