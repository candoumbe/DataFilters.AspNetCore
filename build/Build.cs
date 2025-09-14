using System;
using System.Collections.Generic;
using System.Linq;
using Candoumbe.Pipelines.Components;
using Candoumbe.Pipelines.Components.Formatting;
using Candoumbe.Pipelines.Components.GitHub;
using Candoumbe.Pipelines.Components.NuGet;
using Candoumbe.Pipelines.Components.Workflows;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.ReportGenerator;

namespace DataFilters.ContinuousIntegration;


[GitHubActions(
                  "continuous",
                  GitHubActionsImage.UbuntuLatest,
                  AutoGenerate = false,
                  FetchDepth = 0,
                  OnPushBranchesIgnore = [IHaveMainBranch.MainBranchName],
                  PublishArtifacts = true,
                  InvokedTargets =
                  [
                      nameof(IUnitTest.UnitTests),
                      nameof(IReportUnitTestCoverage.ReportUnitTestCoverage),
                      nameof(IMutationTest.MutationTests),
                      nameof(IPack.Pack)
                  ],
                  CacheKeyFiles = ["global.json", "src/**/*.csproj"],
                  ImportSecrets =
                  [
                      nameof(IPushNugetPackages.NuGetApiKey),
                      nameof(IReportCoverage.CodecovToken),
                      nameof(IMutationTest.StrykerDashboardApiKey)
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
                  OnPushBranches = [IGitFlow.MainBranchName],
                  InvokedTargets =
                  [
                      nameof(IUnitTest.UnitTests),
                      nameof(IPushNugetPackages.Publish),
                      nameof(ICreateGithubRelease.AddGithubRelease)
                  ],
                  EnableGitHubToken = true,
                  CacheKeyFiles = ["global.json", "src/**/*.csproj"],
                  PublishArtifacts = true,
                  ImportSecrets =
                  [
                      nameof(IPushNugetPackages.NuGetApiKey),
                      nameof(IReportCoverage.CodecovToken),
                      nameof(IMutationTest.StrykerDashboardApiKey)
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
    IDotnetFormat,
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
    IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations =>
    [
        new NugetPushConfiguration(apiKey: this.Get<IPushNugetPackages>().NuGetApiKey,
                                   canBeUsed: () => this.Get<IPushNugetPackages>().NuGetApiKey is not null,
                                   source: "https://api.nuget.org/v3/index.json"),
        new GitHubPushNugetConfiguration(githubToken: this.Get<IHaveGitHubRepository>().GitHubToken,
                                         canBeUsed: () => this.Get<IHaveGitHubRepository>().GitHubToken is not null,
                                         source: new Uri("https://nuget.pkg.github.com/candoumbe/index.json"))
    ];

    ///<inheritdoc/>
    bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>().CodecovToken is not null;

    ///<inheritdoc/>
    IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects
        => s_projectsWithUnitTests.Select(projectName => new MutationProjectConfiguration(Solution.AllProjects.Single(csproj => string.Equals(csproj.Name, projectName, StringComparison.InvariantCultureIgnoreCase)),
                                                                                          Solution.AllProjects.Where(csproj => csproj.Name.EndsWith($"{projectName}.UnitTests", StringComparison.InvariantCultureIgnoreCase)),
                                                                                          this.Get<IHaveTestDirectory>().TestDirectory / $"{projectName}.UnitTests" / "stryker-config.json"))
            .ToArray();


    private static readonly string[] s_projectsWithUnitTests = ["DataFilters.AspNetCore"];

    /// <inheritdoc />
    Configure<ReportGeneratorSettings> IReportUnitTestCoverage.ReportGeneratorSettings => _ => _.SetFramework("net8.0");

    /// <inheritdoc />
    bool IDotnetFormat.VerifyNoChanges => IsLocalBuild;
}