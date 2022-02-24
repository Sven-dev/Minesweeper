using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PostBuildVersionFileInserter : IPostprocessBuildWithReport
{
    const string VersionFileName = "BuildVersion.txt";//this is also hard set in PreBuildVersionSetter and MenuController.GetVersion()
    const string VersionFileNameInDeploy = "BuildVersionGame.txt"; //this is also hard set in de smartwall central controller application

    public int callbackOrder { get { return 0; } }
    [PostProcessBuild]
    public void OnPostprocessBuild(BuildReport report)
    {
        string srcPath = Path.Combine(Application.streamingAssetsPath, VersionFileName);
        string destPath = Path.Combine(Path.GetDirectoryName(report.summary.outputPath), VersionFileNameInDeploy);
        File.Copy(srcPath, destPath,true);
    }
}
