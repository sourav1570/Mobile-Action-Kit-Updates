using UnityEditor;
using UnityEngine;

public class SetupRequiredScriptsEditor : EditorWindow
{
    private string[] options = new string[]
    {
        "Setup Menus Interaction",
        "Setup Items",
        "Setup Weapons",
        "Setup Soldier Customization",
        "Setup Levels",
        "Setup Sounds",
        "Setup Settings"
    };

    private int selectedOption = 0;

    [MenuItem("Tools/Mobile Action Kit/Menus/Setup Required Scripts")]
    public static void ShowWindow()
    {
        GetWindow<SetupRequiredScriptsEditor>("Setup Required Scripts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Setup Required Scripts", EditorStyles.boldLabel);

        // Dropdown Selection
        selectedOption = EditorGUILayout.Popup("Select Setup Type", selectedOption, options);

        GUILayout.Space(10);

        // Add Required Scripts Button
        if (GUILayout.Button("Add Required Scripts"))
        {
            AddRequiredScripts();
        }
    }

    private void AddRequiredScripts()
    {
        string prefabPath = "";

        switch (selectedOption)
        {
            case 0:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Menus Interaction.prefab";
                break;
            case 1:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Items Menu Manager.prefab";
                break;
            case 2:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Weapons Manager.prefab";
                break;
            case 3:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Soldier Customization Manager.prefab";
                break;
            case 4:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Levels Manager.prefab";
                break;
            case 5:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Sounds Manager.prefab";
                break; 
            case 6:
                prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/Settings Manager.prefab";
                break;

        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError("Prefab not found at: " + prefabPath);
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        Debug.Log("Spawned and unpacked: " + prefab.name);
    }
}
