using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(GameobjectToggler))]
    public class GameObjectTogglerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GameobjectToggler toggler = (GameobjectToggler)target;

            if (GUILayout.Button(toggler.AreGameObjectsCurrentlyActive ? "Deactivate All GameObjects" : "Activate All GameObjects"))
            {
                toggler.ToggleGameObjects();
            }

            if (GUILayout.Button("Nest Under New Parent"))
            {
                toggler.CreateParentForAdditionalGameObjects();
            }

            if (GUILayout.Button("Remove Nested GameObjects"))
            {
                toggler.RemoveNestedGameObjects();
            }

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(toggler);
            }
        }
    }
}








//using UnityEditor;
//using UnityEngine;

//namespace MobileActionKit
//{
//    [CustomEditor(typeof(GameobjectToggler))]
//    public class GameObjectTogglerEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            DrawDefaultInspector();

//            GameobjectToggler toggler = (GameobjectToggler)target;

//            if (GUILayout.Button(toggler.AreGameObjectsCurrentlyActive ? "Deactivate All GameObjects" : "Activate All GameObjects"))
//            {
//                toggler.ToggleGameObjects();
//            }

//            // Parent Already Exist Checkbox
//           // toggler.ParentAlreadyExists = EditorGUILayout.Toggle("Parent Already Exist", toggler.ParentAlreadyExists);

//            //if (toggler.ParentAlreadyExists)
//            //{
//                // Add Additional GameObjects Checkbox
//               // toggler.AddAdditionalGameObjects = EditorGUILayout.Toggle("Add Additional GameObjects", toggler.AddAdditionalGameObjects);

//                //if (toggler.AddGameObjectsWithNoParent)
//                //{
//                    // Display Additional GameObjects List
//                   // SerializedProperty additionalObjects = serializedObject.FindProperty("GameObjectsWithNoParent");
//                //   EditorGUILayout.PropertyField(additionalObjects, true);

//                    // Create Parent for Additional GameObjects Button
//                    if (GUILayout.Button("Nest Under New Parent"))
//                    {
//                        toggler.CreateParentForAdditionalGameObjects();
//                    }
//                //}
//           // }

//            if (GUI.changed)
//            {
//                serializedObject.ApplyModifiedProperties();
//                EditorUtility.SetDirty(toggler);
//            }
//        }
//    }
//}


//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(GameobjectToggler))]
//public class GameObjectTogglerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        GameobjectToggler toggler = (GameobjectToggler)target;

//        if (GUILayout.Button(toggler.AreObjectsCurrentlyActive ? "Deactivate All GameObjects" : "Activate All GameObjects"))
//        {
//            toggler.ToggleGameObjects();
//        }

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(toggler);
//        }
//    }
//}



//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(GameobjectToggler))]
//public class GameObjectTogglerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        GameobjectToggler toggler = (GameobjectToggler)target;

//        if (GUILayout.Button(toggler.areObjectsActive ? "Deactivate All GameObjects" : "Activate All GameObjects"))
//        {
//            toggler.ToggleGameObjects();
//        }
//    }
//}
