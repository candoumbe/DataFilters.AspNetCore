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
    using Candoumbe.Pipelines.Components.Workflows;

    [GitHubActions(
                      "continuous",
                      GitHubActionsImage.UbuntuLatest,
                      AutoGenerate = false,
                      FetchDepth = 0,
                      OnPushBranchesIgnore = [nameof(IGitFlow.MainBranchName)],
                      PublishArtifacts = true,
                      InvokedTargets = [nameof(IUnitTest.UnitTests), nameof(IReportUnitTestCoverage.ReportUnitTestCoverage), nameof(IPack.Pack)],
                      CacheKeyFiles = ["global.json", "src/**/*.csproj"],
                      ImportSecrets =
                      [
                          nameof(IPushNugetPackages.NuGetApiKey),
                          nameof(IReportCoverage.CodecovToken)
                      ],
                      OnPullRequestExcludePaths =
                      [
                          "docs/*",
                          "README.md",
                          "CHANGELOG.md",
                          "LICENSE"
                      ]
                  )]
    [GitHubActions(
        "deployment",
        GitHubActionsImage.UbuntuLatest,
        AutoGenerate = false,
        FetchDepth = 0,
        OnPushBranches = [nameof(IGitFlow.MainBranchName), nameof(IGitFlow.ReleaseBranchPrefix) + "/*"],
        InvokedTargets = [nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease)],
        EnableGitHubToken = true,
        CacheKeyFiles = ["global.json", "src/**/*.csproj"],
        PublishArtifacts = true,
        ImportSecrets = [nameof(IPushNugetPackages.NuGetApiKey)],
        OnPullRequestExcludePaths =
        [
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        ]
    )]

    [GitHubActions(
        "nightly-manual",
        GitHubActionsImage.UbuntuLatest,
        AutoGenerate = false,
        FetchDepth = 0,
        On = [GitHubActionsTrigger.WorkflowDispatch],
        InvokedTargets = [nameof(IMutationTest.MutationTests), nameof(IPack.Pack)],
        EnableGitHubToken = true,
        CacheKeyFiles = ["global.json", "src/**/*.csproj"],
        PublishArtifacts = true,
        ImportSecrets = [nameof(IMutationTest.StrykerDashboardApiKey)]
    )]

    public class Build : EnhancedNukeBuild,
        IHaveSourceDirectory,
        IHaveTestDirectory,
        IHaveSolution,
        IClean,
        IRestore,
        IMutationTest,
        IPushNugetPackages,
        ICreateGithubRelease,
        IReportUnitTestCoverage,
        IGitFlowWithPullRequest
    {
        public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);

        [Required][Solution] public Solution Solution;

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IClean.DirectoriesToDelete => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/bin", "**/obj")
                                                                   .Concat(this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/bin", "**/obj"));

        ///<inheritdoc/>
        Solution IHaveSolution.Solution => Solution;

        ///<inheritdoc/>
        IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*.UnitTests");

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("*.csproj");

        ///<inheritdoc/>
        IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations => throw new NotImplementedException();

        ///<inheritdoc/>
        bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>().CodecovToken is not null;

        ///<inheritdoc/>
        IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects
            => new[] { "DataFilters.AspNetCore" }
                .Select(projectName => new MutationProjectConfiguration(Solution.AllProjects.Single(csproj => string.Equals(csproj.Name, projectName, StringComparison.InvariantCultureIgnoreCase)),
                                                                        Solution.AllProjects.Where(csproj => csproj.Name.EndsWith($"{projectName}.UnitTests", StringComparison.InvariantCultureIgnoreCase)),
                                                                        this.Get<IHaveTestDirectory>().TestDirectory / $"{projectName}.UnitTests" / "stryker-config.json"))
                .ToArray();
    }
}