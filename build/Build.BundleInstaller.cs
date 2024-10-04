using Nuke.Common.Git;
using Nuke.Common.Tooling ;

sealed partial class Build
{
    Target CreateBundleInstaller => d => d
        .DependsOn(Compile)
        .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnMainOrMasterBranch())
        .Executes(() =>
        {
            foreach (var project in Bundles)
            {
                Log.Information("Project: {Name}", project.Name);

                var directories = Directory.GetDirectories(project.Directory, "* Release *", SearchOption.AllDirectories);
                Assert.NotEmpty(directories, "No files were found to create a bundle");

                var bundleRoot = ArtifactsDirectory / project.Name;
                var bundlePath = bundleRoot;
                var manifestPath = bundlePath / "PackageContents.xml";
                var contentsDirectory = bundlePath / "Contents";
                foreach (var path in directories)
                {
                    var version = YearRegex.Match(path).Value;

                    Log.Information("Bundle files for version {Version}:", version);
                    CopyAssemblies(path, contentsDirectory / version);
                }

                GenerateManifest(project, directories, manifestPath);
                //CompressFolder(bundleRoot);
                Log.Information($"BundleRoot: {bundleRoot}");
                
                var innoSetupFolder = Directory.GetDirectories(RootDirectory + "\\Installer", "InnoSetup", SearchOption.AllDirectories).FirstOrDefault();
                
                Log.Information($"innoSetupFolder Folder: {innoSetupFolder}");
                
                //Create ISS File
                string defaultDirName = $@"C:\ProgramData\Autodesk\ApplicationPlugins\{project.Name}.bundle";
                string defaultGroupName = $"{project.Name}";
                string outputDir = ArtifactsDirectory;
                string sourcesFolder = bundleRoot;
                string outputBaseFilename = $"{project.Name}";
                if ( innoSetupFolder != null ) {
                    string issFilePath = Path.Combine(innoSetupFolder, "IssScriptFile.iss"); // Path where you want to create the .iss file
                
                    GenerateIssFile(issFilePath, Version, defaultDirName, defaultGroupName, outputDir, sourcesFolder, outputBaseFilename );                
                
                    //Create installer for bundle
                    if ( innoSetupFolder != null ) {
                        var exeFile = Path.Combine(innoSetupFolder, "ISCC.exe");
                        Log.Information($"exeFile: {exeFile}");
                        var arguments = $"\"{issFilePath}\"" ;
                        Log.Information($"arguments: {arguments}");
                
                        var process = ProcessTasks.StartProcess(exeFile, arguments, logInvocation: false, logger: InstallLogger);
                        process.AssertZeroExitCode();
                    }
                }

                //Delete OutBundle
                bundleRoot.DeleteDirectory();
            }
        });
    
    static void GenerateIssFile(
        string filePath, 
        string appVersion, 
        string defaultDirName, 
        string defaultGroupName, 
        string outputDir, 
        string sourcesFolder, 
        string outputBaseFilename)
    {
        // Template for the Inno Setup Script
        string issContent = $@"[Setup]
                    AppName={defaultGroupName}.TTD
                    AppVersion={appVersion}
                    DefaultDirName=""{defaultDirName}""
                    DefaultGroupName={defaultGroupName}
                    OutputDir= ""{outputDir}""
                    OutputBaseFilename= {outputBaseFilename}


                    [Files]
                    Source: ""{sourcesFolder}\*""; DestDir: ""{defaultDirName}""; Flags: ignoreversion recursesubdirs createallsubdirs; 

                    [Code]
                    function GetProgramData(Param: string): string;
                    begin
                      Result := ExpandConstant('{{pf}}') + '\..';  // Returns the path for %ProgramData%
                    end;";

        try
        {
            // Write the content to the .iss file
            File.WriteAllText(filePath, issContent);
        }
        catch (Exception ex)
        {
            Log.Information($"Error creating ISS file: {ex.Message}");
        }
    }

}