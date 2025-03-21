using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;


namespace MobileActionKit
{
    public class RenameAndDeleteAnimation : EditorWindow
    {
        private GameObject selectedObject;
        private string newObjectName;
        private bool isValidDrag;

        [MenuItem("Tools/Mobile Action Kit/Humanoid AI/RenameAndModifyAnimations")]
        static void Init()
        {
            RenameAndDeleteAnimation window = (RenameAndDeleteAnimation)EditorWindow.GetWindow(typeof(RenameAndDeleteAnimation));
            window.Show();
        }

        void OnGUI()
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                isValidDrag = IsDragValid();
            }

            if (currentEvent.type == EventType.DragPerform && isValidDrag)
            {
                DragAndDrop.AcceptDrag();
                OnDragPerform();
            }

            GUILayout.Label("Drag and Drop Animated Model", EditorStyles.boldLabel);

            if (isValidDrag)
            {
                selectedObject = EditorGUILayout.ObjectField("GameObject:", selectedObject, typeof(GameObject), true) as GameObject;
                newObjectName = EditorGUILayout.TextField("New Name:", newObjectName);

                if (GUILayout.Button("Rename GameObject And Animation Clip"))
                {
                    RenameAndUpdateAnimations();
                }
            }
        }

        bool IsDragValid()
        {
            foreach (Object draggedObject in DragAndDrop.objectReferences)
            {
                if (draggedObject is GameObject)
                {
                    return true;
                }
            }
            return false;
        }

        void OnDragPerform()
        {
            if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is GameObject)
            {
                selectedObject = (GameObject)DragAndDrop.objectReferences[0];
                newObjectName = selectedObject.name;
                isValidDrag = true;
            }
        }

        void RenameAndUpdateAnimations()
        {
            if (selectedObject == null)
            {
                Debug.LogError("Please select a GameObject.");
                return;
            }

            if (string.IsNullOrEmpty(newObjectName))
            {
                Debug.LogError("Please enter a new name for the GameObject.");
                return;
            }

            // Rename GameObject in the scene
            selectedObject.name = newObjectName;

            // Get the model path
            string modelPath = AssetDatabase.GetAssetPath(selectedObject);

            //  Rename the model asset file in the project
            string newModelPath = modelPath.Replace(selectedObject.name, newObjectName);
            AssetDatabase.RenameAsset(modelPath, newObjectName);
            AssetDatabase.ImportAsset(newModelPath);

            // Introduce a delay to ensure the model renaming is complete before updating animation clips
            EditorApplication.update += UpdateAnimationClips;
        }

        void UpdateAnimationClips()
        {
            // Get the model path
            string modelPath = AssetDatabase.GetAssetPath(selectedObject);

            // Get the importer for the model after renaming
            ModelImporter modelImporter = AssetImporter.GetAtPath(modelPath) as ModelImporter;

            if (modelImporter != null)
            {
                // Remove all animation clips from the import settings except "mixamo.com"
                ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
                modelImporter.clipAnimations = new ModelImporterClipAnimation[] { }; // Clear existing animations

                foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
                {
                    if (clipAnimation.name == "mixamo.com")
                    {
                        // Add back the "mixamo.com" clip to the import settings
                        clipAnimation.name = newObjectName; // Set the name to the desired one
                        modelImporter.clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
                        break;
                    }
                }

                // Apply changes
                modelImporter.SaveAndReimport();
            }
            else
            {
                Debug.LogError("Failed to get ModelImporter for the GameObject.");
            }

            // Remove the update callback to stop further updates
            EditorApplication.update -= UpdateAnimationClips;
        }
    }
}




//using UnityEngine;
//using UnityEditor;
//using UnityEditor.Animations;

//public class RenameAndDeleteAnimation : EditorWindow
//{
//    private GameObject selectedObject;
//    private string newObjectName;
//    private bool isValidDrag;

//    [MenuItem("Tools/MobileActionKit/Humanoid AI/RenameAndModifyAnimations")]
//    static void Init()
//    {
//        RenameAndDeleteAnimation window = (RenameAndDeleteAnimation)EditorWindow.GetWindow(typeof(RenameAndDeleteAnimation));
//        window.Show();
//    }

//    void OnGUI()
//    {
//        Event currentEvent = Event.current;

//        if (currentEvent.type == EventType.DragUpdated)
//        {
//            DragAndDrop.visualMode = IsDragValid() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
//            isValidDrag = IsDragValid();
//        }

//        if (currentEvent.type == EventType.DragPerform && isValidDrag)
//        {
//            DragAndDrop.AcceptDrag();
//            OnDragPerform();
//        }

//        GUILayout.Label("Select GameObject and Enter New Name", EditorStyles.boldLabel);

//        if (isValidDrag)
//        {
//            selectedObject = EditorGUILayout.ObjectField("GameObject:", selectedObject, typeof(GameObject), true) as GameObject;
//            newObjectName = EditorGUILayout.TextField("New Name:", newObjectName);

//            if (GUILayout.Button("Rename GameObject"))
//            {
//                RenameAndUpdateAnimations();
//            }
//            if (GUILayout.Button("Rename And Update Animation Clip"))
//            {
//                RenameAndUpdateAnimations();
//            }
//        }
//    }

//    bool IsDragValid()
//    {
//        foreach (Object draggedObject in DragAndDrop.objectReferences)
//        {
//            if (draggedObject is GameObject)
//            {
//                return true;
//            }
//        }
//        return false;
//    }

//    void OnDragPerform()
//    {
//        if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is GameObject)
//        {
//            selectedObject = (GameObject)DragAndDrop.objectReferences[0];
//            newObjectName = selectedObject.name;
//            isValidDrag = true;
//        }
//    }

//    void RenameAndUpdateAnimations()
//    {
//        if (selectedObject == null)
//        {
//            Debug.LogError("Please select a GameObject.");
//            return;
//        }

//        if (string.IsNullOrEmpty(newObjectName))
//        {
//            Debug.LogError("Please enter a new name for the GameObject.");
//            return;
//        }

//        // Rename GameObject in the scene
//        selectedObject.name = newObjectName;

//        // Get the model path
//        string modelPath = AssetDatabase.GetAssetPath(selectedObject);

//        //  Rename the model asset file in the project
//        string newModelPath = modelPath.Replace(selectedObject.name, newObjectName);
//        AssetDatabase.RenameAsset(modelPath, newObjectName);
//        AssetDatabase.ImportAsset(newModelPath);

//        // Get the importer for the model after renaming
//        ModelImporter modelImporter = AssetImporter.GetAtPath(newModelPath) as ModelImporter;

//        if (modelImporter != null)
//        {
//            // Remove all animation clips from the import settings except "mixamo.com"
//            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
//            modelImporter.clipAnimations = new ModelImporterClipAnimation[] { }; // Clear existing animations

//            foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
//            {
//                if (clipAnimation.name == "mixamo.com")
//                {
//                    // Add back the "mixamo.com" clip to the import settings
//                    clipAnimation.name = newObjectName; // Set the name to the desired one
//                    modelImporter.clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
//                    break;
//                }
//            }

//            // Apply changes
//            modelImporter.SaveAndReimport();
//        }
//    }
//}






//using UnityEngine;
//using UnityEditor;
//using UnityEditor.Animations;

//public class RenameAndDeleteAnimation : EditorWindow
//{
//    private GameObject selectedObject;
//    private string newObjectName;

//    [MenuItem("Custom/Rename and Delete Animation")]
//    static void Init()
//    {
//        RenameAndDeleteAnimation window = (RenameAndDeleteAnimation)EditorWindow.GetWindow(typeof(RenameAndDeleteAnimation));
//        window.Show();
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("Select GameObject and Enter New Name", EditorStyles.boldLabel);

//        selectedObject = EditorGUILayout.ObjectField("GameObject:", selectedObject, typeof(GameObject), true) as GameObject;
//        newObjectName = EditorGUILayout.TextField("New Name:", newObjectName);

//        if (GUILayout.Button("Rename And Update Animations"))
//        {
//            RenameAndUpdateAnimations();
//        }
//    }

//    void RenameAndUpdateAnimations()
//    {
//        if (selectedObject == null)
//        {
//            Debug.LogError("Please select a GameObject.");
//            return;
//        }

//        if (string.IsNullOrEmpty(newObjectName))
//        {
//            Debug.LogError("Please enter a new name for the GameObject.");
//            return;
//        }

//        // Rename GameObject in the scene
//        selectedObject.name = newObjectName;

//        // Get the model path
//        string modelPath = AssetDatabase.GetAssetPath(selectedObject);

//      //  Rename the model asset file in the project
//        string newModelPath = modelPath.Replace(selectedObject.name, newObjectName);
//        AssetDatabase.RenameAsset(modelPath, newObjectName);
//        AssetDatabase.ImportAsset(newModelPath);

//        // Get the importer for the model after renaming
//        ModelImporter modelImporter = AssetImporter.GetAtPath(newModelPath) as ModelImporter;

//        if (modelImporter != null)
//        {
//            // Remove all animation clips from the import settings except "mixamo.com"
//            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
//            modelImporter.clipAnimations = new ModelImporterClipAnimation[] { }; // Clear existing animations

//            foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
//            {
//                if (clipAnimation.name == "mixamo.com")
//                {
//                    // Add back the "mixamo.com" clip to the import settings
//                    clipAnimation.name = newObjectName; // Set the name to the desired one
//                    modelImporter.clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
//                    break;
//                }
//            }

//            // Apply changes
//            modelImporter.SaveAndReimport();
//        }
//        else
//        {
//            Debug.LogError("Failed to get ModelImporter for the GameObject.");
//        }
//    }
//}








//using UnityEngine;
//using UnityEditor;

//public class RenameAndDeleteAnimation : EditorWindow
//{
//    private GameObject selectedObject;
//    private string newObjectName;

//    [MenuItem("Custom/Rename and Delete Animation")]
//    static void Init()
//    {
//        RenameAndDeleteAnimation window = (RenameAndDeleteAnimation)EditorWindow.GetWindow(typeof(RenameAndDeleteAnimation));
//        window.Show();
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("Select GameObject and Enter New Name", EditorStyles.boldLabel);

//        selectedObject = EditorGUILayout.ObjectField("GameObject:", selectedObject, typeof(GameObject), true) as GameObject;
//        newObjectName = EditorGUILayout.TextField("New Name:", newObjectName);

//        if (GUILayout.Button("Rename And Update Animations"))
//        {
//            RenameAndUpdateAnimations();
//        }
//    }

//    void RenameAndUpdateAnimations()
//    {
//        if (selectedObject == null)
//        {
//            Debug.LogError("Please select a GameObject.");
//            return;
//        }

//        if (string.IsNullOrEmpty(newObjectName))
//        {
//            Debug.LogError("Please enter a new name for the GameObject.");
//            return;
//        }

//        // Rename GameObject in the scene
//        selectedObject.name = newObjectName;

//        // Get the model path
//        string modelPath = AssetDatabase.GetAssetPath(selectedObject);

//        // Rename the model asset file in the project
//        string newModelPath = modelPath.Replace(selectedObject.name, newObjectName);
//        AssetDatabase.RenameAsset(modelPath, newObjectName);
//        AssetDatabase.ImportAsset(newModelPath);

//        // Get the importer for the model after renaming
//        ModelImporter modelImporter = AssetImporter.GetAtPath(newModelPath) as ModelImporter;

//        if (modelImporter != null)
//        {
//            // Remove all animation clips from the import settings except "mixamo.com"
//            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
//            modelImporter.clipAnimations = new ModelImporterClipAnimation[] { }; // Clear existing animations

//            foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
//            {
//                if (clipAnimation.name == "mixamo.com")
//                {
//                    // Add back the "mixamo.com" clip to the import settings
//                    modelImporter.clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
//                    break;
//                }
//            }

//            // Apply changes
//            modelImporter.SaveAndReimport();
//        }
//    }
//}





//using UnityEngine;
//using UnityEditor;

//public class RenameAndDeleteAnimation : EditorWindow
//{
//    private GameObject selectedObject;
//    private string newObjectName;

//    [MenuItem("Custom/Rename and Delete Animation")]
//    static void Init()
//    {
//        RenameAndDeleteAnimation window = (RenameAndDeleteAnimation)EditorWindow.GetWindow(typeof(RenameAndDeleteAnimation));
//        window.Show();
//    }

//    void OnGUI()
//    {
//        GUILayout.Label("Select GameObject and Enter New Name", EditorStyles.boldLabel);

//        selectedObject = EditorGUILayout.ObjectField("GameObject:", selectedObject, typeof(GameObject), true) as GameObject;
//        newObjectName = EditorGUILayout.TextField("New Name:", newObjectName);

//        if (GUILayout.Button("Rename And Update Animations"))
//        {
//            RenameAndUpdateAnimations();
//        }
//    }

//    void RenameAndUpdateAnimations()
//    {
//        if (selectedObject == null)
//        {
//            Debug.LogError("Please select a GameObject.");
//            return;
//        }

//        if (string.IsNullOrEmpty(newObjectName))
//        {
//            Debug.LogError("Please enter a new name for the GameObject.");
//            return;
//        }

//        // Get the model path
//        string modelPath = AssetDatabase.GetAssetPath(selectedObject);

//        // Get the importer for the model
//        ModelImporter modelImporter = AssetImporter.GetAtPath(modelPath) as ModelImporter;

//        // Remove all animation clips from the import settings except "mixamo.com"
//        ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;
//        modelImporter.clipAnimations = new ModelImporterClipAnimation[] { }; // Clear existing animations

//        foreach (ModelImporterClipAnimation clipAnimation in clipAnimations)
//        {
//            if (clipAnimation.name == "mixamo.com")
//            {
//                // Add back the "mixamo.com" clip to the import settings
//                modelImporter.clipAnimations = new ModelImporterClipAnimation[] { clipAnimation };
//                break;
//            }
//        }

//        // Apply changes
//        modelImporter.SaveAndReimport();
//    }
//}
