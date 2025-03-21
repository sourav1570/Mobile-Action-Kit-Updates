using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace MobileActionKit
{
    public class ArtillerySupportEditor : EditorWindow
    {
        private GameObject player;
        private GameObject canvas2D;
        private Button ArtillerySupportButton;
        private Transform playerCamera;
        private float TargetDesignatorRange = 100f;
        private float ShellsSpawnAltitudeFromImpactPoint = 10f;
        private int ShellsInBarrage = 5;
        private GameObject ArtilleryShellPrefab;
        private Vector3 MinSpawnOffset = Vector3.zero;
        private Vector3 maxSpawnOffset = Vector3.zero;
        private float MinArtilleryShellsInterval = 0.1f;
        private float MaxArtilleryShellsInterval = 0.5f;
        private float ArtilleryShellSpeed = 10f;
        private float BarrageDelay;
        private float ButtonReactivationDelay = 3f;
        private Color ActivatedButtonColor = Color.white;
        private Color DeactivatedButtonColor = Color.gray;

        [MenuItem("Tools/Mobile Action Kit/Player/Player/Create Artillery Support", priority = 3)]
        public static void ShowWindow()
        {
            GetWindow<ArtillerySupportEditor>("Artillery Support");
        }

        private void OnGUI()
        {
            GUILayout.Label("Artillery Support", EditorStyles.boldLabel);

            // Player Object Field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player", GUILayout.Width(200)); // Fixed width for label
            player = (GameObject)EditorGUILayout.ObjectField(player, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            // Canvas 2D Object Field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Canvas 2D", GUILayout.Width(200));
            canvas2D = (GameObject)EditorGUILayout.ObjectField(canvas2D, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Required UI"))
            {
                AddRequiredUI();
            }

            GUILayout.Space(10);
            GUILayout.Label("Artillery Support Fields", EditorStyles.boldLabel);

            // Artillery Support Button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Artillery Support Button", GUILayout.Width(200));
            ArtillerySupportButton = (Button)EditorGUILayout.ObjectField(ArtillerySupportButton, typeof(Button), true);
            EditorGUILayout.EndHorizontal();

            // Player Camera
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player Camera", GUILayout.Width(200));
            playerCamera = (Transform)EditorGUILayout.ObjectField(playerCamera, typeof(Transform), true);
            EditorGUILayout.EndHorizontal();

            // Target Designator Range
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Designator Range", GUILayout.Width(200));
            TargetDesignatorRange = EditorGUILayout.FloatField(TargetDesignatorRange);
            EditorGUILayout.EndHorizontal();

            // Shells Spawn Altitude From Impact Point
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Shells Spawn Altitude From Impact Point", GUILayout.Width(250));
            ShellsSpawnAltitudeFromImpactPoint = EditorGUILayout.FloatField(ShellsSpawnAltitudeFromImpactPoint);
            EditorGUILayout.EndHorizontal();

            // Shells In Barrage
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Shells In Barrage", GUILayout.Width(200));
            ShellsInBarrage = EditorGUILayout.IntField(ShellsInBarrage);
            EditorGUILayout.EndHorizontal();

            // Artillery Shell Prefab
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Artillery Shell Prefab", GUILayout.Width(200));
            ArtilleryShellPrefab = (GameObject)EditorGUILayout.ObjectField(ArtilleryShellPrefab, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            // Min Spawn Offset
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min Spawn Offset", GUILayout.Width(200));
            MinSpawnOffset = EditorGUILayout.Vector3Field("", MinSpawnOffset); // Empty label for alignment
            EditorGUILayout.EndHorizontal();

            // Max Spawn Offset
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max Spawn Offset", GUILayout.Width(200));
            maxSpawnOffset = EditorGUILayout.Vector3Field("", maxSpawnOffset);
            EditorGUILayout.EndHorizontal();

            // Min Artillery Shells Interval
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min Artillery Shells Interval", GUILayout.Width(200));
            MinArtilleryShellsInterval = EditorGUILayout.FloatField(MinArtilleryShellsInterval);
            EditorGUILayout.EndHorizontal();

            // Max Artillery Shells Interval
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max Artillery Shells Interval", GUILayout.Width(200));
            MaxArtilleryShellsInterval = EditorGUILayout.FloatField(MaxArtilleryShellsInterval);
            EditorGUILayout.EndHorizontal();

            // Artillery Shell Speed
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Artillery Shell Speed", GUILayout.Width(200));
            ArtilleryShellSpeed = EditorGUILayout.FloatField(ArtilleryShellSpeed);
            EditorGUILayout.EndHorizontal();

            // Barrage Delay
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Barrage Delay", GUILayout.Width(200));
            BarrageDelay = EditorGUILayout.FloatField(BarrageDelay);
            EditorGUILayout.EndHorizontal();

            // Button Reactivation Delay
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Button Reactivation Delay", GUILayout.Width(200));
            ButtonReactivationDelay = EditorGUILayout.FloatField(ButtonReactivationDelay);
            EditorGUILayout.EndHorizontal();

            // Activated Button Color
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activated Button Color", GUILayout.Width(200));
            ActivatedButtonColor = EditorGUILayout.ColorField(ActivatedButtonColor);
            EditorGUILayout.EndHorizontal();

            // Deactivated Button Color
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Deactivated Button Color", GUILayout.Width(200));
            DeactivatedButtonColor = EditorGUILayout.ColorField(DeactivatedButtonColor);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            if (GUILayout.Button("Setup Artillery Support"))
            {
                CreateArtillerySupport();
            }
        }


        //private void OnGUI()
        //{
        //    GUILayout.Label("Artillery Support", EditorStyles.boldLabel);
        //    player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);

        //    canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);

        //    if (GUILayout.Button("Add Required UI"))
        //    {
        //        AddRequiredUI();
        //    }

        //    GUILayout.Space(10);

        //    GUILayout.Label("Artillery Support Fields", EditorStyles.boldLabel);
        //    ArtillerySupportButton = (Button)EditorGUILayout.ObjectField("Artillery Support Button", ArtillerySupportButton, typeof(Button), true);
        //    playerCamera = (Transform)EditorGUILayout.ObjectField("Player Camera", playerCamera, typeof(Transform), true);
        //    TargetDesignatorRange = EditorGUILayout.FloatField("Target Designator Range", TargetDesignatorRange);
        //    ShellsSpawnAltitudeFromImpactPoint = EditorGUILayout.FloatField("Shells Spawn Altitude From Impact Point", ShellsSpawnAltitudeFromImpactPoint);
        //    ShellsInBarrage = EditorGUILayout.IntField("Shells In Barrage", ShellsInBarrage);
        //    ArtilleryShellPrefab = (GameObject)EditorGUILayout.ObjectField("Artillery Shell Prefabb", ArtilleryShellPrefab, typeof(GameObject), false);
        //    MinSpawnOffset = EditorGUILayout.Vector3Field("Min Spawn Offset", MinSpawnOffset);
        //    maxSpawnOffset = EditorGUILayout.Vector3Field("Max Spawn Offset", maxSpawnOffset);
        //    MinArtilleryShellsInterval = EditorGUILayout.FloatField("Min Artillery Shells Interval", MinArtilleryShellsInterval);
        //    MaxArtilleryShellsInterval = EditorGUILayout.FloatField("Max Artillery Shells Interval", MaxArtilleryShellsInterval);
        //    ArtilleryShellSpeed = EditorGUILayout.FloatField("Artillery Shell Speed", ArtilleryShellSpeed);

        //    BarrageDelay = EditorGUILayout.FloatField("Barrage Delay", BarrageDelay);

        //    ButtonReactivationDelay = EditorGUILayout.FloatField("Button Reactivation Delay", ButtonReactivationDelay);
        //    ActivatedButtonColor = EditorGUILayout.ColorField("Activated Button Color", ActivatedButtonColor);
        //    DeactivatedButtonColor = EditorGUILayout.ColorField("Deactivated Button Color", DeactivatedButtonColor);

        //    GUILayout.Space(20);

        //    if (GUILayout.Button("Create Artillery Support"))
        //    {
        //        CreateArtillerySupport();
        //    }
        //}

        private void CreateArtillerySupport()
        {
            if (player == null)
            {
                Debug.LogError("Player is not assigned.");
                return;
            }

            // Check if MortarAttack script is attached
            PlayerArtillerySupport mortarAttack = player.GetComponent<PlayerArtillerySupport>();
            if (mortarAttack == null)
            {
                mortarAttack = player.AddComponent<PlayerArtillerySupport>();
            }

            // Assign fields
            mortarAttack.ArtillerySupportButton = ArtillerySupportButton;
            mortarAttack.PlayerCamera = playerCamera;
            mortarAttack.TargetDesignatorRange = TargetDesignatorRange;
            mortarAttack.ShellsSpawnAltitudeFromImpactPoint = ShellsSpawnAltitudeFromImpactPoint;
            mortarAttack.ShellsInBarrage = ShellsInBarrage;
            mortarAttack.ArtilleryShellPrefab = ArtilleryShellPrefab;
            mortarAttack.MinSpawnOffset = MinSpawnOffset;
            mortarAttack.MaxSpawnOffset = maxSpawnOffset;
            mortarAttack.MinArtilleryShellsInterval = MinArtilleryShellsInterval;
            mortarAttack.MaxArtilleryShellsInterval = MaxArtilleryShellsInterval;
            mortarAttack.ArtilleryShellSpeed = ArtilleryShellSpeed;

            mortarAttack.BarrageDelay = BarrageDelay;

            mortarAttack.ButtonReactivationDelay = ButtonReactivationDelay;
            mortarAttack.ActivatedButtonColor = ActivatedButtonColor;
            mortarAttack.DeactivatedButtonColor = DeactivatedButtonColor;

            Debug.Log("Artillery Support created and fields assigned.");
        }

        private void AddRequiredUI()
        {
            if (canvas2D == null)
            {
                Debug.LogError("Canvas 2D is not assigned.");
                return;
            }

            string prefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/Artillary Support UI.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                return;
            }

            GameObject buttonInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas2D.transform);
            if (buttonInstance != null)
            {
                buttonInstance.name = "Artillary Support UI";

                // Unpack the prefab
                PrefabUtility.UnpackPrefabInstance(buttonInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            else
            {
                Debug.LogError("Failed to instantiate Artillary Support UI.");
            }
        }
    }
}