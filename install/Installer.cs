// using System;
// using System.IO ;
// using System.IO.Compression ;
// using System.Windows.Forms ;
// using Installer;
// using Microsoft.Deployment.WindowsInstaller ;
// using WixSharp;
// using WixSharp.CommonTasks;
// using WixSharp.Controls;
// using Action = WixSharp.Action ;
// using Assembly = System.Reflection.Assembly;
// using File = WixSharp.File ;
//
// // For using [CustomAction] attribute
//
// const string outputName = "RevitNice3PointCodeBase";
// const string projectName = "RevitNice3PointCodeBase";
//
// var project = new Project
// {
//     OutDir = "output",
//     Name = projectName,
//     Platform = Platform.x64,
//     UI = WUI.WixUI_FeatureTree,
//     MajorUpgrade = MajorUpgrade.Default,
//     GUID = new Guid("426FCAD7-7585-47D1-8E30-5F1A86DBBFF3"),
//     BannerImage = @"install\Resources\Icons\BannerImage.png",
//     BackgroundImage = @"install\Resources\Icons\BackgroundImage.png",
//     Version = Assembly.GetExecutingAssembly().GetName().Version.ClearRevision(),
//     ControlPanelInfo =
//     {
//         Manufacturer = Environment.UserName,
//         ProductIcon = @"install\Resources\Icons\ShellIcon.ico"
//     }
// };

// var wixEntities = Generator.GenerateWixEntitiesForBundle(args);
//project.RemoveDialogsBetween(NativeDialogs.WelcomeDlg, NativeDialogs.CustomizeDlg);

// BuildSingleUserMsi();
// BuildMultiUserUserMsi();
//
//
//
// void BuildSingleUserMsi()
// {
//     project.InstallScope = InstallScope.perUser;
//     project.OutFileName = $"{outputName}-{project.Version}-SingleUser";
//     project.Dirs =
//     [
//         new InstallDir(@"%AppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
//     ];
//     project.BuildMsi();
// }
//
// void BuildMultiUserUserMsi()
// {
//     project.InstallScope = InstallScope.perMachine;
//     project.OutFileName = $"{outputName}-{project.Version}-MultiUser";
//     project.Dirs =
//     [
//         new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
//     ];
//     project.BuildMsi();
// }

//
// BuildMultiUserUserMsiBundle() ;
// void BuildMultiUserUserMsiBundle()
// {
//     var manageAction = new ManagedAction( "ExtractZipFile") ;
//     
//     var project2 = new Project("MyProduct",
//         new Dir(@"%ProgramFiles%\MyCompany\MyProduct",
//             new File("C:\\Users\\C.DES.011\\Desktop\\Private\\Dev\\Code Base TTD\\RevitCodeBaseApp\\output\\OutBundle\\bundle.zip")),
//         manageAction);
//
//     project2.OutFileName = $"{outputName}-{project2.Version}-MultiUser-Bundle";
//     // Define a custom action
//     // project2.Actions = new[] 
//     // {
//     //     new ManagedAction(CustomActions.MyAction, 
//     //         Return.check, 
//     //         When.After, 
//     //         Step.InstallFiles, 
//     //         Condition.NOT_Installed)
//     // };
//     
//     // project.InstallScope = InstallScope.perMachine;
//     // project.OutFileName = $"{outputName}-{project.Version}-MultiUser-Bundle";
//     // project.Dirs =
//     // [
//     //     new InstallDir(@"%CommonAppDataFolder%\Autodesk\Revit\Addins\", wixEntities)
//     // ];
//     //
//     // project.Actions = new ManagedAction( CustomActions.ExtractZipFile, Return.ignore, When.AfterInstallFiles, Step.InstallFinalize ) ;
//     //
//     
//     
//     project2.BuildMsi();
// }

// public class CustomActions
// {
//     [CustomAction]
//     public static ActionResult MyAction(Session session)
//     {
//         MessageBox.Show("Installation step executed!", "WixSharp Custom Action");
//         return ActionResult.Success;
//     }
// }
//
//

// //

//
using System;
using System.IO ;
using System.IO.Compression ;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller ;
using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File ;

class Program
{
  static void Main()
  {
    var project = new Project("MyProduct",
      new Dir(@"%ProgramFiles%\My Company\My Product",
        new File("C:\\Users\\C.DES.011\\Desktop\\Private\\Dev\\Code Base TTD\\RevitCodeBaseApp\\output\\OutBundle\\bundle.zip")));

    // Define a custom action
    project.Actions = new[] 
    {
      new ManagedAction(CustomActions.ExtractZipFile, 
        Return.check, 
        When.After, 
        Step.InstallFinalize, 
        Condition.Installed)
    };

    // Build the MSI
    project.BuildMsi();
  }
}

// public class CustomActions
// {
//   [CustomAction]
//   public static ActionResult MyAction(Session session)
//   {
//     // MessageBox.Show("Installation step executed!", "WixSharp Custom Action");
//     return ActionResult.Success;
//   }
// }

public class CustomActions
{
  [CustomAction]
  public static ActionResult ExtractZipFile(Session session)
  {
    string installDir = session.Property("INSTALLDIR");
    string zipFilePath = "C:\\Program Files (x86)\\My Company\\My Product\\bundle.zip";
    string extractPath = "C:\\Program Files (x86)\\My Company\\My Product\\";

    if (System.IO.File.Exists(zipFilePath))
    {
      try
      {
        ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        session.Log("ZIP file extracted successfully.");
      }
      catch (Exception ex)
      {
        session.Log("Failed to extract ZIP file: " + ex.Message);
        return ActionResult.Failure;
      }
    }
    else
    {
      session.Log("ZIP file not found.");
      return ActionResult.Failure;
    }

    return ActionResult.Success;
  }
}
