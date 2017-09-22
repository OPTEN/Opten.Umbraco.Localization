#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=NUnit.Extension.NUnitV2ResultWriter"
#tool "docfx.msbuild"
#addin "Cake.DocFx"
#addin "Cake.Git"
#addin "Cake.FileHelpers"
#addin "nuget:http://nuget.oss-concept.ch/nuget/?package=Opten.Cake"

var target = Argument("target", "Default");
var feedUrl = "https://www.nuget.org/api/v2/package";

var dest = Directory("./artifacts");
var umb = dest + Directory("_umbraco");
string version = null;

// Cleanup

Task("Clean")
	.Does(() =>
{
	if (DirectoryExists(dest))
	{
		CleanDirectory(dest);
		DeleteDirectory(dest, recursive: true);
	}
});

// Versioning

Task("Version") 
	.IsDependentOn("Clean") 
	.Does(() =>
{
	if (DirectoryExists(dest) == false)
	{
		CreateDirectory(dest);
	}

	version = GitDescribe("../", false, GitDescribeStrategy.Tags, 0);

	PatchAssemblyInfo("../src/Opten.Umbraco.Localization.Core/Properties/AssemblyInfo.cs", version);
	PatchAssemblyInfo("../src/Opten.Umbraco.Localization.Web/Properties/AssemblyInfo.cs", version);

	FileWriteText(dest + File("Opten.Umbraco.Localization.variables.txt"), "version=" + version);

	Information("Patch files with ?v: {0}", ReplaceTextInFiles(
       "../src/Opten.Umbraco.Localization.Web.UI/App_Plugins/**/*",
       "?v=ASSEMBLY_VERSION",
       "?v=" + version
	).Count());

	Information("Patch files with &v: {0}", ReplaceTextInFiles(
       "../src/Opten.Umbraco.Localization.Web.UI/App_Plugins/**/*",
       "&v=ASSEMBLY_VERSION",
       "&v=" + version
	).Count());

	Information("Patch package.xml: {0}", ReplaceTextInFiles(
       File("package.xml"),
       "$ASSEMBLY_VERSION$",
       version
	).Count());
});

// Building

Task("Restore-NuGet-Packages") 
	.IsDependentOn("Version") 
	.Does(() =>
{ 
	NuGetRestore("../Opten.Umbraco.Localization.sln", new NuGetRestoreSettings {
		NoCache = true
	}); 
});

Task("Build") 
	.IsDependentOn("Restore-NuGet-Packages") 
	.Does(() =>
{
	MSBuild("../src/Opten.Umbraco.Localization.Core/Opten.Umbraco.Localization.Core.csproj", settings =>
		settings.SetConfiguration("Debug"));

	MSBuild("../src/Opten.Umbraco.Localization.Core/Opten.Umbraco.Localization.Core.csproj", settings =>
		settings.SetConfiguration("Release"));

	MSBuild("../src/Opten.Umbraco.Localization.Web/Opten.Umbraco.Localization.Web.csproj", settings =>
		settings.SetConfiguration("Debug"));

	MSBuild("../src/Opten.Umbraco.Localization.Web/Opten.Umbraco.Localization.Web.csproj", settings =>
		settings.SetConfiguration("Release"));

	MSBuild("../tests/Opten.Umbraco.Localization.Test/Opten.Umbraco.Localization.Test.csproj", settings =>
		settings.SetConfiguration("Release"));

	MSBuild("../src/Opten.Umbraco.Localization.Web.UI/Opten.Umbraco.Localization.Web.UI.csproj", settings =>
		settings.SetConfiguration("Debug"));

	MSBuild("../src/Opten.Umbraco.Localization.Web.UI/Opten.Umbraco.Localization.Web.UI.csproj", settings =>
		settings.SetConfiguration("Release"));

	// Copy files to artifacts for Umbraco Package
	CopyDirectory(Directory("../src/Opten.Umbraco.Localization.Web.UI/App_Plugins/OPTEN.Localization"), umb + Directory("App_Plugins/OPTEN.Localization"));
	CopyDirectory(Directory("../src/Opten.Umbraco.Localization.Web.UI/App_Plugins/OPTEN.Localization.Migration"), umb + Directory("App_Plugins/OPTEN.Localization.Migration"));
	CopyDirectory(Directory("../src/Opten.Umbraco.Localization.Web.UI/App_Plugins/OPTEN.UrlAlias"), umb + Directory("App_Plugins/OPTEN.UrlAlias"));
	CreateDirectory(umb + Directory("bin"));
	CopyFileToDirectory(File("../src/Opten.Umbraco.Localization.Web.UI/bin/Opten.Umbraco.Localization.Core.dll"), umb + Directory("bin"));
	CopyFileToDirectory(File("../src/Opten.Umbraco.Localization.Web.UI/bin/Opten.Umbraco.Localization.Web.dll"), umb + Directory("bin"));
	CopyFileToDirectory(File("../src/Opten.Umbraco.Localization.Web.UI/bin/Opten.Common.dll"), umb + Directory("bin"));
	CopyFileToDirectory(File("../src/Opten.Umbraco.Localization.Web.UI/bin/Microsoft.Web.XmlTransform.dll"), umb + Directory("bin"));
	CreateDirectory(umb + Directory("config"));
	CopyFileToDirectory(File("../src/Opten.Umbraco.Localization.Web.UI/config/opten.localization.config.json"), umb + Directory("config"));
	CopyFileToDirectory(File("package.xml"), umb);
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	var results = dest + Directory("tests");

	if (DirectoryExists(results) == false)
	{
		CreateDirectory(results);
	}

	//TODO: Why not csproj?
	NUnit3("../tests/Opten.Umbraco.Localization.Test/bin/Release/Opten.Umbraco.Localization.Test.dll", new NUnit3Settings {
		Results = results + File("Opten.Umbraco.Localization.Test.xml"),
		Configuration = "Release",
		ResultFormat = "nunit2" // Wait until Bamboo 5.14 is out to support NUnit 3!
	});
});

Task("Pack")
	.IsDependentOn("Build")
	.Does(() =>
{
	NuGetPackWithDependencies("./Opten.Umbraco.Localization.Core.nuspec", new NuGetPackSettings {
		Version = version,
		BasePath = "../",
		OutputDirectory = dest
	}, feedUrl);

	NuGetPackWithDependencies("./Opten.Umbraco.Localization.nuspec", new NuGetPackSettings {
		Version = version,
		BasePath = "../",
		OutputDirectory = dest
	}, feedUrl);

	// Umbraco 
 	MSBuild("./UmbracoPackage.proj", settings => 
 		settings.SetConfiguration("Release") 
 			    .WithTarget("Package") 
 				.WithProperty("BuildDir", MakeAbsolute(umb).FullPath.Replace("/", "\\")) 
 				.WithProperty("ArtifactsDir", dest)); 

});

// Deploying

Task("Deploy")
	.Does(() =>
{
	string packageId = "Opten.Umbraco.Localization";

	// Get the Version from the .txt file
	version = EnvironmentVariable("bamboo_inject_" + packageId.Replace(".", "_") + "_version");

	if(string.IsNullOrWhiteSpace(version))
	{
		throw new Exception("Version is missing for " + packageId + ".");
	}

	packageId = "Opten.Umbraco.Localization.Core";

	// Get the path to the package
	var package = File(packageId + "." + version + ".nupkg");
            
	// Push the package
	NuGetPush(package, new NuGetPushSettings {
		Source = feedUrl,
		ApiKey = EnvironmentVariable("NUGET_API_KEY")
	});

	packageId = "Opten.Umbraco.Localization";

	// Get the path to the package
	package = File(packageId + "." + version + ".nupkg");
            
	// Push the package
	NuGetPush(package, new NuGetPushSettings {
		Source = feedUrl,
		ApiKey = EnvironmentVariable("NUGET_API_KEY")
	});

	// Notifications
	Slack(new SlackSettings {
		ProjectName = "Opten.Umbraco.Localization.Core"
	});
	Slack(new SlackSettings {
		ProjectName = "Opten.Umbraco.Localization"
	});
});

Task("Default")
	.IsDependentOn("Pack");

RunTarget(target);