using UnityEditor;
using UnityEngine;

public class SetupMenusEditor : EditorWindow
{
    public Camera mainCamera;

    private bool createMainMenu = true;
    private bool createMissionMenu = true;
    private bool createWeaponsMenu = true;
    private bool createItemsShopMenu = true;
    private bool createSoldierCustomizationMenu = true;
    private bool createIAPMenus = true;
    private bool createSettingsMenu = true;

    private string[] menuNames = new string[]
    {
        "Main Menu", "Mission Menu", "Weapons Menu", "Items Menu",
        "Soldier Customization", "IAP Menu", "Settings Menu"
    };

    private bool[] menuStates;

    [MenuItem("Tools/Mobile Action Kit/Menus/Setup Menus")]
    public static void ShowWindow()
    {
        GetWindow<SetupMenusEditor>("Setup Menus");
    }

    private void OnEnable()
    {
        menuStates = new bool[]
        {
            createMainMenu, createMissionMenu, createWeaponsMenu,
            createItemsShopMenu, createSoldierCustomizationMenu,
            createIAPMenus, createSettingsMenu
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Setup Menus", EditorStyles.boldLabel);

        // Assign Main Camera
        mainCamera = (Camera)EditorGUILayout.ObjectField("Main Camera", mainCamera, typeof(Camera), true);

        GUILayout.Space(10);

        GUILayout.Label("Select Menus to Create:", EditorStyles.boldLabel);

        for (int i = 0; i < menuNames.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            menuStates[i] = EditorGUILayout.Toggle(menuNames[i], menuStates[i]);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);

        // Create Button
        if (GUILayout.Button("Create The Menus", GUILayout.Height(30)))
        {
            CreateMenus();
        }
    }

    private void CreateMenus()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Please assign a Main Camera.");
            return;
        }

        string basePath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Menus_Prefabs/";

        // Load Prefabs
        GameObject canvasPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(basePath + "Canvas.prefab");
        GameObject eventSystemPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(basePath + "EventSystem.prefab");

        if (canvasPrefab == null || eventSystemPrefab == null)
        {
            Debug.LogError("Prefab paths are invalid.");
            return;
        }

        // Instantiate Canvas and EventSystem
        GameObject canvasInstance = PrefabUtility.InstantiatePrefab(canvasPrefab) as GameObject;
        GameObject eventSystemInstance = PrefabUtility.InstantiatePrefab(eventSystemPrefab) as GameObject;

        if (canvasInstance == null || eventSystemInstance == null)
        {
            Debug.LogError("Failed to instantiate prefabs.");
            return;
        }

        // Assign Camera to Canvas
        Canvas canvasComponent = canvasInstance.GetComponent<Canvas>();
        if (canvasComponent != null)
        {
            canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
            canvasComponent.worldCamera = mainCamera;
        }

        // Unpack Prefabs
        PrefabUtility.UnpackPrefabInstance(canvasInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);
        PrefabUtility.UnpackPrefabInstance(eventSystemInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

        // Delete Unchecked Menus
        HandleMenuDeletion(canvasInstance);
    }

    private void HandleMenuDeletion(GameObject canvasInstance)
    {
        string[] menuObjectNames = new string[]
        {
            "Main Menu", "Mission Menu", "Weapons Menu", "Items Menu",
            "Soldier Customization", "IAP Menu", "Settings Menu"
        };

        for (int i = 0; i < menuObjectNames.Length; i++)
        {
            if (!menuStates[i])
            {
                Transform menuTransform = canvasInstance.transform.Find(menuObjectNames[i]);
               
                if (menuTransform != null)
                {
                    DestroyImmediate(menuTransform.gameObject);
                    Debug.Log($"Deleted: {menuObjectNames[i]}");
                }

                // Special Case: Delete "Loading Screen" if "Mission Menu" is unchecked
                if (menuObjectNames[i] == "Mission Menu")
                {
                    Transform loadingScreenTransform = canvasInstance.transform.Find("Loading Screen");
                    if (loadingScreenTransform != null)
                    {
                        DestroyImmediate(loadingScreenTransform.gameObject);
                        Debug.Log("Deleted: Loading Screen (because Mission Menu was unchecked)");
                    }
                }
            }
        }
    }
}
