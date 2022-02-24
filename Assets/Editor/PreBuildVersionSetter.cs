using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreBuildVersionSetter : IPreprocessBuildWithReport
{
    const string VersionFileName = "BuildVersion.txt"; //this is also hard set in PostBuildVersionFileInserter and MenuController.GetVersion()
    const string PackageVersion = "P1";

    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildReport report)
    {
        string path = Path.Combine(Application.streamingAssetsPath, VersionFileName);
        int previusVersion = 0;
        if (File.Exists(path))
        {
            string versionSavedInFile = File.ReadAllText(path);
            previusVersion = int.Parse(versionSavedInFile.Split('.')[1]);
        }
        try
        {
            previusVersion++;
            WriteVersionFile(PackageVersion, previusVersion, path);
        }
        catch (IOException iOEx)
        {
            Debug.LogError("Failed to update build version file");
        }
    }

    private void WriteVersionFile(string packageVersion, int versionNumber, string path)
    {
        if (path.Equals(string.Empty)) {  }
        using (StreamWriter sr = File.CreateText(path))
        {
            sr.Write(packageVersion + "." + versionNumber.ToString());
        }
    }
}