using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;
using Serilog;
using static System.Net.Mime.MediaTypeNames;
using Nuke.Common.Tools.Docker;
using System.IO;

class Build : NukeBuild
{
    [Solution] readonly Solution Solution;

    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Platform to build - win-x64/linux-x64")] 
    readonly string Platform = IsLinux ? "linux-x64" : "win-x64";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Log.Information("Clean started");
            DotNetTasks.DotNetClean(s => s
                .SetProject(RootDirectory / "src" / "Datalinq.sln")
                .SetConfiguration(Configuration)
                .SetVerbosity(DotNetVerbosity.quiet)
                .SetProperty("WarningLevel", "0")
            );
            Log.Information("Clean finished");
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Log.Information("Restore started");
            DotNetTasks.DotNetRestore(s => s
                .SetProperty("WarningLevel", "0")
                .SetProjectFile(Solution));
            Log.Information("Restore finished");
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("Compile started");
            DotNetTasks.DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetProperty("WarningLevel", "0")
                .SetConfiguration(Configuration)
                .EnableNoRestore());
            Log.Information("Compile finished");
        });


    Target Test => _ => _
    .DependsOn(Compile)
    .Executes(() =>
    {
        //Log.Information("Testing (DataLinq.Code) started");

        //DotNetTasks.DotNetTest(s => s
        //    .SetProjectFile(RootDirectory / "src" / "test" / "DataLinq.Test.Code" / "DataLinq.Test.Code.csproj")
        //    .SetConfiguration(Configuration)
        //    .EnableNoBuild()
        //    .EnableNoRestore()
        //    .SetVerbosity(DotNetVerbosity.minimal));

        //Log.Information("Testing (DataLinq.Code) finished");

        //Log.Information("Testing (DataLinq.Core) started");

        //DotNetTasks.DotNetTest(s => s
        //    .SetProjectFile(RootDirectory / "src" / "test" / "DataLinq.Test.Core" / "DataLinq.Test.Core.csproj")
        //    .SetConfiguration(Configuration)
        //    .EnableNoBuild()
        //    .EnableNoRestore()
        //    .SetVerbosity(DotNetVerbosity.minimal));

        //Log.Information("Testing (DataLinq.Core) finished");

        Log.Information("Testing (DataLinq.Engine) started");

        DotNetTasks.DotNetTest(s => s
            .SetProjectFile(RootDirectory / "src" / "test" / "DataLinq.Test.Engine" / "DataLinq.Test.Engine.csproj")
            .SetConfiguration(Configuration)
            .EnableNoBuild()
            .EnableNoRestore()
            .SetVerbosity(DotNetVerbosity.minimal));

        Log.Information("Testing (DataLinq.Engine) finished");

        //Log.Information("Testing (DataLinq.Web) started");

        //DotNetTasks.DotNetTest(s => s
        //    .SetProjectFile(RootDirectory / "src" / "test" / "DataLinq.Test.Web" / "DataLinq.Test.Web.csproj")
        //    .SetConfiguration(Configuration)
        //    .EnableNoBuild()
        //    .EnableNoRestore()
        //    .SetVerbosity(DotNetVerbosity.minimal));

        //Log.Information("Testing (DataLinq.Web) finished");
    });

    Target Deploy => _ => _
    .DependsOn(Test) 
    .Executes(() =>
    {
        Log.Information("Deployment started");

        var projectCode = RootDirectory / "src" / "web" / "DataLinq.Code" / "DataLinq.Code.csproj";
        var outputDirCode = RootDirectory / "app" / "code" / "publish";
        var projectApi = RootDirectory / "src" / "web" / "DataLinq.Api" / "DataLinq.Api.csproj";
        var outputDirApi = RootDirectory / "app" / "api" / "publish";

        Log.Information("Publishing of DataLinq.Code started");

        DotNetTasks.DotNetPublish(s => s
            .SetProject(projectCode)
            .SetConfiguration(Configuration.Release) 
            .SetOutput(outputDirCode)
            .EnableNoRestore()
            .SetProperty("UseAppHost", "false"));

        Log.Information("Finished publishing DataLinq.Code");

        Log.Information("Publishing of DataLinq.Api started");

        DotNetTasks.DotNetPublish(s => s
            .SetProject(projectApi)
            .SetConfiguration(Configuration.Release)
            .SetOutput(outputDirApi)
            .EnableNoRestore()
            .SetProperty("UseAppHost", "false"));

        Log.Information("Finished publishing DataLinq.Api");

        Log.Information("Building Docker image for DataLinq.Code");

        Directory.CreateDirectory(outputDirCode);

        var dockerfileSourceCode = projectCode.Parent / "Dockerfile"; 
        var dockerfileTargetCode = outputDirCode / "Dockerfile";
        File.Copy(dockerfileSourceCode, dockerfileTargetCode, overwrite: true);

        DockerTasks.DockerBuild(s => s
            .SetPath(outputDirCode)
            .SetFile(dockerfileTargetCode)
            .SetTag("datalinq.code"));

        Log.Information("Building Docker image for DataLinq.Api");

        Directory.CreateDirectory(outputDirApi);

        var dockerfileSourceApi = projectApi.Parent / "Dockerfile";
        var dockerfileTargetApi = outputDirApi / "Dockerfile";
        File.Copy(dockerfileSourceApi, dockerfileTargetApi, overwrite: true);

        DockerTasks.DockerBuild(s => s
            .SetPath(outputDirApi)
            .SetFile(dockerfileTargetApi)
            .SetTag("datalinq.api"));

        Log.Information("Deployment finished");
    });


    Target Default => _ => _
        .DependsOn(Deploy);
}

