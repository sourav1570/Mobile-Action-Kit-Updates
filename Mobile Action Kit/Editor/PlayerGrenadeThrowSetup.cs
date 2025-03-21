using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class PlayerGrenadeThrowSetup : EditorWindow
    {
        public bool IsAlreadySetup = false;

        [System.Serializable]
        public enum ThrowType
        {
            ThrowGrenadeInstantly,
            SelectPlayerGrenadeHands
        }

        public ThrowType PlayerGrenadeThrowType;

        private GameObject player;
        private GameObject weaponsRoot;

        private Camera PlayerCamera;
        private GameObject GrenadeIcon;

        private GameObject playerGrenadeHandsTemplate;
        private GameObject GrenadePrefab;

        private GameObject playerGrenadeHandsModel;
        private PlayerManager PlayerManagerScript;
        private Animator animator;
        private RuntimeAnimatorController runtimeAnimatorController;

        private float StandRunAnimationSpeed;
        private float CrouchRunAnimationSpeed;
        private float StandWalkAnimationSpeed;
        private float CrouchWalkAnimationSpeed;

        private AudioClip wieldAudioClip;
        private AudioClip grenadePinPullAudioClip;
        private AudioClip grenadeThrowAudioClip;

        GameObject spawnedTemplate;
        GameObject spawnedThrower;

        private PlayerGrenadeThrower PlayerGrenadeThrowerScript;
        private GameObject PlayerGrenadeBobberGameobject;


        [MenuItem("Tools/Mobile Action Kit/Player/Grenade/Player Grenade Throw Setup", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<PlayerGrenadeThrowSetup>("Player Grenade Throw Setup");
        }
        private void OnGUI()
        {
            // Title
            GUILayout.Label("Player Grenade Throw Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Is Already Setup Toggle
            IsAlreadySetup = EditorGUILayout.Toggle("IS ALREADY SETUP", IsAlreadySetup);

            // Grenade Throw Type Enum Popup
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player Grenade Throw Type", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f)); // 40% for the label        
            PlayerGrenadeThrowType = (ThrowType)EditorGUILayout.EnumPopup(PlayerGrenadeThrowType);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (!IsAlreadySetup) // If not already setup
            {
                // Player Setup Section
                GUILayout.Label("Player Setup", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");

                // Other Fields
                CreateDynamicField("Player", ref player, typeof(GameObject));
                CreateDynamicField("Weapons Root", ref weaponsRoot, typeof(GameObject));
                CreateDynamicField("Player Camera", ref PlayerCamera, typeof(Camera));
                CreateDynamicField("Grenade Icon", ref GrenadeIcon, typeof(GameObject));
                CreateDynamicField("Player Grenade Hands Template", ref playerGrenadeHandsTemplate, typeof(GameObject));
                CreateDynamicField("Grenade Prefab", ref GrenadePrefab, typeof(GameObject));
                CreateDynamicField("Animator", ref animator, typeof(Animator));
                CreateDynamicField("Animator Controller", ref runtimeAnimatorController, typeof(RuntimeAnimatorController));
                CreateDynamicField("Player Grenade Hands Model", ref playerGrenadeHandsModel, typeof(GameObject));
                CreateDynamicField("Player Manager Script", ref PlayerManagerScript, typeof(PlayerManager));

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);

                // Animation Speeds Section
                GUILayout.Label("Animation Speeds", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");

                StandRunAnimationSpeed = CreateDynamicFloatField("Stand Run Speed", StandRunAnimationSpeed);
                CrouchRunAnimationSpeed = CreateDynamicFloatField("Crouch Run Speed", CrouchRunAnimationSpeed);
                StandWalkAnimationSpeed = CreateDynamicFloatField("Stand Walk Speed", StandWalkAnimationSpeed);
                CrouchWalkAnimationSpeed = CreateDynamicFloatField("Crouch Walk Speed", CrouchWalkAnimationSpeed);

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);

                // Audio Clips Section
                GUILayout.Label("Audio Clips", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");

                CreateDynamicField("Wield Audio", ref wieldAudioClip, typeof(AudioClip));
                CreateDynamicField("Grenade Pin Pull Audio", ref grenadePinPullAudioClip, typeof(AudioClip));
                CreateDynamicField("Grenade Throw Audio", ref grenadeThrowAudioClip, typeof(AudioClip));

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
            }
            else // If already setup
            {
                GUILayout.Label("Drag and Drop GameObjects", EditorStyles.boldLabel);

                GUILayout.Space(10);
                // Setup fields when already setup
                CreateDynamicField("Player Grenade Hands Model", ref playerGrenadeHandsModel, typeof(GameObject));
                CreateDynamicField("Player Grenade Thrower Script", ref PlayerGrenadeThrowerScript, typeof(PlayerGrenadeThrower));
                CreateDynamicField("Player Grenade Bobber Gameobject", ref PlayerGrenadeBobberGameobject, typeof(GameObject));
                CreateDynamicField("Grenade Prefab", ref GrenadePrefab, typeof(GameObject));

                GUILayout.Space(10);

                CreateDynamicField("Player Camera", ref PlayerCamera, typeof(Camera));
                CreateDynamicField("Grenade Icon", ref GrenadeIcon, typeof(GameObject));
                CreateDynamicField("Animator", ref animator, typeof(Animator));
                CreateDynamicField("Player Manager Script", ref PlayerManagerScript, typeof(PlayerManager));
                CreateDynamicField("Wield Audio", ref wieldAudioClip, typeof(AudioClip));
                CreateDynamicField("Grenade Pin Pull Audio", ref grenadePinPullAudioClip, typeof(AudioClip));
                CreateDynamicField("Grenade Throw Audio", ref grenadeThrowAudioClip, typeof(AudioClip));

                GUILayout.Space(10);

                // Animation Speeds Section
                GUILayout.Label("Animation Speeds", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");

                StandRunAnimationSpeed = CreateDynamicFloatField("Stand Run Speed", StandRunAnimationSpeed);
                CrouchRunAnimationSpeed = CreateDynamicFloatField("Crouch Run Speed", CrouchRunAnimationSpeed);
                StandWalkAnimationSpeed = CreateDynamicFloatField("Stand Walk Speed", StandWalkAnimationSpeed);
                CrouchWalkAnimationSpeed = CreateDynamicFloatField("Crouch Walk Speed", CrouchWalkAnimationSpeed);

                EditorGUILayout.EndVertical();
            


            }

            // Buttons Section
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Centers the buttons
            if (GUILayout.Button("Setup Grenade Throw Ability", GUILayout.Width(200), GUILayout.Height(30)))
            {
                SetupGrenadeThrowAbility();
            }

            if (GUILayout.Button("Edit Animations", GUILayout.Width(200), GUILayout.Height(30)))
            {
                EditAnimations();
            }
            GUILayout.FlexibleSpace(); // Centers the buttons
            EditorGUILayout.EndHorizontal();
        }
        private void CreateDynamicField<T>(string label, ref T field, System.Type type) where T : UnityEngine.Object
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f)); // 40% for the label
            field = (T)EditorGUILayout.ObjectField(field, type, true);
            EditorGUILayout.EndHorizontal();
        }
        private float CreateDynamicFloatField(string label, float value)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f)); // 40% for the label
            value = EditorGUILayout.FloatField(value);
            EditorGUILayout.EndHorizontal();
            return value;
        }
        private void EditAnimations()
        {
            EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Player/Grenade/Edit Grenade Animations");
        }
        private void SetupGrenadeThrowAbility()
        {
            if(IsAlreadySetup == false)
            {
                if (PlayerGrenadeThrowType == ThrowType.SelectPlayerGrenadeHands)
                {
                    AutoSetup();
                    if (PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>() != null)
                    {
                        PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().GrenadeUtilizationType = "Selection";
                    }
                }
                else
                {
                    AutoSetup();
                    if (PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>() != null)
                    {
                        PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().GrenadeUtilizationType = "Immediate Throw";
                    }
                    if (PlayerManagerScript.BobbingScripts.Contains(spawnedTemplate))
                    {
                        PlayerManagerScript.BobbingScripts.Remove(spawnedTemplate);
                    }
                    if (PlayerManagerScript.PlayerWeaponScripts.Contains(playerGrenadeHandsModel.GetComponent<PlayerWeapon>()))
                    {
                        PlayerManagerScript.PlayerWeaponScripts.Remove(playerGrenadeHandsModel.GetComponent<PlayerWeapon>());
                    }

                    PlayerWeapon weapon = playerGrenadeHandsModel.GetComponent<PlayerWeapon>();
                    if (weapon != null)
                    {
                        Object.DestroyImmediate(weapon);
                    }
                }
            }
            else
            {
                if (PlayerGrenadeThrowType == ThrowType.SelectPlayerGrenadeHands)
                {
                    ModifyExistingSetup();
                    if (PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>() != null)
                    {
                        PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().GrenadeUtilizationType = "Selection";
                    }
                }
                else
                {
                   
                    ModifyExistingSetup();
                    if (PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>() != null)
                    {
                        PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().GrenadeUtilizationType = "Immediate Throw";
                    }
                    if (PlayerManagerScript.BobbingScripts.Contains(PlayerGrenadeBobberGameobject))
                    {
                        PlayerManagerScript.BobbingScripts.Remove(PlayerGrenadeBobberGameobject);
                    }
                    if (PlayerManagerScript.PlayerWeaponScripts.Contains(playerGrenadeHandsModel.GetComponent<PlayerWeapon>()))
                    {
                        PlayerManagerScript.PlayerWeaponScripts.Remove(playerGrenadeHandsModel.GetComponent<PlayerWeapon>());
                    }

                    PlayerWeapon weapon = playerGrenadeHandsModel.GetComponent<PlayerWeapon>();
                    if (weapon != null)
                    {
                        Object.DestroyImmediate(weapon);
                    }
                }
            }                  
        }
        private void ModifyExistingSetup()
        {
            PlayerGrenadeBobberGameobject.GetComponent<ProceduralIdleBobbing>().PlayerManagerScript = PlayerManagerScript;

            if (!PlayerManagerScript.BobbingScripts.Contains(PlayerGrenadeBobberGameobject))
            {
                PlayerManagerScript.BobbingScripts.Add(PlayerGrenadeBobberGameobject);
            }

            // Add components to Player Grenade Hands Model
            if (playerGrenadeHandsModel.GetComponent<GameobjectReactivationManager>() == null)
            {
                playerGrenadeHandsModel.AddComponent<GameobjectReactivationManager>();
            }
            if (playerGrenadeHandsModel.GetComponent<GameobjectActivationManager>() == null)
            {
                playerGrenadeHandsModel.AddComponent<GameobjectActivationManager>();
            }
            if (playerGrenadeHandsModel.GetComponent<PlayerWeapon>() == null)
            {
                playerGrenadeHandsModel.AddComponent<PlayerWeapon>();
            }

            if (!PlayerManagerScript.PlayerWeaponScripts.Contains(playerGrenadeHandsModel.GetComponent<PlayerWeapon>()))
            {
                PlayerManagerScript.PlayerWeaponScripts.Add(playerGrenadeHandsModel.GetComponent<PlayerWeapon>());
            }

          
            PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadePrefab = GrenadePrefab;
            PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadeAnimatorComponent = animator;
            PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().RequiredComponents.PlayerCamera = PlayerCamera.transform;
            PlayerGrenadeThrowerScript.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadeIcon = GrenadeIcon;

            PlayerGrenadeThrowerScript.transform.GetChild(0).GetComponent<AudioSource>().clip = grenadePinPullAudioClip;
            PlayerGrenadeThrowerScript.transform.GetChild(1).GetComponent<AudioSource>().clip = grenadeThrowAudioClip;
            PlayerGrenadeThrowerScript.transform.GetChild(2).GetComponent<AudioSource>().clip = wieldAudioClip;

            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().RequiredComponents.WeaponAnimatorComponent = animator;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponSounds.WieldSoundAudioSource = PlayerGrenadeThrowerScript.transform.GetChild(2).GetComponent<AudioSource>();

            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.RunSpeedParameterName = "RunSpeed";
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.WalkSpeedParameterName = "WalkSpeed";
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandRunAnimationSpeed = StandRunAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchRunAnimationSpeed = CrouchRunAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandWalkAnimationSpeed = StandWalkAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchWalkAnimationSpeed = CrouchWalkAnimationSpeed;

            Debug.Log("Player Grenade Throw Ability setup completed.");
        }
        private void AutoSetup()
        {
            // Validate required fields
            if (player == null)
            {
                Debug.LogError("Player GameObject is not assigned.");
                return;
            }
            if (weaponsRoot == null)
            {
                Debug.LogError("Weapons Root GameObject is not assigned.");
                return;
            }
            if (playerGrenadeHandsTemplate == null)
            {
                Debug.LogError("Player Grenade Hands Template prefab is not assigned.");
                return;
            }
            if (playerGrenadeHandsModel == null)
            {
                Debug.LogError("Player Grenade Hands Model GameObject is not assigned.");
                return;
            }
            if (animator == null)
            {
                Debug.LogError("Animator component is not assigned.");
                return;
            }
            if (runtimeAnimatorController == null)
            {
                Debug.LogError("Runtime Animator Controller is not assigned.");
                return;
            }

            // Log progress
            Debug.Log("Starting setup of grenade throw ability.");

            // Spawn and setup Player Grenade Hands Template
            spawnedTemplate = Instantiate(playerGrenadeHandsTemplate, weaponsRoot.transform);
            if (spawnedTemplate == null)
            {
                Debug.LogError("Failed to instantiate Player Grenade Hands Template.");
                return;
            }

            if (PrefabUtility.IsPartOfPrefabInstance(spawnedTemplate))
            {
                PrefabUtility.UnpackPrefabInstance(spawnedTemplate, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }


            spawnedTemplate.GetComponent<ProceduralIdleBobbing>().PlayerManagerScript = PlayerManagerScript;
            PlayerManagerScript.BobbingScripts.Add(spawnedTemplate);

            spawnedTemplate.name = playerGrenadeHandsModel.name + " Weapon";

            if (spawnedTemplate.transform.childCount > 0)
            {
                Transform firstChild = spawnedTemplate.transform.GetChild(0);
                if (firstChild == null)
                {
                    Debug.LogError("Failed to find the first child of the spawned template.");
                    return;
                }

                playerGrenadeHandsModel.transform.SetParent(firstChild);
                firstChild.name = playerGrenadeHandsModel.name + " Pivot";
            }

            Object.DestroyImmediate(spawnedTemplate.transform.GetChild(0).transform.GetChild(0).gameObject);
            playerGrenadeHandsModel.name = playerGrenadeHandsModel.name;

            // Add components to Player Grenade Hands Model
            playerGrenadeHandsModel.AddComponent<GameobjectReactivationManager>();
            playerGrenadeHandsModel.AddComponent<GameobjectActivationManager>();
            playerGrenadeHandsModel.AddComponent<PlayerWeapon>();

            // Create Grenade Spawn Point
            GameObject grenadeSpawnPoint = new GameObject("Grenade Spawn Point");
            grenadeSpawnPoint.transform.SetParent(playerGrenadeHandsModel.transform);

            // Setup Animator
            animator.runtimeAnimatorController = runtimeAnimatorController;

            // Get AlternatePlayerController and check AlternateAnimations


            if (animator == null)
            {
                Debug.LogError("Animator is null before assigning to AlternateAnimations.AlternateAnimator.");
                return;
            }


            Debug.Log("Assigned Animator to AlternatePlayerController's AlternateAnimations.");

            // Spawn Player Grenade Thrower prefab
            string grenadeThrowerPrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Grenades & Melee/Player Grenade Thrower.prefab";
            GameObject grenadeThrowerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(grenadeThrowerPrefabPath);
            if (grenadeThrowerPrefab == null)
            {
                Debug.LogError("Player Grenade Thrower prefab not found at: " + grenadeThrowerPrefabPath);
                return;
            }

            spawnedThrower = Instantiate(grenadeThrowerPrefab, player.transform);


            if (spawnedThrower == null)
            {
                Debug.LogError("Failed to instantiate Player Grenade Thrower.");
                return;
            }

            if (PrefabUtility.IsPartOfPrefabInstance(spawnedThrower))
            {
                PrefabUtility.UnpackPrefabInstance(spawnedThrower, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            spawnedThrower.name = playerGrenadeHandsModel.name + " Grenade Thrower Script";

            PlayerManagerScript.PlayerWeaponScripts.Add(playerGrenadeHandsModel.GetComponent<PlayerWeapon>());


            PlayerGrenadeThrowerScript = spawnedThrower.GetComponent<PlayerGrenadeThrower>();
            spawnedThrower.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadePrefab = GrenadePrefab;
            spawnedThrower.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadeAnimatorComponent = animator;
            spawnedThrower.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadeSpawnPoint = grenadeSpawnPoint.transform;
            spawnedThrower.GetComponent<PlayerGrenadeThrower>().RequiredComponents.PlayerCamera = PlayerCamera.transform;
            spawnedThrower.GetComponent<PlayerGrenadeThrower>().RequiredComponents.GrenadeIcon = GrenadeIcon;

            spawnedThrower.transform.GetChild(0).GetComponent<AudioSource>().clip = grenadePinPullAudioClip;
            spawnedThrower.transform.GetChild(1).GetComponent<AudioSource>().clip = grenadeThrowAudioClip;
            spawnedThrower.transform.GetChild(2).GetComponent<AudioSource>().clip = wieldAudioClip;

            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().RequiredComponents.WeaponAnimatorComponent = animator;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponSounds.WieldSoundAudioSource = spawnedThrower.transform.GetChild(2).GetComponent<AudioSource>();

            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.RunSpeedParameterName = "RunSpeed";
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.WalkSpeedParameterName = "WalkSpeed";
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandRunAnimationSpeed = StandRunAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchRunAnimationSpeed = CrouchRunAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandWalkAnimationSpeed = StandWalkAnimationSpeed;
            playerGrenadeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchWalkAnimationSpeed = CrouchWalkAnimationSpeed;

            Debug.Log("Player Grenade Throw Ability setup completed.");
        }
    }
}