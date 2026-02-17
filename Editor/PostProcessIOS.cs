using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PostProcessIOS
{
    [PostProcessBuild(45)]
    private static void PostProcessBuild_iOS(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS)
            return;

        string podfilePath = Path.Combine(buildPath, "Podfile");

        if (!File.Exists(podfilePath))
        {
            Debug.LogError("Podfile not found!");
            return;
        }

        string podfile = File.ReadAllText(podfilePath);

        // Avoid duplicate insertion
        if (podfile.Contains("TapMindAdapter"))
            return;

        string tapMindPods =
            "  pod 'TapMindISAdapter', '1.0.3'\n" +
            "  pod 'TapMindSDK', '1.1.5'\n";

        // Insert pods before `end` of UnityFramework target
        string targetBlock = "target 'UnityFramework' do";
        int targetIndex = podfile.IndexOf(targetBlock);

        if (targetIndex == -1)
        {
            Debug.LogError("UnityFramework target not found in Podfile");
            return;
        }

        int endIndex = podfile.IndexOf("\nend", targetIndex);
        podfile = podfile.Insert(endIndex, "\n" + tapMindPods);

        File.WriteAllText(podfilePath, podfile);
    }
}
