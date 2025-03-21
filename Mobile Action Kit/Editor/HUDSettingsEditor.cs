using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class HUDSettingsEditor : EditorWindow
    {
        [System.Serializable]
        public class HUDSettingsData : ScriptableObject
        {
            public List<GameObject> UIElementsToResizeAndDrag = new List<GameObject>();
        }

        public bool IsHudSettingsAlreadyExist = false;
        private GameObject requiredScripts;
        private GameObject canvas2D;
        private GameObject pausePanel;
        private Sprite spriteIcon;
        private GameObject HUDSettingsPrefab;
          
        private HUDSettingsData settingsData;
        private SerializedObject serializedSettings;

        GameObject hudSettings;

        [MenuItem("Tools/Mobile Action Kit/Player/Player/Create HUD Settings", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<HUDSettingsEditor>("HUD Settings");
        }

        private void OnEnable()
        {
            // Initialize the data object for serialization
            settingsData = ScriptableObject.CreateInstance<HUDSettingsData>();
            serializedSettings = new SerializedObject(settingsData);
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("HUD Settings", EditorStyles.boldLabel);

        //    IsHudSettingsAlreadyExist = EditorGUILayout.Toggle("Is Hud Settings Already Exist", IsHudSettingsAlreadyExist);

        //    requiredScripts = (GameObject)EditorGUILayout.ObjectField("Required Scripts", requiredScripts, typeof(GameObject), true);
        //    canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);
        //    pausePanel = (GameObject)EditorGUILayout.ObjectField("Pause Panel", pausePanel, typeof(GameObject), true);
        //    spriteIcon = (Sprite)EditorGUILayout.ObjectField("Sprite Adjuster Image", spriteIcon, typeof(Sprite), true);

        //    HUDSettings = (GameObject)EditorGUILayout.ObjectField("HUD Settings GameObject", HUDSettings, typeof(GameObject), true);

        //    GUILayout.Label("Adjuster UI", EditorStyles.boldLabel);
        //    SerializedProperty adjusterUIProperty = serializedSettings.FindProperty("adjusterUI");
        //    EditorGUILayout.PropertyField(adjusterUIProperty, true);

        //    serializedSettings.ApplyModifiedProperties();

        //    if (GUILayout.Button("Add Image Component and Apply Sprite"))
        //    {
        //        AddImageComponentAndApplySprite();
        //    }

        //    if (GUILayout.Button("Create Required UI"))
        //    {
        //        CreateRequiredUI();
        //    }

        //    if (GUILayout.Button("Add Required Components"))
        //    {
        //        AddRequiredComponents();
        //    }
        //}
        private void OnGUI()
        {
            // Set a consistent label width for better alignment
            EditorGUIUtility.labelWidth = 200; // Adjust based on your needs

            // Add a title for the HUD settings
            GUILayout.Label("HUD Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space(); // Add space after the title

            // Group HUD settings fields
            EditorGUILayout.BeginVertical("box");
            IsHudSettingsAlreadyExist = EditorGUILayout.Toggle("Is Hud Settings Already Exist", IsHudSettingsAlreadyExist);

            if(IsHudSettingsAlreadyExist == false)
            {
                requiredScripts = (GameObject)EditorGUILayout.ObjectField("Required Scripts", requiredScripts, typeof(GameObject), true);
                canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);
                pausePanel = (GameObject)EditorGUILayout.ObjectField("Pause Panel", pausePanel, typeof(GameObject), true);
                spriteIcon = (Sprite)EditorGUILayout.ObjectField("Sprite Adjuster Image", spriteIcon, typeof(Sprite), true);
                HUDSettingsPrefab = (GameObject)EditorGUILayout.ObjectField("HUD Settings Prefab" +
                    "", HUDSettingsPrefab, typeof(GameObject), true);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space(10); // Add some space before the next section

                // Property field for the adjuster UI
                SerializedProperty adjusterUIProperty = serializedSettings.FindProperty("UIElementsToResizeAndDrag");
                EditorGUILayout.PropertyField(adjusterUIProperty, true);

                // Apply changes to serialized properties
                serializedSettings.ApplyModifiedProperties();

                EditorGUILayout.Space(10); // Add space before the buttons

                // Centered Buttons Section
                CenteredButton("Add Image Component and Apply Sprite", () =>
                {
                    AddImageComponentAndApplySprite();
                });

                CenteredButton("Create Required UI", () =>
                {
                    CreateRequiredUI();
                });

                CenteredButton("Add Required Components", () =>
                {
                    AddRequiredComponents();
                });
            }
            else
            {
                canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);
                spriteIcon = (Sprite)EditorGUILayout.ObjectField("Sprite Adjuster Image", spriteIcon, typeof(Sprite), true);
                EditorGUILayout.EndVertical();


                // Property field for the adjuster UI
                SerializedProperty adjusterUIProperty = serializedSettings.FindProperty("UIElementsToResizeAndDrag");
                EditorGUILayout.PropertyField(adjusterUIProperty, true);

                // Apply changes to serialized properties
                serializedSettings.ApplyModifiedProperties();

                // Centered Buttons Section
                CenteredButton("Add Image Component and Apply Sprite", () =>
                {
                    AddImageComponentAndApplySprite();
                });

                // Centered Buttons Section
                CenteredButton("Add Required Components To UI Elements", () =>
                {
                    AddRequiredComponentsToUI();
                });
            }

        }

        /// <summary>
        /// Helper method to create a centered button.
        /// </summary>
        private void CenteredButton(string label, System.Action onClick)
        {
            EditorGUILayout.BeginHorizontal(); // Begin horizontal layout
            GUILayout.FlexibleSpace(); // Push content to the center
            if (GUILayout.Button(label, GUILayout.Height(40), GUILayout.Width(300)))
            {
                onClick.Invoke(); // Execute the action when the button is clicked
            }
            GUILayout.FlexibleSpace(); // Push content to the center
            EditorGUILayout.EndHorizontal(); // End horizontal layout
            EditorGUILayout.Space(5); // Add spacing between buttons
        }




        private void AddImageComponentAndApplySprite()
        {
            if (spriteIcon == null)
            {
                Debug.LogError("Sprite Icon is not assigned.");
                return;
            }

            foreach (var obj in settingsData.UIElementsToResizeAndDrag)
            {
                if (obj != null)
                {
                    var image = obj.GetComponent<UnityEngine.UI.Image>();
                    if (image == null)
                    {
                        image = obj.AddComponent<UnityEngine.UI.Image>();
                    }
                    image.sprite = spriteIcon;
                }
            }
        }

        private void CreateRequiredUI()
        {
            if (canvas2D == null)
            {
                Debug.LogError("Canvas 2D is not assigned.");
                return;
            }

            string prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/HUD Settings.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError("HUD Settings prefab not found at " + prefabPath);
                return;
            }

            hudSettings = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas2D.transform);
            hudSettings.transform.SetParent(canvas2D.transform, false);

            PrefabUtility.UnpackPrefabInstance(hudSettings, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        }

        private void AddRequiredComponents()
        {
            if (HUDSettingsPrefab == null)
            {
                Debug.LogError("HUD Settings is not instantiated or found under Canvas 2D.");
                return;
            }

            GameObject emptyGO = new GameObject("EmptyGameObject");
            emptyGO.transform.SetParent(requiredScripts.transform);

            emptyGO.transform.name = "HUD Settings";

            var hudSettingsComponent = emptyGO.AddComponent<HUDSettings>();

            hudSettingsComponent.HudSettingsPanel = hudSettings;
            hudSettingsComponent.PausePanel = pausePanel;
            hudSettingsComponent.DragButton = HUDSettingsPrefab.transform.GetChild(0).GetChild(4).GetComponent<Button>();
            hudSettingsComponent.ResizeButton = HUDSettingsPrefab.transform.GetChild(0).GetChild(3).GetComponent<Button>();
            hudSettingsComponent.SaveButton = HUDSettingsPrefab.transform.GetChild(0).GetChild(1).GetComponent<Button>();
            hudSettingsComponent.ResetButton = HUDSettingsPrefab.transform.GetChild(0).GetChild(2).GetComponent<Button>();

            foreach (var obj in settingsData.UIElementsToResizeAndDrag)
            {
                if (obj != null)
                {
                    var resizePanel = obj.AddComponent<ResizeImage>();
                    resizePanel.UniqueNameToSaveResizing = obj.name + " " + "Resize";

                    var dragPanel = obj.AddComponent<DragImage>();
                    dragPanel.UniqueNameToSaveDragging = obj.name + " " + "Dragging";
                }
            }
        }
        private void AddRequiredComponentsToUI()
        {
            foreach (var obj in settingsData.UIElementsToResizeAndDrag)
            {
                if (obj != null)
                {
                    var resizePanel = obj.AddComponent<ResizeImage>();
                    resizePanel.UniqueNameToSaveResizing = obj.name + " " + "Resize";

                    var dragPanel = obj.AddComponent<DragImage>();
                    dragPanel.UniqueNameToSaveDragging = obj.name + " " + "Dragging";
                }
            }
        }
    }
}





//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using UnityEngine.UI;

//namespace MobileActionKit
//{
//    public class HUDSettingsEditor : EditorWindow
//    {
//        private GameObject requiredScripts;
//        private GameObject canvas2D;
//        private GameObject pausePanel;
//        private Sprite spriteIcon;
//        private List<GameObject> adjusterUI = new List<GameObject>();

//        [MenuItem("Tools/MobileActionKit/Player/Player/Create HUD Settings", priority = 5)]
//        public static void ShowWindow()
//        {
//            GetWindow<HUDSettingsEditor>("HUD Settings");
//        }

//        private void OnGUI()
//        {
//            GUILayout.Label("HUD Settings", EditorStyles.boldLabel);

//            requiredScripts = (GameObject)EditorGUILayout.ObjectField("Required Scripts", requiredScripts, typeof(GameObject), true);
//            canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);
//            pausePanel = (GameObject)EditorGUILayout.ObjectField("Pause Panel", pausePanel, typeof(GameObject), true);
//            spriteIcon = (Sprite)EditorGUILayout.ObjectField("Sprite Adjuster Image", spriteIcon, typeof(Sprite), true);

//            GUILayout.Label("Adjuster UI", EditorStyles.boldLabel);
//            SerializedObject serializedObject = new SerializedObject(this);
//            SerializedProperty adjusterUIProperty = serializedObject.FindProperty("adjusterUI");
//            EditorGUILayout.PropertyField(adjusterUIProperty, true);
//            serializedObject.ApplyModifiedProperties();

//            if (GUILayout.Button("Add Image Component and Apply Sprite"))
//            {
//                AddImageComponentAndApplySprite();
//            }

//            if (GUILayout.Button("Create Required UI"))
//            {
//                CreateRequiredUI();
//            }

//            if (GUILayout.Button("Add Required Components"))
//            {
//                AddRequiredComponents();
//            }
//        }

//        private void AddImageComponentAndApplySprite()
//        {
//            if (spriteIcon == null)
//            {
//                Debug.LogError("Sprite Icon is not assigned.");
//                return;
//            }

//            foreach (var obj in adjusterUI)
//            {
//                if (obj != null)
//                {
//                    var image = obj.GetComponent<UnityEngine.UI.Image>();
//                    if (image == null)
//                    {
//                        image = obj.AddComponent<UnityEngine.UI.Image>();
//                    }
//                    image.sprite = spriteIcon;
//                }
//            }
//        }

//        private void CreateRequiredUI()
//        {
//            if (canvas2D == null)
//            {
//                Debug.LogError("Canvas 2D is not assigned.");
//                return;
//            }

//            string prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/HUD Settings.prefab";
//            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

//            if (prefab == null)
//            {
//                Debug.LogError("HUD Settings prefab not found at " + prefabPath);
//                return;
//            }

//            GameObject hudSettings = Instantiate(prefab);
//            hudSettings.transform.SetParent(canvas2D.transform, false);
//            PrefabUtility.UnpackPrefabInstance(hudSettings, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
//        }

//        private void AddRequiredComponents()
//        {
//            GameObject hudSettings = canvas2D?.transform.Find("HUD Settings")?.gameObject;

//            if (hudSettings == null)
//            {
//                Debug.LogError("HUD Settings is not instantiated or found under Canvas 2D.");
//                return;
//            }

//            GameObject emptyGO = new GameObject("EmptyGameObject");
//            emptyGO.transform.SetParent(hudSettings.transform);

//            var hudSettingsComponent = emptyGO.AddComponent<HUDSettings>();

//            hudSettingsComponent.HudSettingsPanel = hudSettings;
//            hudSettingsComponent.PausePanel = pausePanel;
//            hudSettingsComponent.DragButton = hudSettings.transform.GetChild(0).GetChild(4).GetComponent<Button>();
//            hudSettingsComponent.ResizeButton = hudSettings.transform.GetChild(0).GetChild(3).GetComponent <Button>();
//            hudSettingsComponent.SaveButton = hudSettings.transform.GetChild(0).GetChild(1).GetComponent <Button>();
//            hudSettingsComponent.ResetButton = hudSettings.transform.GetChild(0).GetChild(2).GetComponent <Button>();

//            foreach (var obj in adjusterUI)
//            {
//                if (obj != null)
//                {
//                    var resizePanel = obj.AddComponent<ResizePanel>();
//                    resizePanel.UniqueNameToSaveResizing = obj.name + "Resize";

//                    var dragPanel = obj.AddComponent<DragPanel>();
//                    dragPanel.UniqueNameToSaveDragging = obj.name + "Dragging";
//                }
//            }
//        }
//    }
//}