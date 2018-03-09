#tool "nuget:?package=xunit.runner.console&version=2.3.1"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./E-Vision-Exercise/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean").Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./E-Vision-Exercise.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./E-Vision-Exercise.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./E-Vision-Exercise.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{		
		var projects = GetFiles("./UnitTests/**/*.csproj");
        foreach(var project in projects)
        {
            DotNetCoreTool(
                projectPath: project.FullPath, 
                command: "xunit", 
                arguments: $"-configuration {configuration} -diagnostics -stoponfail"
            );
        }
});


Task("Create-NuGet-Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
   var nuGetPackSettings = new NuGetPackSettings {
            Id                      = "E-Vision-Exercise",
            Version                 = "1.0.1",
            Title                   = "E-Vision-Exercise",
			Description             = "E-Vision-Exercise",
            Summary                 = "E-Vision-Exercise",
            Authors                 = new[] {"Hamid Mosalla"},
            Owners                  = new[] {"Evision"},
            RequireLicenseAcceptance= false,
            Symbols                 = false,
            NoPackageAnalysis       = true,
            Files                   = new [] { new NuSpecContent {  Source = "EVisionExercise.dll", Target = "bin"} },
            BasePath                = "./E-Vision-Exercise/bin/Release/netcoreapp2.0",
            OutputDirectory         = "./ProjectNugetPackages"
                            };

    NuGetPack(nuGetPackSettings);
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Create-NuGet-Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);