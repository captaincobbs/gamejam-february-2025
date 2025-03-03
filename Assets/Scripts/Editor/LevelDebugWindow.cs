using UnityEditor;
using UnityEngine;

public class LevelDebugWindow : EditorWindow
{
    [MenuItem("Window/Level Debug")]
    public static void ShowWindow()
    {
        GetWindow<LevelDebugWindow>();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Debug", EditorStyles.boldLabel);
    }
}
