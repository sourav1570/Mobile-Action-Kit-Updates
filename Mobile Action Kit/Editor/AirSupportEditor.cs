using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace MobileActionKit
{
    public class AirSupportEditor : EditorWindow
    {
        private GameObject player;
        private GameObject canvas2D;

        private Button airSupportUIButton;
        private Transform playerCamera;
        private GameObject AirSupportPlanePrefab;
        private float TargetDesignatorRange;
        private float JetSpawnAltitude;
        private float PayloadReleaseAltitude;
        private float MinSpawnPointDistanceFromHitPoint;
        private float MaxSpawnPointDistanceFromHitPoint;
        private bool EnableDiveAttack;
        private int NumberOfPlanesToSpawn;

        private float MinPlaneSpawnDelay;
        private float MaxPlaneSpawnDelay;

        private Vector3 MinDistanceBetweenAircraft;
        private Vector3 MaxDistanceBetweenAircraft;

        private Vector3 MinTargetAreaRadius;
        private Vector3 MaxTargetAreaRadius;

        private bool AttackTargetAreaFromtheRight;
        private bool AttackTargetAreaFromtheLeft;
        private bool AttackTargetAreaFromtheFront;
        private bool AttackTargetAreaFromtheRear;

        private float AirSupportButtonResetDelay;
        private Color ActivatedButtonColor;
        private Color DeactivatedButtonColor;
        private TextMeshProUGUI airSupportAvailablityText;
        private string MessageIfAirSupportAvailable;
        private string MessageIfAirSupportUnavailable;
        private float TextMessageDisplayDuration;

        [MenuItem("Tools/Mobile Action Kit/Player/Player/Create Air Support", priority = 4)]
        public static void ShowWindow()
        {
            GetWindow<AirSupportEditor>("Air Support Setup");
        }
        private void OnGUI()
        {
            GUILayout.Label("Air Support Setup", EditorStyles.boldLabel);

            // Player field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player", GUILayout.Width(200));
            player = (GameObject)EditorGUILayout.ObjectField(player, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            // Canvas 2D field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Canvas 2D", GUILayout.Width(200));
            canvas2D = (GameObject)EditorGUILayout.ObjectField(canvas2D, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Required UI"))
            {
                AddRequiredUI();
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Air Support Script Fields", EditorStyles.boldLabel);

            // Script fields
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Air Support UI Button", GUILayout.Width(200));
            airSupportUIButton = (Button)EditorGUILayout.ObjectField(airSupportUIButton, typeof(Button), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Player Camera", GUILayout.Width(200));
            playerCamera = (Transform)EditorGUILayout.ObjectField(playerCamera, typeof(Transform), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Air Support Plane Prefab", GUILayout.Width(200));
            AirSupportPlanePrefab = (GameObject)EditorGUILayout.ObjectField(AirSupportPlanePrefab, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            AddFloatField("Target Designator Range", ref TargetDesignatorRange);
            AddFloatField("Jet Spawn Altitude", ref JetSpawnAltitude);
            AddFloatField("Payload Release Altitude", ref PayloadReleaseAltitude);
            AddFloatField("Min Spawn Point Distance From Hit Point", ref MinSpawnPointDistanceFromHitPoint);
            AddFloatField("Max Spawn Point Distance From Hit Point", ref MaxSpawnPointDistanceFromHitPoint);

            AddToggleField("Enable Dive Attack", ref EnableDiveAttack);

            AddIntField("Number Of Planes To Spawn", ref NumberOfPlanesToSpawn);
            AddFloatField("Min Plane Spawn Delay", ref MinPlaneSpawnDelay);
            AddFloatField("Max Plane Spawn Delay", ref MaxPlaneSpawnDelay);

            AddVector3Field("Min Distance Between Aircraft", ref MinDistanceBetweenAircraft);
            AddVector3Field("Max Distance Between Aircraft", ref MaxDistanceBetweenAircraft);

            AddVector3Field("Min Target Area Radius", ref MinTargetAreaRadius);
            AddVector3Field("Max Target Area Radius", ref MaxTargetAreaRadius);

            AddToggleField("Attack Target Area From The Right", ref AttackTargetAreaFromtheRight);
            AddToggleField("Attack Target Area From The Left", ref AttackTargetAreaFromtheLeft);
            AddToggleField("Attack Target Area From The Front", ref AttackTargetAreaFromtheFront);
            AddToggleField("Attack Target Area From The Rear", ref AttackTargetAreaFromtheRear);

            AddFloatField("Air Support Button Reset Delay", ref AirSupportButtonResetDelay);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activated Button Color", GUILayout.Width(200));
            ActivatedButtonColor = EditorGUILayout.ColorField(ActivatedButtonColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Deactivated Button Color", GUILayout.Width(200));
            DeactivatedButtonColor = EditorGUILayout.ColorField(DeactivatedButtonColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Air Support Availability Text", GUILayout.Width(200));
            airSupportAvailablityText = (TextMeshProUGUI)EditorGUILayout.ObjectField(airSupportAvailablityText, typeof(TextMeshProUGUI), true);
            EditorGUILayout.EndHorizontal();

            AddTextField("Message If Air Support Available", ref MessageIfAirSupportAvailable);
            AddTextField("Message If Air Support Unavailable", ref MessageIfAirSupportUnavailable);
            AddFloatField("Text Message Display Duration", ref TextMessageDisplayDuration);

            GUILayout.Space(10);

            // Buttons
            if (GUILayout.Button("Create Air Support"))
            {
                AssignAirSupportValues();
            }
        }

        /// <summary>
        /// Helper method for adding float fields with fixed label width.
        /// </summary>
        private void AddFloatField(string label, ref float value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(250));
            value = EditorGUILayout.FloatField(value);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Helper method for adding integer fields with fixed label width.
        /// </summary>
        private void AddIntField(string label, ref int value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(250));
            value = EditorGUILayout.IntField(value);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Helper method for adding toggle fields with fixed label width.
        /// </summary>
        private void AddToggleField(string label, ref bool value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(250));
            value = EditorGUILayout.Toggle(value);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Helper method for adding Vector3 fields with fixed label width.
        /// </summary>
        private void AddVector3Field(string label, ref Vector3 value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(250));
            value = EditorGUILayout.Vector3Field("", value); // Empty label for alignment
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Helper method for adding text fields with fixed label width.
        /// </summary>
        private void AddTextField(string label, ref string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(250));
            value = EditorGUILayout.TextField(value);
            EditorGUILayout.EndHorizontal();
        }


        //private void OnGUI()
        //{
        //    GUILayout.Label("Air Support Setup", EditorStyles.boldLabel);

        //    // Player field
        //    player = (GameObject)EditorGUILayout.ObjectField("Player", player, typeof(GameObject), true);

        //    // Canvas 2D field
        //    canvas2D = (GameObject)EditorGUILayout.ObjectField("Canvas 2D", canvas2D, typeof(GameObject), true);

        //    if (GUILayout.Button("Add Required UI"))
        //    {
        //        AddRequiredUI();
        //    }

        //    GUILayout.Space(10);
        //    EditorGUILayout.LabelField("Air Support Script Fields", EditorStyles.boldLabel);

        //    // Script fields
        //    airSupportUIButton = (Button)EditorGUILayout.ObjectField("Air Support UI Button", airSupportUIButton, typeof(Button), true);
        //    playerCamera = (Transform)EditorGUILayout.ObjectField("Player Camera", playerCamera, typeof(Transform), true);
        //    AirSupportPlanePrefab = (GameObject)EditorGUILayout.ObjectField("Air Support Plane Prefab", AirSupportPlanePrefab, typeof(GameObject), true);

        //    TargetDesignatorRange = EditorGUILayout.FloatField("Target Designator Range", TargetDesignatorRange);
        //    JetSpawnAltitude = EditorGUILayout.FloatField("Jet Spawn Altitude", JetSpawnAltitude);
        //    PayloadReleaseAltitude = EditorGUILayout.FloatField("Payload Release Altitude", PayloadReleaseAltitude);
        //    MinSpawnPointDistanceFromHitPoint = EditorGUILayout.FloatField("Min Spawn Point Distance From Hit Point", MinSpawnPointDistanceFromHitPoint);
        //    MaxSpawnPointDistanceFromHitPoint = EditorGUILayout.FloatField("Max Spawn Point Distance From Hit Point", MaxSpawnPointDistanceFromHitPoint);
        //    EnableDiveAttack = EditorGUILayout.Toggle("Enable Dive Attack", EnableDiveAttack);
        //    NumberOfPlanesToSpawn = EditorGUILayout.IntField("Number Of Planes To Spawn", NumberOfPlanesToSpawn);

        //    MinPlaneSpawnDelay = EditorGUILayout.IntField("Min Plane Spawn Delay", MinPlaneSpawnDelay);
        //    MaxPlaneSpawnDelay = EditorGUILayout.IntField("Max Plane Spawn Delay", MaxPlaneSpawnDelay);

        //    MinDistanceBetweenAircraft = EditorGUILayout.Vector3Field("Min Distance Between Aircraft", MinDistanceBetweenAircraft);
        //    MaxDistanceBetweenAircraft = EditorGUILayout.Vector3Field("Max Distance Between Aircraft", MaxDistanceBetweenAircraft);

        //    MinTargetAreaRadius = EditorGUILayout.Vector3Field("Min Target Area Radius", MinTargetAreaRadius);
        //    MaxTargetAreaRadius = EditorGUILayout.Vector3Field("Max Target Area Radius", MaxTargetAreaRadius);

        //    AttackTargetAreaFromtheRight = EditorGUILayout.Toggle("Attack Target Area From The Right", AttackTargetAreaFromtheRight);
        //    AttackTargetAreaFromtheLeft = EditorGUILayout.Toggle("Attack Target Area From The Left", AttackTargetAreaFromtheLeft);
        //    AttackTargetAreaFromtheFront = EditorGUILayout.Toggle("Attack Target Area From The Front", AttackTargetAreaFromtheFront);
        //    AttackTargetAreaFromtheRear = EditorGUILayout.Toggle("Attack Target Area From The Rear", AttackTargetAreaFromtheRear);

        //    AirSupportButtonResetDelay = EditorGUILayout.FloatField("Air Support Button Reset Delay", AirSupportButtonResetDelay);

        //    ActivatedButtonColor = EditorGUILayout.ColorField("Activated Button Color", ActivatedButtonColor);
        //    DeactivatedButtonColor = EditorGUILayout.ColorField("Deactivated Button Color", DeactivatedButtonColor);

        //    airSupportAvailablityText = (TextMeshProUGUI)EditorGUILayout.ObjectField("Air Support Availablity Text", airSupportAvailablityText, typeof(TextMeshProUGUI), true);
        //    MessageIfAirSupportAvailable = EditorGUILayout.TextField("Message If Air Support Available", MessageIfAirSupportAvailable);
        //    MessageIfAirSupportUnavailable = EditorGUILayout.TextField("Message If Air Support Unavailable", MessageIfAirSupportUnavailable);
        //    TextMessageDisplayDuration = EditorGUILayout.FloatField("Text Message Display Duration", TextMessageDisplayDuration);

        //    GUILayout.Space(10);

        //    // Buttons
        //    if (GUILayout.Button("Create Air Support"))
        //    {
        //        AssignAirSupportValues();
        //    }
        //}

        private void AssignAirSupportValues()
        {
            if (player != null)
            {
                PlayerAirSupport airSupportCall = player.GetComponent<PlayerAirSupport>();
                if (airSupportCall == null)
                {
                    airSupportCall = player.AddComponent<PlayerAirSupport>();
                }

                // Assign values
                airSupportCall.AirSupportUIButton = airSupportUIButton;
                airSupportCall.PlayerCamera = playerCamera;
                airSupportCall.AirSupportPlanePrefab = AirSupportPlanePrefab;

                airSupportCall.TargetDesignatorRange = TargetDesignatorRange;
                airSupportCall.JetSpawnAltitude = JetSpawnAltitude;
                airSupportCall.PayloadReleaseAltitude = PayloadReleaseAltitude;
                airSupportCall.MinSpawnPointDistanceFromHitPoint = MinSpawnPointDistanceFromHitPoint;
                airSupportCall.MaxSpawnPointDistanceFromHitPoint = MaxSpawnPointDistanceFromHitPoint;
                airSupportCall.EnableDiveAttack = EnableDiveAttack;
                airSupportCall.NumberOfPlanesToSpawn = NumberOfPlanesToSpawn;
                airSupportCall.MinPlaneSpawnDelay = MinPlaneSpawnDelay;
                airSupportCall.MaxPlaneSpawnDelay = MaxPlaneSpawnDelay;


                airSupportCall.MinDistanceBetweenAircraft = MinDistanceBetweenAircraft;
                airSupportCall.MaxDistanceBetweenAircraft = MaxDistanceBetweenAircraft;
                airSupportCall.MinTargetAreaRadius = MinTargetAreaRadius;
                airSupportCall.MinTargetAreaRadius = MaxTargetAreaRadius;
                airSupportCall.AttackTargetAreaFromTheRight = AttackTargetAreaFromtheRight;
                airSupportCall.AttackTargetAreaFromTheLeft = AttackTargetAreaFromtheLeft;
                airSupportCall.AttackTargetAreaFromTheFront = AttackTargetAreaFromtheFront;
                airSupportCall.AttackTargetAreaFromTheRear = AttackTargetAreaFromtheRear;
                airSupportCall.AirSupportButtonResetDelay = AirSupportButtonResetDelay;
                airSupportCall.ActivatedButtonColor = ActivatedButtonColor;
                airSupportCall.DeactivatedButtonColor = DeactivatedButtonColor;
                airSupportCall.AirSupportAvailablityText = airSupportAvailablityText;
                airSupportCall.MessageIfAirSupportAvailable = MessageIfAirSupportAvailable;
                airSupportCall.MessageIfAirSupportUnavailable = MessageIfAirSupportUnavailable;
                airSupportCall.TextMessageDisplayDuration = TextMessageDisplayDuration;

                EditorUtility.SetDirty(airSupportCall);
            }
            else
            {
                Debug.LogError("Player GameObject is not assigned!");
            }
        }

        private void AddRequiredUI()
        {
            if (canvas2D != null)
            {
                string airSupportUIPrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/Air Support UI.prefab";
                string jetInfoPrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/Jet Info.prefab";

                // Load and instantiate the Air Support UI prefab
                GameObject airSupportUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(airSupportUIPrefabPath);
                if (airSupportUIPrefab != null)
                {
                    GameObject instantiatedUI = (GameObject)PrefabUtility.InstantiatePrefab(airSupportUIPrefab);
                    instantiatedUI.transform.SetParent(canvas2D.transform, false);

                    PrefabUtility.UnpackPrefabInstance(instantiatedUI, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    Debug.Log("Air Support UI added to Canvas 2D.");
                }
                else
                {
                    Debug.LogError($"Air Support UI prefab not found at path: {airSupportUIPrefabPath}");
                }

                // Load and instantiate the Jet Info prefab
                GameObject jetInfoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(jetInfoPrefabPath);
                if (jetInfoPrefab != null)
                {
                    GameObject instantiatedJetInfo = (GameObject)PrefabUtility.InstantiatePrefab(jetInfoPrefab);
                    instantiatedJetInfo.transform.SetParent(canvas2D.transform, false);

                    PrefabUtility.UnpackPrefabInstance(instantiatedJetInfo, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    Debug.Log("Jet Info added to Canvas 2D.");
                }
                else
                {
                    Debug.LogError($"Jet Info prefab not found at path: {jetInfoPrefabPath}");
                }

                EditorUtility.SetDirty(canvas2D);
            }
            else
            {
                Debug.LogError("Canvas 2D is not assigned!");
            }
        }

    }
}
