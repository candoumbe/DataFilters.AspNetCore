namespace DataFilters.ContinuousIntegration
{
    using System;
    using System.Collections.Generic;
    using Nuke.Common.CI.GitHubActions;
    using Nuke.Common;
    using Candoumbe.Pipelines.Components;
    using Nuke.Common.ProjectModel;
    using Nuke.Common.CI;
    using Nuke.Common.IO;
    using Candoumbe.Pipelines.Components.NuGet;
    using Candoumbe.Pipelines.Components.GitHub;
    using System.Linq;

    [GitHubActions(
        "continuous",
        GitHubActionsImage.UbuntuLatest,
        FetchDepth = 0,
        OnPushBranchesIgnore = new[] { nameof(IGitFlow.MainBranchName) },
        PublishArtifacts = true,
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IReportCoverage.ReportCoverage), nameof(IPack.Pack) },
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
        ImportSecrets = new[]
        {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken)
        },
        OnPullRequestExcludePaths = new[]
        {
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        }
    )]
    [GitHubActions(
        "deployment",
        GitHubActionsImage.UbuntuLatest,
        FetchDepth = 0,
        OnPushBranches = new[] { nameof(IGitFlow.MainBranchName), nameof(IGitFlow.ReleaseBranchPrefix) + "/*" },
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
        EnableGitHubToken = true,
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
        PublishArtifacts = true,
        ImportSecrets = new[] { nameof(NugetApiKey) },
        OnPullRequestExcludePaths = new[] {
        "docs/*",
        "README.md",
        "CHANGELOG.md",
        "LICENSE"
        }
    )]

    public class Build : NukeBuild,
        IHaveSolution,
        IHaveSourceDirectory,
        IHaveTestDirectory,
        IGitFlow,
        IRestore,
        IClean,
        ICompile,
        IUnitTest,
        IPack,
        IPushNugetPackages,
        ICreateGithubRelease,
        IReportCoverage
    {
        public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);

        [Required][Solution] public readonly Solution Solution;

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IClean.DirectoriesToDelete => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/bin", "**/obj")
                                                                   .Concat(this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/bin", "**/obj"));

        ///<inheritdoc/>
        Solution IHaveSolution.Solution => Solution;

        [CI] public readonly GitHubActions GitHubActions;

        [Parameter]
        [Secret]
        public readonly string NugetApiKey;

        ///<inheritdoc/>
        IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*.UnitTests");

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("*.csproj");

        ///<inheritdoc/>
        IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations => throw new NotImplementedException();

        ///<inheritdoc/>
        bool IReportCoverage.ReportToCodeCov => true;
    }
}