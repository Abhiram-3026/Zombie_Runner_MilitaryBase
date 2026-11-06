#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>Automatic one-time project setup: creates MainMenu + Gameplay scenes, build settings, player settings, always-included shaders.</summary>
[InitializeOnLoad]
public static class ZombieRunnerSetup
{
    const string MenuScene = "Assets/Scenes/MainMenu.unity";
    const string GameScene = "Assets/Scenes/Gameplay.unity";

    static ZombieRunnerSetup() { EditorApplication.delayCall += Auto; }

    static void Auto()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!File.Exists(MenuScene) || !File.Exists(GameScene)) Run(true);
        else EnsureBuildSettings();
    }

    [MenuItem("Zombie Runner/Setup Project (scenes + settings)")]
    public static void RunFromMenu() { Run(true); }

    static void Run(bool openMenu)
    {
        Directory.CreateDirectory("Assets/Scenes");
        ConfigurePlayer();
        AddShaders("Standard", "Sprites/Default", "Legacy Shaders/Diffuse", "GUI/Text Shader");
        MakeScene(MenuScene, typeof(MenuBootstrap));
        MakeScene(GameScene, typeof(GameBootstrap));
        EnsureBuildSettings();
        AssetDatabase.SaveAssets();
        if (openMenu) EditorSceneManager.OpenScene(MenuScene);
        Debug.Log("[ZombieRunner] Setup complete. Press Play in the MainMenu scene.");
    }

    static void MakeScene(string path, System.Type bootstrap)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        new GameObject("Bootstrap").AddComponent(bootstrap);
        EditorSceneManager.SaveScene(scene, path);
    }

    static void EnsureBuildSettings()
    {
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MenuScene, true), new EditorBuildSettingsScene(GameScene, true) };
    }

    static void ConfigurePlayer()
    {
        PlayerSettings.productName = "3D Zombie Runner";
        PlayerSettings.companyName = "Student Project";
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.resizableWindow = true;
        PlayerSettings.runInBackground = true;
        if (PlayerSettings.colorSpace != ColorSpace.Gamma) PlayerSettings.colorSpace = ColorSpace.Gamma;
    }

    static void AddShaders(params string[] names)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset");
        if (assets == null || assets.Length == 0) return;
        var so = new SerializedObject(assets[0]);
        var arr = so.FindProperty("m_AlwaysIncludedShaders");
        if (arr == null) return;
        foreach (var n in names)
        {
            var sh = Shader.Find(n);
            if (sh == null) continue;
            bool have = false;
            for (int i = 0; i < arr.arraySize; i++) if (arr.GetArrayElementAtIndex(i).objectReferenceValue == sh) have = true;
            if (have) continue;
            arr.arraySize++;
            arr.GetArrayElementAtIndex(arr.arraySize - 1).objectReferenceValue = sh;
        }
        so.ApplyModifiedProperties();
    }

    [MenuItem("Zombie Runner/Build Windows EXE")]
    public static void BuildExe()
    {
        Run(false);
        Directory.CreateDirectory("Builds");
        var opts = new BuildPlayerOptions
        {
            scenes = new[] { MenuScene, GameScene },
            locationPathName = "Builds/ZombieRunner_MilitaryBase.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        BuildPipeline.BuildPlayer(opts);
    }
}
#endif
