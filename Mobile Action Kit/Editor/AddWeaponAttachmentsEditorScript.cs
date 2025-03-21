using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MobileActionKit
{
    public class AddWeaponAttachmentsWindow : EditorWindow
    {
        public PlayerWeapon PlayerWeaponScript;
        public GameObject WeaponAttachmentsRootGameObject;
        public GameObject Weapon;
        public bool CreateNewAttachmentScript = true;
        public AttachmentsActivator ExistingScript;

        [System.Serializable]
        public enum AttachmentTypes
        {
            Optics,
            Suppressor,
            Other
        }

        [System.Serializable]
        public class AttachmentConfiguration
        {
            public string UniqueNameForAttachment;
            public AttachmentTypes AttachmentType;
            public GameObject Attachment;
            public MonoBehaviour AttachmentFunctionScript;
            public string AttachmentFunction;
        }

        public List<AttachmentConfiguration> Attachments = new List<AttachmentConfiguration>();

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Add Weapon Attachments",priority = 7)]
        public static void ShowWindow()
        {
            GetWindow<AddWeaponAttachmentsWindow>("Add Weapon Attachments");
        }
        private void OnGUI()
        {
            // Set a dynamic label width based on the window size
            EditorGUIUtility.labelWidth = Mathf.Min(position.width * 0.4f, 250); // 40% of window width, capped at 250px

            // Title for the window
            GUILayout.Label("Add Weapon Attachments", EditorStyles.boldLabel);

            // Player Weapon and Root Section
            EditorGUILayout.LabelField("Weapon and Root Settings", EditorStyles.boldLabel);
            PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);
            WeaponAttachmentsRootGameObject = (GameObject)EditorGUILayout.ObjectField("Weapon Attachments Root", WeaponAttachmentsRootGameObject, typeof(GameObject), true);
            Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
            CreateNewAttachmentScript = EditorGUILayout.Toggle("Create New Attachment Script", CreateNewAttachmentScript);

            if (!CreateNewAttachmentScript)
            {
                ExistingScript = (AttachmentsActivator)EditorGUILayout.ObjectField("Existing Script", ExistingScript, typeof(AttachmentsActivator), true);
            }

            EditorGUILayout.Space();

            // Attachments Section
            GUILayout.Label("Attachments", EditorStyles.boldLabel);
            int removeIndex = -1; // Track index to remove later

            for (int i = 0; i < Attachments.Count; i++)
            {
                var attachment = Attachments[i];
                EditorGUILayout.BeginVertical("box");

                attachment.UniqueNameForAttachment = EditorGUILayout.TextField("Unique Name", attachment.UniqueNameForAttachment);
                attachment.AttachmentType = (AttachmentTypes)EditorGUILayout.EnumPopup("Attachment Type", attachment.AttachmentType);
                attachment.Attachment = (GameObject)EditorGUILayout.ObjectField("Attachment", attachment.Attachment, typeof(GameObject), true);
                attachment.AttachmentFunctionScript = (MonoBehaviour)EditorGUILayout.ObjectField("Attachment Function Script", attachment.AttachmentFunctionScript, typeof(MonoBehaviour), true);

                if (attachment.AttachmentFunctionScript != null)
                {
                    var methods = attachment.AttachmentFunctionScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
                    string[] methodNames = System.Array.FindAll(methods, m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
                                                                .Select(m => m.Name).ToArray();

                    int selectedIndex = System.Array.IndexOf(methodNames, attachment.AttachmentFunction);
                    selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

                    selectedIndex = EditorGUILayout.Popup("Attachment Function", selectedIndex, methodNames);
                    attachment.AttachmentFunction = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
                }

              

                EditorGUILayout.EndVertical();
            }

            // Remove the marked attachment after the loop
            if (removeIndex >= 0)
            {
                Attachments.RemoveAt(removeIndex);
            }

            // Add Attachment Button
            if (GUILayout.Button("Add Attachment"))
            {
                Attachments.Add(new AttachmentConfiguration());
            }

            // Remove Attachment Button
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Attachment"))
            {
                if (Attachments.Count > 0) // Ensure there are attachments to remove
                {
                    Attachments.RemoveAt(Attachments.Count - 1); // Remove the last attachment
                }
                else
                {
                    Debug.LogWarning("No attachments to remove!"); // Optional warning for clarity
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Add/Modify Components Buttons
            if (CreateNewAttachmentScript && GUILayout.Button("Add Required Components To GameObjects"))
            {
                AddRequiredComponents();
            }
            else if (!CreateNewAttachmentScript && ExistingScript != null && GUILayout.Button("Modify Existing Script"))
            {
                ModifyExistingScript();
            }

            // Tweak Weapon Positions & Rotations Button
            if (GUILayout.Button("Tweak Weapon Positions & Rotations"))
            {
                TweakWeaponPosition();
            }

            // Reset label width back to default after drawing the GUI
            EditorGUIUtility.labelWidth = 0;
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Add Weapon Attachments", EditorStyles.boldLabel);

        //    PlayerWeaponScript = (PlayerWeapon)EditorGUILayout.ObjectField("Player Weapon Script", PlayerWeaponScript, typeof(PlayerWeapon), true);
        //    WeaponAttachmentRootGameObject = (GameObject)EditorGUILayout.ObjectField("Weapon Attachment Root", WeaponAttachmentRootGameObject, typeof(GameObject), true);
        //    Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
        //    CreateNewAttachmentScript = EditorGUILayout.Toggle("Create New Attachment Script", CreateNewAttachmentScript);

        //    if (!CreateNewAttachmentScript)
        //    {
        //        ExistingScript = (AttachmentsActivator)EditorGUILayout.ObjectField("Existing Script", ExistingScript, typeof(AttachmentsActivator), true);
        //    }

        //    EditorGUILayout.Space();
        //    GUILayout.Label("Attachments", EditorStyles.boldLabel);

        //    int removeIndex = -1; // Track index to remove later

        //    for (int i = 0; i < Attachments.Count; i++)
        //    {
        //        var attachment = Attachments[i];
        //        EditorGUILayout.BeginVertical("box");

        //        attachment.UniqueNameForAttachment = EditorGUILayout.TextField("Unique Name", attachment.UniqueNameForAttachment);
        //        attachment.AttachmentType = (AttachmentTypes)EditorGUILayout.EnumPopup("Attachment Type", attachment.AttachmentType);
        //        attachment.Attachment = (GameObject)EditorGUILayout.ObjectField("Attachment", attachment.Attachment, typeof(GameObject), true);
        //        attachment.TargetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Target Script", attachment.TargetScript, typeof(MonoBehaviour), true);

        //        if (attachment.TargetScript != null)
        //        {
        //            var methods = attachment.TargetScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
        //            string[] methodNames = System.Array.FindAll(methods, m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
        //                                                .Select(m => m.Name).ToArray();

        //            int selectedIndex = System.Array.IndexOf(methodNames, attachment.SelectedFunctionName);
        //            selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

        //            selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methodNames);
        //            attachment.SelectedFunctionName = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
        //        }

        //        EditorGUILayout.BeginHorizontal();
        //        if (GUILayout.Button("Remove"))
        //        {
        //            removeIndex = i; // Mark this attachment for removal
        //        }
        //        EditorGUILayout.EndHorizontal();

        //        EditorGUILayout.EndVertical();
        //    }

        //    // Remove the marked attachment after the loop
        //    if (removeIndex >= 0)
        //    {
        //        Attachments.RemoveAt(removeIndex);
        //    }

        //    if (GUILayout.Button("Add Attachment"))
        //    {
        //        Attachments.Add(new AttachmentConfiguration());
        //    }

        //    EditorGUILayout.Space();

        //    if (CreateNewAttachmentScript && GUILayout.Button("Add Required Components"))
        //    {
        //        AddRequiredComponents();
        //    }
        //    else if (!CreateNewAttachmentScript && ExistingScript != null && GUILayout.Button("Modify Existing Script"))
        //    {
        //        ModifyExistingScript();
        //    }

        //    if (GUILayout.Button("Tweak Weapon Positions & Rotations"))
        //    {
        //        TweakWeaponPosition();
        //    }
        //}
        private void TweakWeaponPosition()
        {
            EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Player/FireArms/Weapon View");
        }
        private void AddRequiredComponents()
        {
            if (!WeaponAttachmentsRootGameObject || !Weapon || Attachments.Count == 0)
            {
                Debug.LogError("Please fill in all required fields before proceeding.");
                return;
            }

            GameObject attachmentHolder = new GameObject(Weapon.name + " " + "Attachment");
            attachmentHolder.transform.SetParent(WeaponAttachmentsRootGameObject.transform);

            AttachmentsActivator activateComponent = attachmentHolder.AddComponent<AttachmentsActivator>();

            foreach (var attachment in Attachments)
            {
                if (string.IsNullOrEmpty(attachment.UniqueNameForAttachment) || !attachment.Attachment)
                {
                    Debug.LogError("Incomplete attachment configuration. Skipping.");
                    continue;
                }

                if (activateComponent.inventoryItems.Any(item => item.keyName == attachment.UniqueNameForAttachment))
                {
                    Debug.LogError($"<color=red>Name '{attachment.UniqueNameForAttachment}' already exists. Skipping.</color>");
                    continue;
                }

                var inventoryItem = new AttachmentsActivator.InventoryItem
                {
                    keyName = attachment.UniqueNameForAttachment,
                    ObjectsToActivate = new GameObject[] { attachment.Attachment },
                    AttachmentFunctionScript = attachment.AttachmentFunctionScript,
                    AttachmentFunction = attachment.AttachmentFunction
                };

                activateComponent.inventoryItems.Add(inventoryItem);

                AddRequiredScripts(attachment);


            }

            Debug.Log("All required components added successfully.");
        }

        private void ModifyExistingScript()
        {
            if (ExistingScript == null || Attachments.Count == 0)
            {
                Debug.LogError("Please fill in all required fields before proceeding.");
                return;
            }

            foreach (var attachment in Attachments)
            {
                if (string.IsNullOrEmpty(attachment.UniqueNameForAttachment) || !attachment.Attachment)
                {
                    Debug.LogError("Incomplete attachment configuration. Skipping.");
                    continue;
                }

                if (ExistingScript.inventoryItems.Any(item => item.keyName == attachment.UniqueNameForAttachment))
                {
                    Debug.LogError($"<color=red>Name '{attachment.UniqueNameForAttachment}' already exists. Skipping.</color>");
                    continue;
                }

                var inventoryItem = new AttachmentsActivator.InventoryItem
                {
                    keyName = attachment.UniqueNameForAttachment,
                    ObjectsToActivate = new GameObject[] { attachment.Attachment },
                    AttachmentFunctionScript = attachment.AttachmentFunctionScript,
                    AttachmentFunction = attachment.AttachmentFunction
                };

                ExistingScript.inventoryItems.Add(inventoryItem);

                AddRequiredScripts(attachment);
            }

            Debug.Log("Existing script modified successfully.");
        }
        private void AddRequiredScripts(AttachmentConfiguration attachment)
        {
            if (attachment.AttachmentType == AttachmentTypes.Optics)
            {
                if (attachment.Attachment.GetComponent<WeaponOptics>() == null)
                {
                    attachment.Attachment.AddComponent<WeaponOptics>();
                    attachment.Attachment.GetComponent<WeaponOptics>().PlayerWeaponComponent = PlayerWeaponScript;
                }
                else
                {
                    attachment.Attachment.GetComponent<WeaponOptics>().PlayerWeaponComponent = PlayerWeaponScript;
                }
            }
            else if(attachment.AttachmentType == AttachmentTypes.Suppressor)
            {
                if (attachment.Attachment.GetComponent<SupressorController>() == null)
                {
                    attachment.Attachment.AddComponent<SupressorController>();
                    attachment.Attachment.GetComponent<SupressorController>().PlayerWeaponScript = PlayerWeaponScript;
                }
                else
                {
                    attachment.Attachment.GetComponent<SupressorController>().PlayerWeaponScript = PlayerWeaponScript;
                }
            }
        }
    }

}

//using UnityEngine;
//using UnityEditor;
//using MobileActionKit;
//using System.Linq;

//public class AddWeaponAttachmentsWindow : EditorWindow
//{
//    public GameObject AttachmentRootGameObject;
//    public GameObject Weapon;
//    public string UniqueNameForAttachment;
//    public GameObject Attachment;
//    public MonoBehaviour TargetScript;
//    public string SelectedFunctionName;
//    public bool CreateNewAttachmentScript = true;
//    public ActivatePurchasedInventory ExistingScript;

//    [MenuItem("Tools/MobileActionKit/Player/Weapon/Add Weapon Attachments")]
//    public static void ShowWindow()
//    {
//        GetWindow<AddWeaponAttachmentsWindow>("Add Weapon Attachments");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Add Weapon Attachments", EditorStyles.boldLabel);

//        // Fields for user input
//        AttachmentRootGameObject = (GameObject)EditorGUILayout.ObjectField("Attachment Root", AttachmentRootGameObject, typeof(GameObject), true);
//        Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
//        UniqueNameForAttachment = EditorGUILayout.TextField("Unique Name", UniqueNameForAttachment);
//        Attachment = (GameObject)EditorGUILayout.ObjectField("Attachment", Attachment, typeof(GameObject), true);

//        CreateNewAttachmentScript = EditorGUILayout.Toggle("Create New Attachment Script", CreateNewAttachmentScript);

//        if (CreateNewAttachmentScript)
//        {
//            TargetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Target Script", TargetScript, typeof(MonoBehaviour), true);

//            if (TargetScript != null)
//            {
//                // Populate method names in a dropdown (only public void methods with no parameters, declared in the target script)
//                var methods = TargetScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
//                string[] methodNames = System.Array.FindAll(methods, m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
//                                           .Select(m => m.Name).ToArray();

//                int selectedIndex = System.Array.IndexOf(methodNames, SelectedFunctionName);
//                selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

//                selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methodNames);
//                SelectedFunctionName = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("Please assign a Target Script.", MessageType.Warning);
//            }

//            if (GUILayout.Button("Add Required Components"))
//            {
//                AddRequiredComponents();
//            }
//        }
//        else
//        {
//            ExistingScript = (ActivatePurchasedInventory)EditorGUILayout.ObjectField("Existing Script", ExistingScript, typeof(ActivatePurchasedInventory), true);

//            TargetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Target Script", TargetScript, typeof(MonoBehaviour), true);

//            if (TargetScript != null)
//            {
//                // Populate method names in a dropdown (only public void methods with no parameters, declared in the target script)
//                var methods = TargetScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
//                string[] methodNames = System.Array.FindAll(methods, m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
//                                           .Select(m => m.Name).ToArray();

//                int selectedIndex = System.Array.IndexOf(methodNames, SelectedFunctionName);
//                selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

//                selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methodNames);
//                SelectedFunctionName = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("Please assign a Target Script.", MessageType.Warning);
//            }

//            if (ExistingScript != null)
//            {
//                if (GUILayout.Button("Modify Existing Script"))
//                {
//                    ModifyExistingScript();
//                }
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("Please assign an Existing Script.", MessageType.Warning);
//            }
//        }

//        EditorGUILayout.Space();
//    }

//    private void AddRequiredComponents()
//    {
//        if (!AttachmentRootGameObject || !Weapon || string.IsNullOrEmpty(UniqueNameForAttachment) || !Attachment || !TargetScript || string.IsNullOrEmpty(SelectedFunctionName))
//        {
//            Debug.LogError("Please fill in all required fields before proceeding.");
//            return;
//        }

//        // Create the empty GameObject as a child of AttachmentRootGameObject
//        GameObject attachmentHolder = new GameObject(Weapon.name + "Attachment");
//        attachmentHolder.transform.SetParent(AttachmentRootGameObject.transform);

//        // Add ActivatePurchasedInventory component
//        ActivatePurchasedInventory activateComponent = attachmentHolder.AddComponent<ActivatePurchasedInventory>();

//        // Create a new InventoryItem
//        var inventoryItem = new ActivatePurchasedInventory.InventoryItem
//        {
//            keyName = UniqueNameForAttachment,
//            ObjectsToActivate = new GameObject[] { Attachment },
//            targetScript = TargetScript,
//            selectedFunctionName = SelectedFunctionName
//        };

//        activateComponent.inventoryItems.Add(inventoryItem);

//        // Add WeaponOptics component to the Attachment
//        Attachment.AddComponent<WeaponOptics>();

//        Debug.Log("Required components added successfully.");
//    }

//    private void ModifyExistingScript()
//    {
//        if (ExistingScript == null || string.IsNullOrEmpty(UniqueNameForAttachment) || !Attachment || !TargetScript || string.IsNullOrEmpty(SelectedFunctionName))
//        {
//            Debug.LogError("Please fill in all required fields before proceeding.");
//            return;
//        }

//        // Check if the UniqueNameForAttachment is already present
//        if (ExistingScript.inventoryItems.Any(item => item.keyName == UniqueNameForAttachment))
//        {
//            Debug.LogError("<color=red>Name already present. Use a different one.</color>");
//            return;
//        }

//        // Create a new InventoryItem
//        var inventoryItem = new ActivatePurchasedInventory.InventoryItem
//        {
//            keyName = UniqueNameForAttachment,
//            ObjectsToActivate = new GameObject[] { Attachment },
//            targetScript = TargetScript,
//            selectedFunctionName = SelectedFunctionName
//        };

//        // Add the InventoryItem to the next available index in the inventoryItems list
//        ExistingScript.inventoryItems.Add(inventoryItem);

//        Debug.Log("Existing script modified successfully.");
//    }

//}



//using UnityEngine;
//using UnityEditor;
//using System.Linq;
//using MobileActionKit;

//public class AddWeaponAttachmentsWindow : EditorWindow
//{
//    public GameObject AttachmentRootGameObject;
//    public GameObject Weapon;
//    public string UniqueNameForAttachment;
//    public GameObject Attachment;
//    public MonoBehaviour TargetScript;
//    public string SelectedFunctionName;

//    [MenuItem("Tools/MobileActionKit/Player/Weapon/Add Weapon Attachments")]
//    public static void ShowWindow()
//    {
//        GetWindow<AddWeaponAttachmentsWindow>("Add Weapon Attachments");
//    }

//    private void OnGUI()
//    {
//        GUILayout.Label("Add Weapon Attachments", EditorStyles.boldLabel);

//        // Fields for user input
//        AttachmentRootGameObject = (GameObject)EditorGUILayout.ObjectField("Attachment Root", AttachmentRootGameObject, typeof(GameObject), true);
//        Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
//        UniqueNameForAttachment = EditorGUILayout.TextField("Unique Name", UniqueNameForAttachment);
//        Attachment = (GameObject)EditorGUILayout.ObjectField("Attachment", Attachment, typeof(GameObject), true);
//        TargetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Target Script", TargetScript, typeof(MonoBehaviour), true);

//        if (TargetScript != null)
//        {
//            // Populate method names in a dropdown (only public void methods with no parameters, declared in the target script)
//            var methods = TargetScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);
//            string[] methodNames = System.Array.FindAll(methods, m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
//                                       .Select(m => m.Name).ToArray();

//            int selectedIndex = System.Array.IndexOf(methodNames, SelectedFunctionName);
//            selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

//            selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methodNames);
//            SelectedFunctionName = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
//        }
//        else
//        {
//            EditorGUILayout.HelpBox("Please assign a Target Script.", MessageType.Warning);
//        }

//        EditorGUILayout.Space();

//        // Button to add required components
//        if (GUILayout.Button("Add Required Components"))
//        {
//            AddRequiredComponents();
//        }
//    }

//    private void AddRequiredComponents()
//    {
//        if (!AttachmentRootGameObject || !Weapon || string.IsNullOrEmpty(UniqueNameForAttachment) || !Attachment || !TargetScript || string.IsNullOrEmpty(SelectedFunctionName))
//        {
//            Debug.LogError("Please fill in all required fields before proceeding.");
//            return;
//        }

//        // Create the empty GameObject as a child of AttachmentRootGameObject
//        GameObject attachmentHolder = new GameObject(Weapon.name + "Attachment");
//        attachmentHolder.transform.SetParent(AttachmentRootGameObject.transform);

//        // Add ActivatePurchasedInventory component
//        ActivatePurchasedInventory activateComponent = attachmentHolder.AddComponent<ActivatePurchasedInventory>();

//        // Create a new InventoryItem
//        var inventoryItem = new ActivatePurchasedInventory.InventoryItem
//        {
//            keyName = UniqueNameForAttachment,
//            ObjectsToActivate = new GameObject[] { Attachment },
//            targetScript = TargetScript,
//            selectedFunctionName = SelectedFunctionName
//        };

//        activateComponent.inventoryItems.Add(inventoryItem);

//        // Add WeaponOptics component to the Attachment
//        Attachment.AddComponent<WeaponOptics>();

//        Debug.Log("Required components added successfully.");
//    }
//}



//using UnityEngine;
//using UnityEditor;
//using System.Linq;

//namespace MobileActionKit
//{
//    public class AddWeaponAttachmentsEditorScript : EditorWindow
//    {
//        public GameObject AttachmentRootGameObject;
//        public GameObject Weapon;
//        public PlayerWeapon PlayerWeaponScript;
//        public string UniqueNameForAttachment;
//        public GameObject Attachment;
//        public MonoBehaviour TargetScript;
//        public string SelectedFunctionName;

//        [MenuItem("Tools/MobileActionKit/Player/Weapon/Add Weapon Attachments")]
//        public static void ShowWindow()
//        {
//            GetWindow<AddWeaponAttachmentsEditorScript>("Add Weapon Attachments");
//        }

//        private void OnGUI()
//        {
//            GUILayout.Label("Add Weapon Attachments", EditorStyles.boldLabel);

//            // Fields for user input
//            AttachmentRootGameObject = (GameObject)EditorGUILayout.ObjectField("Attachment Root", AttachmentRootGameObject, typeof(GameObject), true);
//            Weapon = (GameObject)EditorGUILayout.ObjectField("Weapon", Weapon, typeof(GameObject), true);
//            UniqueNameForAttachment = EditorGUILayout.TextField("Unique Name", UniqueNameForAttachment);
//            Attachment = (GameObject)EditorGUILayout.ObjectField("Attachment", Attachment, typeof(GameObject), true);
//            TargetScript = (MonoBehaviour)EditorGUILayout.ObjectField("Target Script", TargetScript, typeof(MonoBehaviour), true);

//            if (TargetScript != null)
//            {
//                // Populate method names in a dropdown
//                var methods = TargetScript.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//                string[] methodNames = System.Array.FindAll(methods, m => m.GetParameters().Length == 0)
//                                           .Select(m => m.Name).ToArray();

//                int selectedIndex = System.Array.IndexOf(methodNames, SelectedFunctionName);
//                selectedIndex = selectedIndex >= 0 ? selectedIndex : 0;

//                selectedIndex = EditorGUILayout.Popup("Select Function", selectedIndex, methodNames);
//                SelectedFunctionName = methodNames.Length > 0 ? methodNames[selectedIndex] : null;
//            }
//            else
//            {
//                EditorGUILayout.HelpBox("Please assign a Target Script.", MessageType.Warning);
//            }

//            EditorGUILayout.Space();

//            // Button to add required components
//            if (GUILayout.Button("Add Required Components"))
//            {
//                AddRequiredComponents();
//            }
//        }

//        private void AddRequiredComponents()
//        {
//            if (!AttachmentRootGameObject || !Weapon || string.IsNullOrEmpty(UniqueNameForAttachment) || !Attachment || !TargetScript || string.IsNullOrEmpty(SelectedFunctionName))
//            {
//                Debug.LogError("Please fill in all required fields before proceeding.");
//                return;
//            }

//            // Create the empty GameObject as a child of AttachmentRootGameObject
//            GameObject attachmentHolder = new GameObject(Weapon.name + "Attachment");
//            attachmentHolder.transform.SetParent(AttachmentRootGameObject.transform);

//            // Add ActivatePurchasedInventory component
//            ActivatePurchasedInventory activateComponent = attachmentHolder.AddComponent<ActivatePurchasedInventory>();

//            // Create a new InventoryItem
//            var inventoryItem = new ActivatePurchasedInventory.InventoryItem
//            {
//                keyName = UniqueNameForAttachment,
//                ObjectsToActivate = new GameObject[] { Attachment },
//                targetScript = TargetScript,
//                selectedFunctionName = SelectedFunctionName
//            };

//            activateComponent.inventoryItems.Add(inventoryItem);

//            // Add WeaponOptics component to the Attachment
//            Attachment.AddComponent<WeaponOptics>();

//            Debug.Log("Required components added successfully.");
//        }
//    }
//}