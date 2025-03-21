using UnityEditor;
using UnityEngine.AI;
using UnityEngine;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class SetupHumanoidAiAgentWindow : EditorWindow
    {
        private enum CharacterType { Soldier, Zombie }
        private CharacterType selectedType = CharacterType.Soldier;

        private GameObject ReferenceAIPrefab;
        private GameObject HumanoidModel;
        private Avatar HumanoidModelAvatar;
        private GameObject SpineBoneToRotate;
        private GameObject MyBodyPartToTarget;
        private GameObject WeaponModel;
        private GameObject WeaponSocket;

        private string RootObjName;
        GameObject Go;

        [MenuItem("Tools/Mobile Action Kit/Humanoid AI/Setup Humanoid AI Agent", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<SetupHumanoidAiAgentWindow>("Setup Humanoid Ai Agent");
        }

        private void OnGUI()
        {
            GUILayout.Label("Select Humanoid Ai Type:");
            selectedType = (CharacterType)EditorGUILayout.EnumPopup(selectedType);

            GUILayout.Label("Reference AI Prefab");
            ReferenceAIPrefab = (GameObject)EditorGUILayout.ObjectField(ReferenceAIPrefab, typeof(GameObject), true);

            GUILayout.Label("HumanoidModel");
            HumanoidModel = (GameObject)EditorGUILayout.ObjectField(HumanoidModel, typeof(GameObject), true);

            GUILayout.Label("HumanoidModel Avatar");
            HumanoidModelAvatar = (Avatar)EditorGUILayout.ObjectField(HumanoidModelAvatar, typeof(Avatar), true);

            if (selectedType == CharacterType.Soldier)
            {
                GUILayout.Label("SpineBoneToRotate");
                SpineBoneToRotate = (GameObject)EditorGUILayout.ObjectField(SpineBoneToRotate, typeof(GameObject), true); 
            }

            GUILayout.Label("MyBodyPartToTarget");
            MyBodyPartToTarget = (GameObject)EditorGUILayout.ObjectField(MyBodyPartToTarget, typeof(GameObject), true);

            EditorGUILayout.Space();

            if (selectedType == CharacterType.Soldier)
            {
                GUILayout.Label("Weapon Model");
                WeaponModel = (GameObject)EditorGUILayout.ObjectField(WeaponModel, typeof(GameObject), true);

                GUILayout.Label("Weapon Socket");
                WeaponSocket = (GameObject)EditorGUILayout.ObjectField(WeaponSocket, typeof(GameObject), true);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Configure Humanoid Ai Agent"))
            {
                if (HumanoidModel != null)
                {
                    if (selectedType == CharacterType.Soldier)
                    {
                        RootObjName = HumanoidModel.name;
                        Go = Instantiate(ReferenceAIPrefab, HumanoidModel.transform.position, HumanoidModel.transform.rotation);
                        if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                        {
                            Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.parent = null;
                        }
                        if (Go.GetComponent<SpineRotation>() != null)
                        {
                            Go.GetComponent<SpineRotation>().RotationalPivotBone.transform.parent = null;
                        }
                        Go.transform.name = RootObjName;

                        GameObject FirstChild = Go.transform.GetChild(0).gameObject;
                        GameObject SecondChild = Go.transform.GetChild(1).gameObject;
                        // Create a separate list of child objects
                        List<Transform> childObjects = new List<Transform>();
                        for (int x = 0; x < HumanoidModel.transform.childCount; x++)
                        {
                            childObjects.Add(HumanoidModel.transform.GetChild(x));
                        }

                        // Reparent the child objects
                        foreach (Transform child in childObjects)
                        {
                            Vector3 Position = child.localPosition;
                            Vector3 Rotation = child.localEulerAngles;

                            child.gameObject.layer = LayerMask.NameToLayer("AI");

                            child.parent = Go.transform;

                            child.localPosition = Position;
                            child.localEulerAngles = Rotation;
                        }

                        if (WeaponModel != null)
                        {
                            GameObject W = Instantiate(WeaponModel, Go.transform.position, Go.transform.rotation);
                            W.transform.parent = WeaponSocket.transform;
                            if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                            {
                                Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.parent = W.transform;
                            }

                            if (Go.GetComponent<SpineRotation>() != null)
                            {
                                Go.GetComponent<SpineRotation>().RotationalPivotBone.transform.parent = W.transform;
                            }

                            if (WeaponSocket != null)
                            {
                                WeaponSocket.transform.localPosition = Vector3.zero;
                                WeaponSocket.transform.localEulerAngles = Vector3.zero;
                                WeaponSocket.transform.localScale = new Vector3(1f, 1f, 1f);
                            }


                            W.transform.localPosition = Vector3.zero;
                            W.transform.localEulerAngles = Vector3.zero;
                            W.transform.localScale = new Vector3(1f, 1f, 1f);
                        }


                        if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                        {
                            Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.localPosition = Vector3.zero;
                            Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.localEulerAngles = Vector3.zero;
                        }

                        if (Go.GetComponent<SpineRotation>() != null)
                        {
                            Go.GetComponent<SpineRotation>().RotationalPivotBone.transform.localPosition = Vector3.zero;
                            Go.GetComponent<SpineRotation>().RotationalPivotBone.transform.localEulerAngles = Vector3.zero;
                        }

                        Go.GetComponent<Animator>().avatar = HumanoidModelAvatar;
                        Go.GetComponent<Targets>().MyBodyPartToTarget = MyBodyPartToTarget.transform;
                        if (Go.GetComponent<SpineRotation>() != null)
                        {
                            Go.GetComponent<SpineRotation>().BoneToRotate = SpineBoneToRotate.transform;
                        }
                        //if (EnableCharging)
                        //{
                        //    Go.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.EnableAiCharging = true;
                        //}


                        if (HumanoidModel != null)
                        {
                            DestroyImmediate(FirstChild);
                            DestroyImmediate(SecondChild);
                            DestroyImmediate(HumanoidModel);

                        }
                    }
                    else
                    {
                        RootObjName = HumanoidModel.name;
                        Go = Instantiate(ReferenceAIPrefab, HumanoidModel.transform.position, HumanoidModel.transform.rotation);
                        if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
                        {
                            Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.parent = null;
                        }
                        if (Go.GetComponent<SpineRotation>() != null)
                        {
                            Go.GetComponent<SpineRotation>().RotationalPivotBone.transform.parent = null;
                        }
                        Go.transform.name = RootObjName;

                        GameObject FirstChild = Go.transform.GetChild(0).gameObject;
                        GameObject SecondChild = Go.transform.GetChild(1).gameObject;
                        // Create a separate list of child objects
                        List<Transform> childObjects = new List<Transform>();
                        for (int x = 0; x < HumanoidModel.transform.childCount; x++)
                        {
                            childObjects.Add(HumanoidModel.transform.GetChild(x));
                        }

                        // Reparent the child objects
                        foreach (Transform child in childObjects)
                        {
                            Vector3 Position = child.localPosition;
                            Vector3 Rotation = child.localEulerAngles;

                            child.gameObject.layer = LayerMask.NameToLayer("AI");

                            child.parent = Go.transform;

                            child.localPosition = Position;
                            child.localEulerAngles = Rotation;
                        }

                        Go.GetComponent<Animator>().avatar = HumanoidModelAvatar;
                        Go.GetComponent<Targets>().MyBodyPartToTarget = MyBodyPartToTarget.transform;

                        if (HumanoidModel != null)
                        {
                            DestroyImmediate(FirstChild);
                            DestroyImmediate(SecondChild);
                            DestroyImmediate(HumanoidModel);

                        }
                    }
                }
            }

            if (GUILayout.Button("Create Ragdoll"))
            {
                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
            }

            if (GUILayout.Button("Customise Humanoid Animations"))
            {
                EditorApplication.ExecuteMenuItem("Tools/Mobile Action Kit/Humanoid AI/Edit Humanoid AI Animations");
            }
        }
    }
}









//using UnityEditor;
//using UnityEngine.AI;
//using UnityEngine;
//using System.Collections.Generic;

//namespace MobileActionKit
//{
//    public class SetupHumanoidAiAgentWindow : EditorWindow
//    {
//        //private GUIStyle headerStyle;

//        private GameObject ReferenceAIPrefab;
//        private GameObject HumanoidModel;
//        private Avatar HumanoidModelAvatar;
//        private GameObject BoneToRotateForIK;
//        private GameObject MyBodyPartToTarget;
//        private GameObject WeaponModel;
//        private GameObject WeaponSpawnPoint;

//        private string RootObjName;
//        GameObject Go;


//        [MenuItem("Tools/MobileActionKit/Humanoid AI/Setup Humanoid AI Agent", priority = 0)]
//        public static void ShowWindow()
//        {
//            GetWindow<SetupHumanoidAiAgentWindow>("Setup Humanoid Ai Agent");
//        }

//        private void OnGUI()
//        {

//            //if (headerStyle == null)
//            //{
//            //    headerStyle = new GUIStyle(GUI.skin.label);
//            //    headerStyle.fontStyle = FontStyle.Bold;
//            //    headerStyle.fontSize = 14;
//            //    headerStyle.padding.top = 10;
//            //}

//            //GUILayout.BeginHorizontal();
//            //GUILayout.Label("StandFireAnimationClip", labelStyle, GUILayout.Width(labelWidth));
//            //StandFireAnimationClip = (AnimationClip)EditorGUILayout.ObjectField(StandFireAnimationClip, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
//            //GUILayout.EndHorizontal();

//            float windowWidth = position.width;
//            float windowHeight = position.height;

//            // Calculate the size of GUI elements based on the window width and height
//            float labelWidth = windowWidth * 0.4f;
//            float fieldWidth = windowWidth * 0.5f;
//            float spaceHeight = windowHeight * 0.02f;
//            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
//            labelStyle.wordWrap = true;

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("ReferenceAI Prefab", labelStyle, GUILayout.Width(labelWidth));
//            ReferenceAIPrefab = (GameObject)EditorGUILayout.ObjectField(ReferenceAIPrefab, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Humanoid Model", labelStyle, GUILayout.Width(labelWidth));
//            HumanoidModel = (GameObject)EditorGUILayout.ObjectField(HumanoidModel, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            //EditorGUILayout.LabelField("YourRiggedModel");
//            //YourRiggedModel = EditorGUILayout.ObjectField(YourRiggedModel, typeof(GameObject), true) as GameObject;

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Humanoid Model Avatar", labelStyle, GUILayout.Width(labelWidth));
//            HumanoidModelAvatar = (Avatar)EditorGUILayout.ObjectField(HumanoidModelAvatar, typeof(Avatar), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            //EditorGUILayout.LabelField("ModelAvatar");
//            //ModelAvatar = EditorGUILayout.ObjectField(ModelAvatar, typeof(Avatar), true) as Avatar;

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("BoneToRotateForIK", labelStyle, GUILayout.Width(labelWidth));
//            BoneToRotateForIK = (GameObject)EditorGUILayout.ObjectField(BoneToRotateForIK, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            //EditorGUILayout.LabelField("BoneToRotateForIK");
//            //BoneToRotateForIK = EditorGUILayout.ObjectField(BoneToRotateForIK, typeof(GameObject), true) as GameObject;

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("MyBodyPartToTarget", labelStyle, GUILayout.Width(labelWidth));
//            MyBodyPartToTarget = (GameObject)EditorGUILayout.ObjectField(MyBodyPartToTarget, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();


//            //EditorGUILayout.LabelField("MyBodyPartToTarget");
//            //MyBodyPartToTarget = EditorGUILayout.ObjectField(MyBodyPartToTarget, typeof(GameObject), true) as GameObject;

//            EditorGUILayout.Space();

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Weapon Model", labelStyle, GUILayout.Width(labelWidth));
//            WeaponModel = (GameObject)EditorGUILayout.ObjectField(WeaponModel, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            //EditorGUILayout.LabelField("WeaponPrefab");
//            //WeaponPrefab = EditorGUILayout.ObjectField(WeaponPrefab, typeof(GameObject), true) as GameObject;

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Weapon SpawnPoint", labelStyle, GUILayout.Width(labelWidth));
//            WeaponSpawnPoint = (GameObject)EditorGUILayout.ObjectField(WeaponSpawnPoint, typeof(GameObject), true, GUILayout.Width(fieldWidth));
//            GUILayout.EndHorizontal();

//            //EditorGUILayout.LabelField("WeaponSpawnPoint");
//            //WeaponSpawnPoint = EditorGUILayout.ObjectField(WeaponSpawnPoint, typeof(GameObject), true) as GameObject;

//            EditorGUILayout.Space();

//            //EditorGUILayout.LabelField("Combat Behaviours", headerStyle); // Add the header

//            //EditorGUILayout.Space();

//            // EnableCharging = EditorGUILayout.Toggle("Enable Charging", EnableCharging);

//            //GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
//            //buttonStyle.normal.background = EditorGUIUtility.whiteTexture;
//            if (GUILayout.Button(new GUIContent("Configure Humanoid Bot")))//, buttonStyle))
//            {
//                if (HumanoidModel != null)
//                {
//                    RootObjName = HumanoidModel.name;
//                    Go = Instantiate(ReferenceAIPrefab, HumanoidModel.transform.position, HumanoidModel.transform.rotation);
//                    if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
//                    {
//                        Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.parent = null;
//                    }
//                    if (Go.GetComponent<SpineRotation>() != null)
//                    {
//                        Go.GetComponent<SpineRotation>().BoneRotator.transform.parent = null;
//                    }
//                    Go.transform.name = RootObjName;

//                    GameObject FirstChild = Go.transform.GetChild(0).gameObject;
//                    GameObject SecondChild = Go.transform.GetChild(1).gameObject;
//                    // Create a separate list of child objects
//                    List<Transform> childObjects = new List<Transform>();
//                    for (int x = 0; x < HumanoidModel.transform.childCount; x++)
//                    {
//                        childObjects.Add(HumanoidModel.transform.GetChild(x));
//                    }

//                    // Reparent the child objects
//                    foreach (Transform child in childObjects)
//                    {
//                        Vector3 Position = child.localPosition;
//                        Vector3 Rotation = child.localEulerAngles;

//                        child.gameObject.layer = LayerMask.NameToLayer("AI");

//                        child.parent = Go.transform;

//                        child.localPosition = Position;
//                        child.localEulerAngles = Rotation;
//                    }

//                    if (WeaponModel != null)
//                    {
//                        GameObject W = Instantiate(WeaponModel, Go.transform.position, Go.transform.rotation);
//                        W.transform.parent = WeaponSpawnPoint.transform;
//                        if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
//                        {
//                            Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.parent = W.transform;
//                        }

//                        if (Go.GetComponent<SpineRotation>() != null)
//                        {
//                            Go.GetComponent<SpineRotation>().BoneRotator.transform.parent = W.transform;
//                        }

//                        if (WeaponSpawnPoint != null)
//                        {
//                            WeaponSpawnPoint.transform.localPosition = Vector3.zero;
//                            WeaponSpawnPoint.transform.localEulerAngles = Vector3.zero;
//                            WeaponSpawnPoint.transform.localScale = new Vector3(1f, 1f, 1f);
//                        }


//                        W.transform.localPosition = Vector3.zero;
//                        W.transform.localEulerAngles = Vector3.zero;
//                        W.transform.localScale = new Vector3(1f, 1f, 1f);
//                    }


//                    if (Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent != null)
//                    {
//                        Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.localPosition = Vector3.zero;
//                        Go.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.transform.localEulerAngles = Vector3.zero;
//                    }

//                    if (Go.GetComponent<SpineRotation>() != null)
//                    {
//                        Go.GetComponent<SpineRotation>().BoneRotator.transform.localPosition = Vector3.zero;
//                        Go.GetComponent<SpineRotation>().BoneRotator.transform.localEulerAngles = Vector3.zero;
//                    }

//                    Go.GetComponent<Animator>().avatar = HumanoidModelAvatar;
//                    Go.GetComponent<Targets>().MyBodyPartToTarget = MyBodyPartToTarget.transform;
//                    if (Go.GetComponent<SpineRotation>() != null)
//                    {
//                        Go.GetComponent<SpineRotation>().BoneToRotate = BoneToRotateForIK.transform;
//                    }
//                    //if (EnableCharging)
//                    //{
//                    //    Go.GetComponent<CoreAiBehaviour>().CombatStateBehaviours.EnableAiCharging = true;
//                    //}


//                    if (HumanoidModel != null)
//                    {
//                        DestroyImmediate(FirstChild);
//                        DestroyImmediate(SecondChild);
//                        DestroyImmediate(HumanoidModel);

//                    }
//                }

//            }

//            // Add a button for creating the ragdoll
//            if (GUILayout.Button(new GUIContent("Create Ragdoll")))//, buttonStyle))
//            {
//                EditorApplication.ExecuteMenuItem("GameObject/3D Object/Ragdoll...");
//            }
//            // Add a button for creating the ragdoll
//            if (GUILayout.Button(new GUIContent("Customise Humanoid Animations")))//, buttonStyle))
//            {
//                EditorApplication.ExecuteMenuItem("Tools/MobileActionKit/Humanoid AI/Edit Humanoid AI Animations");
//            }
//        }
//    }
//}