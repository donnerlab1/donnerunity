
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class BuildDonner {

    [MenuItem("Donner/Build Active Scene")]
    public static void BuildScene()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] activeScenes = new string[EditorBuildSettings.scenes.Length];
        for(int i = 0; i < activeScenes.Length; i++)
        {
            activeScenes[i] = EditorBuildSettings.scenes[i].path;
        }
        string[] levels = new string[] {SceneManager.GetActiveScene().path};
        
        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/DonnerGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Resources/tls.cert", path + "/DonnerGame_Data/Resources/tls.cert");
        FileUtil.CopyFileOrDirectory("Assets/Resources/admin.macaroon", path + "/DonnerGame_Data/Resources/admin.macaroon");
        FileUtil.CopyFileOrDirectory("Assets/Resources/donner.conf", path + "/DonnerGame_Data/Resources/donner.conf");
        FileUtil.CopyFileOrDirectory("Assets/Donner/Plugins/grpc_csharp_ext.x86.dll", path + "/DonnerGame_Data/Managed/grpc_csharp_ext.x86.dll");
        FileUtil.CopyFileOrDirectory("Assets/Donner/Plugins/grpc_csharp_ext.x64.dll", path + "/DonnerGame_Data/Managed/grpc_csharp_ext.x64.dll");

        
    }
    [MenuItem("Donner/Build with Editor Settings")]
    public static void BuildAllScenes()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = EditorBuildSettings.scenes[i].path;
        }
        
        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/DonnerGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/Resources/tls.cert", path + "/DonnerGame_Data/Resources/tls.cert");
        FileUtil.CopyFileOrDirectory("Assets/Resources/admin.macaroon", path + "/DonnerGame_Data/Resources/admin.macaroon");
        FileUtil.CopyFileOrDirectory("Assets/Resources/donner.conf", path + "/DonnerGame_Data/Resources/donner.conf");
        FileUtil.CopyFileOrDirectory("Assets/Donner/Plugins/grpc_csharp_ext.x86.dll", path + "/DonnerGame_Data/Managed/grpc_csharp_ext.x86.dll");
        FileUtil.CopyFileOrDirectory("Assets/Donner/Plugins/grpc_csharp_ext.x64.dll", path + "/DonnerGame_Data/Managed/grpc_csharp_ext.x64.dll");


    }

}
