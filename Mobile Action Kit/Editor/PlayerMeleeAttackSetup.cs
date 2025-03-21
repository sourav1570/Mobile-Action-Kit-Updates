using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class PlayerMeleeAttackSetup : EditorWindow
    {
        public bool IsAlreadySetup = false;

        [System.Serializable]
        public enum AttackType
        {
            InstantAttack,
            Selection
        }

        public AttackType PlayerMeleeType;

        private GameObject player;
        private GameObject weaponsRoot;

        private Camera PlayerCamera;
        private GameObject MeleeIcon;

        private GameObject PlayerMeleeHandsTemplate;

        private GameObject playerMeleeHandsModel;
        private PlayerManager PlayerManagerScript;
        private Animator animator;
        private RuntimeAnimatorController runtimeAnimatorController;

        private float StandRunAnimationSpeed = 1f;
        private float CrouchRunAnimationSpeed = 0.5f;
        private float StandWalkAnimationSpeed = 1f;
        private float CrouchWalkAnimationSpeed = 0.5f;

        GameObject spawnedTemplate;
        GameObject MeleeAttack;

        private MeleeAttack PlayerMeleeAttackScript;
        private GameObject PlayerMeleeGameobject;

        [MenuItem("Tools/Mobile Action Kit/Player/Melee/Player Melee Setup", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<PlayerMeleeAttackSetup>("Player Melee Setup");
        }
        private void OnGUI()
        {
            // Title
            GUILayout.Label("Player Melee Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Is Already Setup Toggle
            IsAlreadySetup = EditorGUILayout.Toggle("IS ALREADY SETUP", IsAlreadySetup);

            // Grenade Throw Type Enum Popup
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Player Melee Type", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f)); // 40% for the label        
            PlayerMeleeType = (AttackType)EditorGUILayout.EnumPopup(PlayerMeleeType);
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
                CreateDynamicField("Melee Icon", ref MeleeIcon, typeof(GameObject));
                CreateDynamicField("Player Melee Hands Template", ref PlayerMeleeHandsTemplate, typeof(GameObject));
                CreateDynamicField("Animator", ref animator, typeof(Animator));
                CreateDynamicField("Animator Controller", ref runtimeAnimatorController, typeof(RuntimeAnimatorController));
                CreateDynamicField("Player Melee Hands Model", ref playerMeleeHandsModel, typeof(GameObject));
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
            }
            else // If already setup
            {
                GUILayout.Label("Drag and Drop GameObjects", EditorStyles.boldLabel);

                GUILayout.Space(10);
                // Setup fields when already setup
                CreateDynamicField("Player Melee Hands Model", ref playerMeleeHandsModel, typeof(GameObject));
                CreateDynamicField("Player Melee Attack Script", ref PlayerMeleeAttackScript, typeof(MeleeAttack));
                CreateDynamicField("Player Melee Gameobject", ref PlayerMeleeGameobject, typeof(GameObject));

                GUILayout.Space(10);

                CreateDynamicField("Player Camera", ref PlayerCamera, typeof(Camera));
                CreateDynamicField("Melee Icon", ref MeleeIcon, typeof(GameObject));
                CreateDynamicField("Animator", ref animator, typeof(Animator));
                CreateDynamicField("Player Manager Script", ref PlayerManagerScript, typeof(PlayerManager));

                // Animation Speeds Section
                GUILayout.Label("Animation Speeds", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");

                StandRunAnimationSpeed = CreateDynamicFloatField("Stand Run Speed", StandRunAnimationSpeed);
                CrouchRunAnimationSpeed = CreateDynamicFloatField("Crouch Run Speed", CrouchRunAnimationSpeed);
                StandWalkAnimationSpeed = CreateDynamicFloatField("Stand Walk Speed", StandWalkAnimationSpeed);
                CrouchWalkAnimationSpeed = CreateDynamicFloatField("Crouch Walk Speed", CrouchWalkAnimationSpeed);

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);

            }

            // Buttons Section
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Centers the buttons
            if (GUILayout.Button("Setup Player Melee Ability", GUILayout.Width(200), GUILayout.Height(30)))
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
            EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Player/Melee/Edit Melee Animations");
        }
        private void SetupGrenadeThrowAbility()
        {
            if (IsAlreadySetup == false)
            {
                if (PlayerMeleeType == AttackType.Selection)
                {
                    AutoSetup();
                    if (PlayerMeleeAttackScript != null)
                    {
                        PlayerMeleeAttackScript.MeleeType = "Selection";
                    }
                }
                else
                {
                    AutoSetup();
                    if (PlayerMeleeAttackScript != null)
                    {
                        PlayerMeleeAttackScript.MeleeType = "Instant Attack";
                    }
                    if (PlayerManagerScript.BobbingScripts.Contains(spawnedTemplate))
                    {
                        PlayerManagerScript.BobbingScripts.Remove(spawnedTemplate);
                    }
                    if (PlayerManagerScript.PlayerWeaponScripts.Contains(playerMeleeHandsModel.GetComponent<PlayerWeapon>()))
                    {
                        PlayerManagerScript.PlayerWeaponScripts.Remove(playerMeleeHandsModel.GetComponent<PlayerWeapon>());
                    }

                    PlayerWeapon weapon = playerMeleeHandsModel.GetComponent<PlayerWeapon>();
                    if (weapon != null)
                    {
                        Object.DestroyImmediate(weapon);
                    }
                }
            }
            else
            {
                if (PlayerMeleeType == AttackType.Selection)
                {
                    ModifyExistingSetup();
                    if (PlayerMeleeAttackScript != null)
                    {
                        PlayerMeleeAttackScript.MeleeType = "Selection";
                    }
                }
                else
                {
                    ModifyExistingSetup();
                    if (PlayerMeleeAttackScript != null)
                    {
                        PlayerMeleeAttackScript.MeleeType = "Instant Attack";
                    }
                    if (PlayerManagerScript.BobbingScripts.Contains(PlayerMeleeGameobject))
                    {
                        PlayerManagerScript.BobbingScripts.Remove(PlayerMeleeGameobject);
                    }
                    if (PlayerManagerScript.PlayerWeaponScripts.Contains(playerMeleeHandsModel.GetComponent<PlayerWeapon>()))
                    {
                        PlayerManagerScript.PlayerWeaponScripts.Remove(playerMeleeHandsModel.GetComponent<PlayerWeapon>());
                    }

                    PlayerWeapon weapon = playerMeleeHandsModel.GetComponent<PlayerWeapon>();
                    if (weapon != null)
                    {
                        Object.DestroyImmediate(weapon);
                    }
                }
            }
        }
        private void ModifyExistingSetup()
        {
            PlayerMeleeGameobject.GetComponent<ProceduralIdleBobbing>().PlayerManagerScript = PlayerManagerScript;

            if (!PlayerManagerScript.BobbingScripts.Contains(PlayerMeleeGameobject))
            {
                PlayerManagerScript.BobbingScripts.Add(PlayerMeleeGameobject);
            }

            // Add components to Player Grenade Hands Model
            if (playerMeleeHandsModel.GetComponent<GameobjectActivationManager>() == null)
            {
                playerMeleeHandsModel.AddComponent<GameobjectActivationManager>();
            }
            if (playerMeleeHandsModel.GetComponent<PlayerWeapon>() == null)
            {
                playerMeleeHandsModel.AddComponent<PlayerWeapon>();
            }

            if (!PlayerManagerScript.PlayerWeaponScripts.Contains(playerMeleeHandsModel.GetComponent<PlayerWeapon>()))
            {
                PlayerManagerScript.PlayerWeaponScripts.Add(playerMeleeHandsModel.GetComponent<PlayerWeapon>());
            }

            PlayerMeleeAttackScript.GetComponent<MeleeAttack>().RequiredComponents.MeleeAnimatorComponent = animator;
 
            PlayerMeleeAttackScript.GetComponent<MeleeAttack>().RequiredComponents.MeleeIcon = MeleeIcon;

            playerMeleeHandsModel.GetComponent<PlayerWeapon>().RequiredComponents.WeaponAnimatorComponent = animator;

            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.RunSpeedParameterName = "RunSpeed";
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.WalkSpeedParameterName = "WalkSpeed";
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandRunAnimationSpeed = StandRunAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchRunAnimationSpeed = CrouchRunAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandWalkAnimationSpeed = StandWalkAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchWalkAnimationSpeed = CrouchWalkAnimationSpeed;

            Debug.Log("Player Melee Ability setup completed.");
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
            if (PlayerMeleeHandsTemplate == null)
            {
                Debug.LogError("Player Melee Hands Template prefab is not assigned.");
                return;
            }
            if (playerMeleeHandsModel == null)
            {
                Debug.LogError("Player Melee Hands Model GameObject is not assigned.");
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
            Debug.Log("Starting setup of knife attack ability.");

            // Spawn and setup Player Grenade Hands Template
            spawnedTemplate = Instantiate(PlayerMeleeHandsTemplate, weaponsRoot.transform);
            if (spawnedTemplate == null)
            {
                Debug.LogError("Failed to instantiate Player Melee Hands Template.");
                return;
            }

            if (PrefabUtility.IsPartOfPrefabInstance(spawnedTemplate))
            {
                PrefabUtility.UnpackPrefabInstance(spawnedTemplate, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }


            spawnedTemplate.GetComponent<ProceduralIdleBobbing>().PlayerManagerScript = PlayerManagerScript;
            PlayerManagerScript.BobbingScripts.Add(spawnedTemplate);

            spawnedTemplate.name = playerMeleeHandsModel.name + " Weapon";

            if (spawnedTemplate.transform.childCount > 0)
            {
                Transform firstChild = spawnedTemplate.transform.GetChild(0);
                if (firstChild == null)
                {
                    Debug.LogError("Failed to find the first child of the spawned template.");
                    return;
                }

                playerMeleeHandsModel.transform.SetParent(firstChild);
                firstChild.name = playerMeleeHandsModel.name + " Pivot";
            }

            Object.DestroyImmediate(spawnedTemplate.transform.GetChild(0).transform.GetChild(0).gameObject);
            playerMeleeHandsModel.name = playerMeleeHandsModel.name;

            // Add components to Player Grenade Hands Model
            playerMeleeHandsModel.AddComponent<GameobjectActivationManager>();
            playerMeleeHandsModel.AddComponent<PlayerWeapon>();

            // Setup Animator
            animator.runtimeAnimatorController = runtimeAnimatorController;

            // Spawn Player Grenade Thrower prefab
            string playerMeleeAttackPrefabPath = "Assets/Mobile Action Kit/Editor/Editor Prefabs/Grenades & Melee/Player Melee Attack.prefab";
            GameObject playerMeleeAttackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerMeleeAttackPrefabPath);
            if (playerMeleeAttackPrefab == null)
            {
                Debug.LogError("Player Melee Attack prefab not found at: " + playerMeleeAttackPrefabPath);
                return;
            }

            MeleeAttack = Instantiate(playerMeleeAttackPrefab, player.transform);


            if (MeleeAttack == null)
            {
                Debug.LogError("Failed to instantiate Player Melee.");
                return;
            }

            if (PrefabUtility.IsPartOfPrefabInstance(MeleeAttack))
            {
                PrefabUtility.UnpackPrefabInstance(MeleeAttack, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            MeleeAttack.name = playerMeleeHandsModel.name + " Melee Attack Script";

            PlayerManagerScript.PlayerWeaponScripts.Add(playerMeleeHandsModel.GetComponent<PlayerWeapon>());

            MeleeAttack.GetComponent<MeleeAttack>().RequiredComponents.MeleeAnimatorComponent = animator;
            MeleeAttack.GetComponent<MeleeAttack>().RequiredComponents.MeleeIcon = MeleeIcon;

            PlayerMeleeAttackScript = MeleeAttack.GetComponent<MeleeAttack>();

            playerMeleeHandsModel.GetComponent<PlayerWeapon>().RequiredComponents.WeaponAnimatorComponent = animator;

            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.RunSpeedParameterName = "RunSpeed";
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.WalkSpeedParameterName = "WalkSpeed";
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandRunAnimationSpeed = StandRunAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchRunAnimationSpeed = CrouchRunAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.StandWalkAnimationSpeed = StandWalkAnimationSpeed;
            playerMeleeHandsModel.GetComponent<PlayerWeapon>().WeaponAnimationClipsSpeeds.CrouchWalkAnimationSpeed = CrouchWalkAnimationSpeed;

            Debug.Log("Player Melee Attack Ability setup completed.");
        }
    }
}