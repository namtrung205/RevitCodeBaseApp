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
    
  string IncrementVersion( string version )
  {
    var parts = version.Split( '.' ) ;
    var major = int.Parse( parts[ 0 ] ) ;
    var minor = int.Parse( parts[ 1 ] ) ;
    var patch = int.Parse( parts[ 2 ] ) ;

    
    
    // Check branch name and increment version
    var branchName = Environment.GetEnvironmentVariable("GITHUB_HEAD_REF");
    Log.Information($"branchName {branchName}" );
    
    if ( branchName != null && branchName.Contains( "release/" ) ) {
      major++ ;
      minor = 0 ;
      patch = 0 ;
    }
    else if ( branchName != null && branchName.Contains( "feature/" ) ) {
      minor++ ;
      patch = 0 ;
    }
    else if ( branchName != null && branchName.Contains( "bugfix/" ) ) {
      patch++ ;
    }
    else {
      patch++ ;
    }

    return $"{major}.{minor}.{patch}" ;
  }


}
