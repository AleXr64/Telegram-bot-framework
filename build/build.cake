#tool "nuget:?package=xunit.runner.console"
#addin Cake.GitVersioning
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Deploy");
var slnPath = "../TGBotFramework/BotFramework/BotFramework.csproj";
var buildDirPath = "./bin";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildDir = Directory(buildDirPath);

var buildSettings = new DotNetCoreBuildSettings
     {
         Framework = "net5.0",
         Configuration = "Deploy",
         OutputDirectory = buildDirPath
       
     };

var packSettings = new DotNetCorePackSettings 
     {
         OutputDirectory = buildDirPath,
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
    .IsDependentOn("Clean")
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
    .IsDependentOn("Clean")
    .Does(() =>
{
    var projectFiles = GetFiles("../TGBotFramework/*Tests/*.csproj");
    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath);
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default").IsDependentOn("Run-Unit-Tests").IsDependentOn("PushNuget");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
