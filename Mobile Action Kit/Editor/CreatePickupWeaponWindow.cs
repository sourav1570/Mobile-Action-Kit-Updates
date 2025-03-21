using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static MobileActionKit.PlayerWeaponsManager;

namespace MobileActionKit
{
    public class CreatePickupWeaponWindow : EditorWindow
    {
        private GameObject pickupWeaponModel;
        private GameObject FinalPickUpWeaponCreated;
        private PlayerWeaponsManager playerWeaponManager;
        private string selectedPath = ""; // To store the path for prefab location

        [System.Serializable]
        public class AttachmentData
        {
            public string KeyName;
            public GameObject attachment;
        }

        private List<AttachmentData> attachments = new List<AttachmentData>();

        private bool ReplaceWeaponBySlot = true;
        private string SlotName = "Primary_Slot";
        private bool ShowWeaponPickupConfirmationUI = false;
        private string keyName;
        private int ammo;
        private Sprite weaponSprite;
        private bool activateAnyOneAttachmentRandomly = false;

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Pickup Weapon/Create Pickup Weapon", priority = 8)]
        public static void ShowWindow()
        {
            GetWindow<CreatePickupWeaponWindow>("Create Pickup Weapon");
        }
        private Vector2 scrollPosition;

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            GUILayout.Label("Pickup Weapon Settings", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            pickupWeaponModel = (GameObject)EditorGUILayout.ObjectField(
                "Pickup Weapon Model",
                pickupWeaponModel,
                typeof(GameObject),
                true,
                GUILayout.ExpandWidth(true)
            );
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.Label("Attachments", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            for (int i = 0; i < attachments.Count; i++)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
                GUILayout.Label($"Attachment {i + 1}", EditorStyles.boldLabel);

                attachments[i].KeyName = EditorGUILayout.TextField("Key Name", attachments[i].KeyName, GUILayout.ExpandWidth(true));
                attachments[i].attachment = (GameObject)EditorGUILayout.ObjectField(
                    "Attachment",
                    attachments[i].attachment,
                    typeof(GameObject),
                    true,
                    GUILayout.ExpandWidth(true)
                );
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Attachment", GUILayout.Height(30), GUILayout.Width(200)))
            {
                attachments.Add(new AttachmentData());
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Remove Last Added Attachment", GUILayout.Height(30), GUILayout.Width(200)))
            {
                if (attachments.Count > 0)
                    attachments.RemoveAt(attachments.Count - 1);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.Label("Additional Settings", EditorStyles.boldLabel);

            //EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            //ReplaceWeaponBySlot = EditorGUILayout.Toggle("Replace Weapon By Slot If Exist", ReplaceWeaponBySlot);
            //SlotName = EditorGUILayout.TextField("Slot Name", SlotName, GUILayout.ExpandWidth(true));

            // Properly aligning the toggle field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Replace Weapon By Slot If Exist", GUILayout.Width(200)); // Adjust width as needed
            ReplaceWeaponBySlot = EditorGUILayout.Toggle(ReplaceWeaponBySlot);
            EditorGUILayout.EndHorizontal();

            SlotName = EditorGUILayout.TextField("Slot Name", SlotName, GUILayout.ExpandWidth(true));

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Show Weapon Pickup Confirmation UI", GUILayout.Width(230));
            ShowWeaponPickupConfirmationUI = EditorGUILayout.Toggle(ShowWeaponPickupConfirmationUI);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            keyName = EditorGUILayout.TextField("Key Name", keyName, GUILayout.ExpandWidth(true));
            ammo = EditorGUILayout.IntField("Ammo", ammo);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activate Random Attachment", GUILayout.Width(200));
            activateAnyOneAttachmentRandomly = EditorGUILayout.Toggle(activateAnyOneAttachmentRandomly);
            EditorGUILayout.EndHorizontal();

            weaponSprite = (Sprite)EditorGUILayout.ObjectField("Weapon Sprite", weaponSprite, typeof(Sprite), false, GUILayout.ExpandWidth(true));
           // EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            if (GUILayout.Button("Create Pickup Weapon", GUILayout.Height(40), GUILayout.ExpandWidth(true)))
            {
                CreatePickupWeapon();
            }

            GUILayout.Space(10);
            GUILayout.Label("Prefab and Manager Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Final Pickup Weapon Created", GUILayout.Width(200));
            FinalPickUpWeaponCreated = (GameObject)EditorGUILayout.ObjectField(FinalPickUpWeaponCreated, typeof(GameObject), true, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            playerWeaponManager = (PlayerWeaponsManager)EditorGUILayout.ObjectField("Player Weapon Manager", playerWeaponManager, typeof(PlayerWeaponsManager), true, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Choose Prefab Location", GUILayout.ExpandWidth(true)))
            {
                string path = EditorUtility.SaveFolderPanel("Save Prefab", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    selectedPath = path;
                }
            }

            EditorGUILayout.LabelField("Selected Prefab Location: " + selectedPath, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Create Prefab & Update Player Weapons Manager Script", GUILayout.ExpandWidth(true)))
            {
                CreatePrefab();
            }

            EditorGUILayout.EndScrollView();
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Pickup Weapon Settings", EditorStyles.boldLabel);

        //    GUILayout.Space(10);

        //    // Pickup Weapon Model Field
        //    EditorGUILayout.BeginVertical("box");
        //    pickupWeaponModel = (GameObject)EditorGUILayout.ObjectField(
        //        "Pickup Weapon Model",
        //        pickupWeaponModel,
        //        typeof(GameObject),
        //        true
        //    );
        //    EditorGUILayout.EndVertical();

        //    GUILayout.Space(10);

        //    // Attachments Section
        //    GUILayout.Label("Attachments", EditorStyles.boldLabel);

        //    EditorGUILayout.BeginVertical("box");
        //    for (int i = 0; i < attachments.Count; i++)
        //    {
        //        EditorGUILayout.BeginVertical("box");
        //        EditorGUILayout.BeginHorizontal();
        //        GUILayout.Label($"Attachment {i + 1}", EditorStyles.boldLabel);
        //        EditorGUILayout.EndHorizontal();

        //        // Render the attachment fields
        //        attachments[i].KeyName = EditorGUILayout.TextField("Key Name", attachments[i].KeyName);
        //        attachments[i].attachment = (GameObject)EditorGUILayout.ObjectField(
        //            "Attachment",
        //            attachments[i].attachment,
        //            typeof(GameObject),
        //            true
        //        );
        //        EditorGUILayout.EndVertical();
        //    }

        //    EditorGUILayout.EndVertical();

        //    GUILayout.Space(10);

        //    // Buttons for Adding and Removing Attachments
        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.FlexibleSpace();

        //    if (GUILayout.Button("Add Attachment", GUILayout.Height(30), GUILayout.Width(200)))
        //    {
        //        attachments.Add(new AttachmentData());
        //    }

        //    GUILayout.Space(10);

        //    if (GUILayout.Button("Remove Last Added Attachment", GUILayout.Height(30), GUILayout.Width(200)))
        //    {
        //        attachments.RemoveAt(attachments.Count - 1);
        //    }

        //    GUILayout.FlexibleSpace();
        //    EditorGUILayout.EndHorizontal();

        //    GUILayout.Space(20);

        //    // Additional Fields for Pickup Weapon
        //    GUILayout.Label("Additional Settings", EditorStyles.boldLabel);

        //    EditorGUILayout.BeginVertical("box");
        //    ReplaceWeaponBySlot = EditorGUILayout.Toggle("Replace Weapon By Slot", ReplaceWeaponBySlot);
        //    SlotName = EditorGUILayout.TextField("Slot Name", SlotName);
        //    ShowWeaponPickupConfirmationUI = EditorGUILayout.Toggle("Show Weapon Pickup Confirmation UI", ShowWeaponPickupConfirmationUI);
        //    keyName = EditorGUILayout.TextField("Key Name", keyName);
        //    ammo = EditorGUILayout.IntField("Ammo", ammo);

        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Label("Activate Random Attachment", GUILayout.Width(200));
        //    activateAnyOneAttachmentRandomly = EditorGUILayout.Toggle(activateAnyOneAttachmentRandomly);
        //    EditorGUILayout.EndHorizontal();

        //    weaponSprite = (Sprite)EditorGUILayout.ObjectField("Weapon Sprite", weaponSprite, typeof(Sprite), false);
        //    EditorGUILayout.EndVertical(); 

        //    GUILayout.Space(10);

        //    // Button to Create Pickup Weapon
        //    if (GUILayout.Button("Create Pickup Weapon", GUILayout.Height(40)))
        //    {
        //        CreatePickupWeapon();
        //    }
        //    GUILayout.Space(10);


        //    // New Fields for Pickup Weapon GameObject and PlayerWeaponManager
        //    GUILayout.Label("Prefab and Manager Settings", EditorStyles.boldLabel);

        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Label("Final Pickup Weapon Created", GUILayout.Width(200));
        //    FinalPickUpWeaponCreated = (GameObject)EditorGUILayout.ObjectField(FinalPickUpWeaponCreated, typeof(GameObject), true);
        //    EditorGUILayout.EndHorizontal();

        //    // Drag and Drop Player Weapon Manager
        //    playerWeaponManager = (PlayerWeaponsManager)EditorGUILayout.ObjectField("Player Weapon Manager", playerWeaponManager, typeof(PlayerWeaponsManager), true);

        //    // Create Prefab Location Path button
        //    if (GUILayout.Button("Choose Prefab Location"))
        //    {
        //        string path = EditorUtility.SaveFolderPanel("Save Prefab", "Assets", "");
        //        if (!string.IsNullOrEmpty(path))
        //        {
        //            selectedPath = path;
        //        }
        //    }

        //    // Display selected prefab location path
        //    EditorGUILayout.LabelField("Selected Prefab Location: " + selectedPath);

        //    // Create Prefab button
        //    if (GUILayout.Button("Create Prefab & Update Player Weapons Manager Script"))
        //    {
        //        CreatePrefab();
        //    }
        //}

        private void CreatePickupWeapon()
        {
            if (pickupWeaponModel == null)
            {
                Debug.LogError("Pickup Weapon Model is not assigned.");
                return;
            }

            // Create the empty gameObject
            string pickupWeaponName = "Pickup Weapon " + pickupWeaponModel.name;
            GameObject pickupWeaponGO = new GameObject(pickupWeaponName);

            pickupWeaponGO.layer = LayerMask.NameToLayer("Ignore Raycast");

            Vector3 position = pickupWeaponModel.transform.localPosition;
            Vector3 eulerangles = pickupWeaponModel.transform.localEulerAngles;

            pickupWeaponGO.transform.localPosition = position;
            pickupWeaponGO.transform.localEulerAngles = Vector3.zero;

            // Add Box Collider with Is Trigger checked
            BoxCollider boxCollider = pickupWeaponGO.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;

            // Add PickupWeapon script and set its fields
            PickupWeapon pickupWeaponScript = pickupWeaponGO.GetComponent<PickupWeapon>();
            if (pickupWeaponScript == null)
            {
                pickupWeaponScript = pickupWeaponGO.AddComponent<PickupWeapon>();
            }

            pickupWeaponScript.ReplaceWeaponBySlotIfExist = ReplaceWeaponBySlot;
            pickupWeaponScript.SlotName = SlotName;
            pickupWeaponScript.ShowWeaponPickupConfirmationUI = ShowWeaponPickupConfirmationUI;
            pickupWeaponScript.KeyName = keyName;
            pickupWeaponScript.InitialAmmo = ammo;
            pickupWeaponScript.WeaponSprite = weaponSprite;
            pickupWeaponScript.ActivateRandomAttachment = activateAnyOneAttachmentRandomly;

            // Set Pickup Weapon Model as child
            pickupWeaponModel.transform.SetParent(pickupWeaponGO.transform);
            pickupWeaponModel.transform.localPosition = Vector3.zero;
            pickupWeaponModel.transform.localEulerAngles = eulerangles;

            // Process attachments
            List<AttachmentKey> attachmentKeys = new List<AttachmentKey>();
            foreach (var attachmentData in attachments)
            {
                if (attachmentData.attachment != null)
                {
                    attachmentData.attachment.transform.SetParent(pickupWeaponModel.transform);

                    // Add AttachmentKey script if not already added
                    AttachmentKey attachmentKey = attachmentData.attachment.GetComponent<AttachmentKey>();
                    if (attachmentKey == null)
                    {
                        attachmentKey = attachmentData.attachment.AddComponent<AttachmentKey>();
                    }

                    // Set KeyName in AttachmentKey
                    attachmentKey.KeyName = attachmentData.KeyName;
                    attachmentKeys.Add(attachmentKey);
                }
            }

            // Assign AttachmentsToActivate in PickupWeapon
            pickupWeaponScript.AttachmentsToActivate = attachmentKeys.ToArray();

            Debug.Log("Pickup Weapon created successfully!");
        }

        //private void CreatePrefab()
        //{
        //    // Check if all necessary fields are set
        //    if (FinalPickUpWeaponCreated == null || playerWeaponManager == null || string.IsNullOrEmpty(selectedPath))
        //    {
        //        Debug.LogError("Please make sure all fields are set correctly.");
        //        return;
        //    }

        //    // Create prefab from Pickup Weapon GameObject
        //    string prefabPath = "Assets" + selectedPath.Substring(Application.dataPath.Length) + "/" + FinalPickUpWeaponCreated.name + ".prefab";
        //    PrefabUtility.SaveAsPrefabAsset(FinalPickUpWeaponCreated, prefabPath);

        //    // Add the newly created prefab to the PickupWeapons list with a unique ID
        //    WeaponsPickup newPickupWeapon = new WeaponsPickup
        //    {
        //        WeaponName = keyName, // Unique ID for the weapon
        //        WeaponPickupPrefab = FinalPickUpWeaponCreated
        //    };

        //    playerWeaponManager.PickupWeapons.Add(newPickupWeapon);

        //    Debug.Log("Prefab created and added to PlayerWeaponManager successfully.");
        //}
        private void CreatePrefab()
        {
            // Check if all necessary fields are set
            if (FinalPickUpWeaponCreated == null || playerWeaponManager == null || string.IsNullOrEmpty(selectedPath))
            {
                Debug.LogError("Please make sure all fields are set correctly.");
                return;
            }

            // Create prefab from Pickup Weapon GameObject
            string prefabPath = "Assets" + selectedPath.Substring(Application.dataPath.Length) + "/" + FinalPickUpWeaponCreated.name + ".prefab";

            // Save the gameObject as a prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(FinalPickUpWeaponCreated, prefabPath);

            // Add the newly created prefab to the PickupWeapons list with a unique ID
            WeaponPickupData newPickupWeapon = new WeaponPickupData
            {
                WeaponName = keyName, // Unique ID for the weapon
                WeaponPickupPrefab = prefab // Use the prefab instead of the original GameObject
            };

            // Optionally, add the new PickupWeapon to the PlayerWeaponManager list
            playerWeaponManager.AvailableWeaponPickups.Add(newPickupWeapon);

            // Ensure the prefab is visible in the hierarchy (instantiate it from the prefab)
            GameObject instantiatedPrefab = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instantiatedPrefab != null)
            {
                instantiatedPrefab.name = FinalPickUpWeaponCreated.name; // Set the name to match the original GameObject
                Debug.Log("Prefab created and instantiated in hierarchy successfully.");
            }

            Object.DestroyImmediate(FinalPickUpWeaponCreated);

            Debug.Log("Prefab created and added to PlayerWeaponManager successfully.");
        }

    }
}




//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//namespace MobileActionKit
//{
//    public class CreatePickupWeaponWindow : EditorWindow
//    {
//        private GameObject pickupWeaponModel;

//        [System.Serializable]
//        public class AttachmentData
//        {
//            public string KeyName;
//            public GameObject attachment;
//        }

//        private List<AttachmentData> attachments = new List<AttachmentData>();

//        private bool replaceThisWeaponByCategory = true;
//        private string category = "Primary_Slot";
//        private bool canPickThisWeaponDirectly = false;
//        private string keyName;
//        private int ammo;
//        private Sprite weaponSprite;
//        private bool activateAnyOneAttachmentRandomly = false;

//        [MenuItem("Tools/MobileActionKit/Player/Weapon/Create Pickup Weapon", priority = 7)]
//        public static void ShowWindow()
//        {
//            GetWindow<CreatePickupWeaponWindow>("Create Pickup Weapon");
//        }

//        private void OnGUI()
//        {
//            GUILayout.Label("Pickup Weapon Settings", EditorStyles.boldLabel);

//            GUILayout.Space(10);

//            // Pickup Weapon Model Field
//            EditorGUILayout.BeginVertical("box");
//            pickupWeaponModel = (GameObject)EditorGUILayout.ObjectField(
//                "Pickup Weapon Model",
//                pickupWeaponModel,
//                typeof(GameObject),
//                true
//            );
//            EditorGUILayout.EndVertical();

//            GUILayout.Space(10);

//            // Attachments Section
//            GUILayout.Label("Attachments", EditorStyles.boldLabel);

//            EditorGUILayout.BeginVertical("box");
//            for (int i = 0; i < attachments.Count; i++)
//            {
//                EditorGUILayout.BeginVertical("box");
//                EditorGUILayout.BeginHorizontal();
//                GUILayout.Label($"Attachment {i + 1}", EditorStyles.boldLabel);

//                // Delay removal until after the loop to maintain layout consistency
//                // bool shouldRemove = GUILayout.Button("Remove", GUILayout.Width(80), GUILayout.Height(20));
//                EditorGUILayout.EndHorizontal();

//                // Render the attachment fields
//                attachments[i].KeyName = EditorGUILayout.TextField("Key Name", attachments[i].KeyName);
//                attachments[i].attachment = (GameObject)EditorGUILayout.ObjectField(
//                    "Attachment",
//                    attachments[i].attachment,
//                    typeof(GameObject),
//                    true
//                );
//                EditorGUILayout.EndVertical();
//            }

//            EditorGUILayout.EndVertical();

//            GUILayout.Space(10);

//            // Buttons for Adding and Removing Attachments
//            EditorGUILayout.BeginHorizontal();
//            GUILayout.FlexibleSpace(); // Adds flexible space to push buttons to the center

//            if (GUILayout.Button("Add Attachment", GUILayout.Height(30), GUILayout.Width(200))) // Set the width as you like
//            {
//                attachments.Add(new AttachmentData());
//            }

//            GUILayout.Space(10); // Space between the buttons if needed

//            if (GUILayout.Button("Remove Last Added Attachment", GUILayout.Height(30), GUILayout.Width(200))) // Set the width as you like
//            {
//                attachments.RemoveAt(attachments.Count - 1);
//            }

//            GUILayout.FlexibleSpace(); // Adds flexible space to push buttons to the center
//            EditorGUILayout.EndHorizontal();


//            GUILayout.Space(20);

//            // Additional Fields for Pickup Weapon
//            GUILayout.Label("Additional Settings", EditorStyles.boldLabel);

//            EditorGUILayout.BeginVertical("box");
//            replaceThisWeaponByCategory = EditorGUILayout.Toggle("Replace By Category", replaceThisWeaponByCategory);
//            category = EditorGUILayout.TextField("Category", category);
//            canPickThisWeaponDirectly = EditorGUILayout.Toggle("Can Pick Directly", canPickThisWeaponDirectly);
//            keyName = EditorGUILayout.TextField("Key Name", keyName);
//            ammo = EditorGUILayout.IntField("Ammo", ammo);
//            // activateAnyOneAttachmentRandomly = EditorGUILayout.Toggle("Activate Random Attachment", activateAnyOneAttachmentRandomly);

//            EditorGUILayout.BeginHorizontal(); // Create a horizontal layout for better control
//            GUILayout.Label("Activate Random Attachment", GUILayout.Width(200)); // Set a fixed width for the label
//            activateAnyOneAttachmentRandomly = EditorGUILayout.Toggle(activateAnyOneAttachmentRandomly); // Place the toggle next to the label
//            EditorGUILayout.EndHorizontal();

//            weaponSprite = (Sprite)EditorGUILayout.ObjectField("Weapon Sprite", weaponSprite, typeof(Sprite), false);
//            EditorGUILayout.EndVertical();

//            GUILayout.Space(20);

//            // Button to Create Pickup Weapon
//            if (GUILayout.Button("Create Pickup Weapon", GUILayout.Height(40)))
//            {
//                CreatePickupWeapon();
//            }
//        }

//        private void CreatePickupWeapon()
//        {
//            if (pickupWeaponModel == null)
//            {
//                Debug.LogError("Pickup Weapon Model is not assigned.");
//                return;
//            }

//            // Create the empty gameObject
//            string pickupWeaponName = "Pickup Weapon " + pickupWeaponModel.name;
//            GameObject pickupWeaponGO = new GameObject(pickupWeaponName);

//            Vector3 position = pickupWeaponModel.transform.localPosition;
//            Vector3 eulerangles = pickupWeaponModel.transform.localEulerAngles;

//            pickupWeaponGO.transform.localPosition = position;
//            pickupWeaponGO.transform.localEulerAngles = eulerangles;

//            // Add Box Collider with Is Trigger checked
//            BoxCollider boxCollider = pickupWeaponGO.AddComponent<BoxCollider>();
//            boxCollider.isTrigger = true;

//            // Add PickupWeapon script and set its fields
//            PickupWeapon pickupWeaponScript = pickupWeaponGO.GetComponent<PickupWeapon>();
//            if (pickupWeaponScript == null)
//            {
//                pickupWeaponScript = pickupWeaponGO.AddComponent<PickupWeapon>();
//            }

//            pickupWeaponScript.ReplaceThisWeaponByCategory = replaceThisWeaponByCategory;
//            pickupWeaponScript.Category = category;
//            pickupWeaponScript.CanPickThisWeaponDirectly = canPickThisWeaponDirectly;
//            pickupWeaponScript.KeyName = keyName;
//            pickupWeaponScript.Ammo = ammo;
//            pickupWeaponScript.WeaponSprite = weaponSprite;
//            pickupWeaponScript.ActivateAnyOneAttachmentRandomly = activateAnyOneAttachmentRandomly;

//            // Set Pickup Weapon Model as child
//            pickupWeaponModel.transform.SetParent(pickupWeaponGO.transform);

//            pickupWeaponModel.transform.localPosition = Vector3.zero ;
//            pickupWeaponModel.transform.localEulerAngles = Vector3.zero;

//            // Process attachments
//            List<AttachmentKey> attachmentKeys = new List<AttachmentKey>();
//            foreach (var attachmentData in attachments)
//            {
//                if (attachmentData.attachment != null)
//                {
//                    attachmentData.attachment.transform.SetParent(pickupWeaponModel.transform);

//                    // Add AttachmentKey script if not already added
//                    AttachmentKey attachmentKey = attachmentData.attachment.GetComponent<AttachmentKey>();
//                    if (attachmentKey == null)
//                    {
//                        attachmentKey = attachmentData.attachment.AddComponent<AttachmentKey>();
//                    }

//                    // Set KeyName in AttachmentKey
//                    attachmentKey.KeyName = attachmentData.KeyName;

//                    attachmentKeys.Add(attachmentKey);
//                }
//            }

//            // Assign AttachmentsToActivate in PickupWeapon
//            pickupWeaponScript.AttachmentsToActivate = attachmentKeys.ToArray();

//            Debug.Log("Pickup Weapon created successfully!");
//        }
//    }
//}
