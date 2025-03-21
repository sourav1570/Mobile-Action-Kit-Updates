using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class SetupPCControlsWindow : EditorWindow
    {
        public GameObject player;
        public GameObject canvas2D;
        public FirstPersonController firstPersonControllerScript;
        public Camera playerCamera;

        [MenuItem("Tools/Mobile Action Kit/Setup PC Controls")]
        public static void ShowWindow()
        {
            GetWindow<SetupPCControlsWindow>("Setup PC Controls");
        }
        private void OnGUI()
        {
            GUILayout.Label("Setup PC Controls", EditorStyles.boldLabel);
            GUILayout.Space(10); // Adds some spacing after the label

            EditorGUIUtility.labelWidth = 200; // Set label width to 200 pixels

            // Player GameObject field
            player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);

            // Canvas2D GameObject field
            canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas2D", canvas2D, typeof(GameObject), true);

            // First Person Controller Script field
            firstPersonControllerScript = (FirstPersonController)EditorGUILayout.ObjectField("First Person Controller Script", firstPersonControllerScript, typeof(FirstPersonController), true);

            // Player Camera field
            playerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", playerCamera, typeof(Camera), true);

            GUILayout.Space(10); // Adds some spacing before the button

            // Setup Button
            if (GUILayout.Button("Setup", GUILayout.Height(30))) // Increased button height for better visibility
            {
                SetupPCControls();
            }
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Setup PC Controls", EditorStyles.boldLabel);

        //    // Player GameObject field
        //    player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);

        //    // Canvas2D GameObject field
        //    canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas2D", canvas2D, typeof(GameObject), true);

        //    // First Person Controller Script field
        //    firstPersonControllerScript = (MonoBehaviour)EditorGUILayout.ObjectField("First Person Controller Script", firstPersonControllerScript, typeof(MonoBehaviour), true);

        //    // Player Camera field
        //    playerCamera = (Camera)EditorGUILayout.ObjectField("Player Camera", playerCamera, typeof(Camera), true);

        //    // Setup Button
        //    if (GUILayout.Button("Setup"))
        //    {
        //        SetupPCControls();
        //    }
        //}

        private void SetupPCControls()
        {
            if (player == null || canvas2D == null || firstPersonControllerScript == null || playerCamera == null)
            {
                Debug.LogError("Please assign all required fields.");
                return;
            }

            // Spawn and setup PC_Controls_Settings prefab
            GameObject pcControlsSettingsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mobile Action Kit/PC Controls/Editor/Editor Prefabs/PC_Controls_Settings.prefab");
            GameObject pcControlsSettingsInstance = PrefabUtility.InstantiatePrefab(pcControlsSettingsPrefab) as GameObject;
            pcControlsSettingsInstance.transform.SetParent(canvas2D.transform, false);
            PrefabUtility.UnpackPrefabInstance(pcControlsSettingsInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            Transform PcControls = pcControlsSettingsInstance.transform.GetChild(2);
            PcControls.parent = null;
            PcControls.position = Vector3.zero;
            PcControls.eulerAngles = Vector3.zero;
            PcControls.localScale = Vector3.one;

            // Uncheck EnablePcMouseRotation in the First Person Controller Script
            SerializedObject serializedScript = new SerializedObject(firstPersonControllerScript);
            SerializedProperty UsePcMouseRotationProp = serializedScript.FindProperty("UsePcMouseLookScripts");
            UsePcMouseRotationProp.boolValue = true;
            serializedScript.ApplyModifiedProperties();

            // Add and configure PcMouseLook script on the Player GameObject
            PcMouseLook playerMouseLook = player.AddComponent<PcMouseLook>();
            playerMouseLook.axes = PcMouseLook.RotationAxes.Horizontal;
            playerMouseLook.HideCursor = false;
            playerMouseLook.WeaponDefaultMouseAxes.XSensitivitySpeed = 800f;
            playerMouseLook.WeaponDefaultMouseAxes.YSensitivitySpeed = 800f;
            playerMouseLook.WeaponDefaultMouseAxes.MinVerticalAxis = 0f;
            playerMouseLook.WeaponDefaultMouseAxes.MaxVerticalAxis = 0f;
            playerMouseLook.WeaponDefaultMouseAxes.MinHorizontalAxis = -360f;
            playerMouseLook.WeaponDefaultMouseAxes.MaxHorizontalAxis = 360f;
            playerMouseLook.WeaponAimedMouseAxes.XSensitivitySpeed = 400f;
            playerMouseLook.WeaponAimedMouseAxes.YSensitivitySpeed = 400f;
            playerMouseLook.WeaponAimedMouseAxes.MinVerticalAxis = 0f;
            playerMouseLook.WeaponAimedMouseAxes.MaxVerticalAxis = 0f;
            playerMouseLook.WeaponAimedMouseAxes.MinHorizontalAxis = -360f;
            playerMouseLook.WeaponAimedMouseAxes.MaxHorizontalAxis = 360f;

            // Add and configure PcMouseLook script on the Player Camera GameObject
            PcMouseLook cameraMouseLook = playerCamera.gameObject.AddComponent<PcMouseLook>();
            cameraMouseLook.axes = PcMouseLook.RotationAxes.Vertical;
            cameraMouseLook.HideCursor = false;
            cameraMouseLook.WeaponDefaultMouseAxes.XSensitivitySpeed = 800f;
            cameraMouseLook.WeaponDefaultMouseAxes.YSensitivitySpeed = 800f;
            cameraMouseLook.WeaponDefaultMouseAxes.MinVerticalAxis = -90f;
            cameraMouseLook.WeaponDefaultMouseAxes.MaxVerticalAxis = 90f;
            cameraMouseLook.WeaponDefaultMouseAxes.MinHorizontalAxis = 0f;
            cameraMouseLook.WeaponDefaultMouseAxes.MaxHorizontalAxis = 0f;
            cameraMouseLook.WeaponAimedMouseAxes.XSensitivitySpeed = 400f;
            cameraMouseLook.WeaponAimedMouseAxes.YSensitivitySpeed = 400f;
            cameraMouseLook.WeaponAimedMouseAxes.MinVerticalAxis = -90f;
            cameraMouseLook.WeaponAimedMouseAxes.MaxVerticalAxis = 90f;
            cameraMouseLook.WeaponAimedMouseAxes.MinHorizontalAxis = 0f;
            cameraMouseLook.WeaponAimedMouseAxes.MaxHorizontalAxis = 0f;

            Debug.Log("PC Controls Setup Completed.");
        }
    }
}