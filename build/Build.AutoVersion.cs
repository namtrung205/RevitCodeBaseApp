using System.Text ;
using Nuke.Common.Git ;
using Nuke.Common.Tools.Git ;
using Nuke.Common.Tools.GitHub ;
using Octokit ;
sealed partial class Build
{
  Target AutoVersion =>
    _ => _
      .DependsOn()
      .Requires( () => GitHubToken )
      .Requires( () => GitRepository )
      .OnlyWhenStatic( () => IsServerBuild && GitRepository.IsOnMainOrMasterBranch() )
      .Executes( async () =>
      {
        Logger.Warn($"Begin PublishGitHub");
        GitHubTasks.GitHubClient = new GitHubClient( new ProductHeaderValue( Solution.Name ) ) { Credentials = new Credentials( GitHubToken ) } ;

        var gitHubName = GitRepository.GetGitHubName() ;
        var gitHubOwner = GitRepository.GetGitHubOwner() ;


        var versionFilePath = Path.Combine( Directory.GetCurrentDirectory(), "version.txt" ) ;
        var version = File.ReadAllText( versionFilePath ).Trim() ;
        var newVersion = IncrementVersion( version ) ;
        File.WriteAllText( versionFilePath, newVersion ) ;
      } ) ;
  
}
