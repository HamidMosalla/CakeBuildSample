#tool "nuget:?package=xunit.runner.console&version=2.3.1"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
//Release mode only respond to power-shell file configuration

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./CakeBuildSample/bin") + Directory(configuration);

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
    NuGetRestore("./CakeBuildSample.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./CakeBuildSample.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./CakeBuildSample.sln", settings =>
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

	var dir = "./ProjectNugetPackages";
	if (!DirectoryExists(dir))
	{
		CreateDirectory(dir);
	}
	
   var nuGetPackSettings = new NuGetPackSettings {
            Id                      = "CakeBuildSample",
            Version                 = "1.0.1",
            Title                   = "CakeBuildSample",
			Description             = "CakeBuildSample",
            Summary                 = "CakeBuildSample",
            Authors                 = new[] {"Hamid Mosalla"},
            Owners                  = new[] {"CakeBuildSample"},
            RequireLicenseAcceptance= false,
            Symbols                 = false,
            NoPackageAnalysis       = true,
            Files                   = new [] { new NuSpecContent {  Source = "CakeBuildSample.dll", Target = "bin"} },
            BasePath                = "./CakeBuildSample/bin/Release/netcoreapp2.0",
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