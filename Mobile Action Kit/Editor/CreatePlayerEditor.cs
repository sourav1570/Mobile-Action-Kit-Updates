using UnityEditor;
using UnityEngine;


namespace MobileActionKit
{
    public class CreatePlayerEditor : EditorWindow
    {
        [MenuItem("Tools/Mobile Action Kit/Player/Player/Create Player", priority = 1)]
        private static void CreatePlayer()
        {
            // Load the Basic Player Setup prefab from the "New Test Under Development" folder
            GameObject BasicSetupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Mobile Action Kit/Editor/Editor Prefabs/Others/Basic Player Setup.prefab");

            // Check if the prefab exists
            if (BasicSetupPrefab == null)
            {
                Debug.LogError("Basic player setup prefab not found.");
                return;
            }

            // Instantiate the Basic Player Setup prefab in the scene
            GameObject BasicSetupInstance = PrefabUtility.InstantiatePrefab(BasicSetupPrefab) as GameObject;
            if (BasicSetupInstance != null)
            {
                // Unpack the player instance completely
                PrefabUtility.UnpackPrefabInstance(BasicSetupInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                // Set the player instance to be at the root of the hierarchy
                BasicSetupInstance.transform.SetParent(null);

                // Collect all child transforms and unparent them
                Transform[] children = new Transform[BasicSetupInstance.transform.childCount];
                for (int i = 0; i < BasicSetupInstance.transform.childCount; i++)
                {
                    children[i] = BasicSetupInstance.transform.GetChild(i);
                }

                // Now unparent the collected children
                foreach (Transform child in children)
                {
                    child.SetParent(null);
                }

                // Destroy the BasicSetupInstance after unparenting
                Object.DestroyImmediate(BasicSetupInstance);
            }

            // Refresh the hierarchy window
            EditorApplication.RepaintHierarchyWindow();
        }
    }
}



//using UnityEditor;
//using UnityEngine;

//public class CreatePlayerEditor : EditorWindow
//{
//    [MenuItem("Tools/MobileActionKit/Player/Create Player")]
//    private static void CreatePlayer()
//    {
//        // Load the Player and Canvas 2D prefabs from the "New Test Under Development" folder
//        GameObject BasicSetupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/New Test Under Development/Basic Player Setup.prefab");


//        // Check if the prefabs exist
//        if (BasicSetupPrefab == null)
//        {
//            Debug.LogError("Basic player setup prefab not found.");
//            return;
//        }



//        // Instantiate the Player prefab in the scene
//        GameObject BasicSetupInstance = PrefabUtility.InstantiatePrefab(BasicSetupPrefab) as GameObject;
//        if (BasicSetupInstance != null)
//        {
//            // Unpack the player instance completely
//            PrefabUtility.UnpackPrefabInstance(BasicSetupInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
//            BasicSetupInstance.transform.SetParent(null); // Set the player to be at the root of the hierarchy
//        }


//        // Refresh the hierarchy window
//        EditorApplication.RepaintHierarchyWindow();
//    }
//}
