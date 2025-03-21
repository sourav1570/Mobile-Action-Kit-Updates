using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MobileActionKit
{
    public class EditWeaponAnimationsForPlayer : EditorWindow
    {
        // private string AnimatorControllerName;
        private GameObject targetObject;
        private AnimationClip InitialPose;
        private AnimationClip Wield;
        private AnimationClip Remove;
        private AnimationClip Idle;
        private AnimationClip Run;
        private AnimationClip Walk;
        private AnimationClip Reload;
        private AnimationClip ReloadEmpty;
        private AnimationClip Shoot;

        private Vector2 scrollPosition;
        Animator anim;

        [MenuItem("Tools/Mobile Action Kit/Player/FireArms/Edit Weapon Animations", priority = 2)]
        private static void Init()
        {
            EditWeaponAnimationsForPlayer window = (EditWeaponAnimationsForPlayer)EditorWindow.GetWindow(typeof(EditWeaponAnimationsForPlayer));
            window.titleContent = new GUIContent("Edit Weapon Animations");
            window.Show();
        }
        private void OnGUI()
        {
            float windowWidth = position.width;
            float windowHeight = position.height;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            float labelWidth = windowWidth * 0.4f;
            float fieldWidth = windowWidth * 0.5f;
            float spaceHeight = windowHeight * 0.02f;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;

            GUILayout.Label("Target Object", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Animator Controller", labelStyle, GUILayout.Width(labelWidth));
            targetObject = (GameObject)EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.Space(spaceHeight);


            GUILayout.Label("ANIMATION CLIPS", EditorStyles.boldLabel);

            GUILayout.Space(spaceHeight);

            GUILayout.BeginHorizontal();
            GUILayout.Label("InitialPose", labelStyle, GUILayout.Width(labelWidth));
            InitialPose = (AnimationClip)EditorGUILayout.ObjectField(InitialPose, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wield", labelStyle, GUILayout.Width(labelWidth));
            Wield = (AnimationClip)EditorGUILayout.ObjectField(Wield, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Remove", labelStyle, GUILayout.Width(labelWidth));
            Remove = (AnimationClip)EditorGUILayout.ObjectField(Remove, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Idle", labelStyle, GUILayout.Width(labelWidth));
            Idle = (AnimationClip)EditorGUILayout.ObjectField(Idle, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Run", labelStyle, GUILayout.Width(labelWidth));
            Run = (AnimationClip)EditorGUILayout.ObjectField(Run, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Walk", labelStyle, GUILayout.Width(labelWidth));
            Walk = (AnimationClip)EditorGUILayout.ObjectField(Walk, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Reload", labelStyle, GUILayout.Width(labelWidth));
            Reload = (AnimationClip)EditorGUILayout.ObjectField(Reload, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("ReloadEmpty", labelStyle, GUILayout.Width(labelWidth));
            ReloadEmpty = (AnimationClip)EditorGUILayout.ObjectField(ReloadEmpty, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shoot", labelStyle, GUILayout.Width(labelWidth));
            Shoot = (AnimationClip)EditorGUILayout.ObjectField(Shoot, typeof(AnimationClip), true, GUILayout.Width(fieldWidth));
            GUILayout.EndHorizontal();


            GUILayout.Space(spaceHeight);
            GUILayout.FlexibleSpace();



            if (GUILayout.Button(new GUIContent("Import Existing Animations From Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                ImportAnimations();
            }            
            if (GUILayout.Button(new GUIContent("Apply Animations To Animator")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                SaveAnimations();
               // EditorWindow.GetWindow<EditWeaponAnimationsForPlayer>().Close();
            }
            if (GUILayout.Button(new GUIContent("Clear All Animations Fields")))//, buttonStyle))//, GUILayout.MinWidth(buttonMinWidth), GUILayout.MaxWidth(buttonMaxWidth), GUILayout.MinHeight(buttonMinHeight), GUILayout.MaxHeight(buttonMaxHeight)))
            {
                ResetAnimations();
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndScrollView();

        }
        private AnimationClip[] GetAnimationClipsFromDraggedObjects()
        {
            Object[] draggedObjects = DragAndDrop.objectReferences;
            List<AnimationClip> clips = new List<AnimationClip>();

            foreach (Object obj in draggedObjects)
            {
                AnimationClip clip = obj as AnimationClip;
                if (clip != null)
                {
                    clips.Add(clip);
                }
            }

            return clips.ToArray();
        }




        private void AssignAnimationClipsToLayer(Animator animator, int layerIndex, AnimationClip[] clips)
        {
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            if (controller != null)
            {
                AnimatorControllerLayer[] layers = controller.layers;

                if (layerIndex >= 0 && layerIndex < layers.Length)
                {
                    AnimatorStateMachine stateMachine = layers[layerIndex].stateMachine;

                    foreach (AnimationClip clip in clips)
                    {
                        AnimatorState state = stateMachine.AddState(clip.name);
                        state.motion = clip;
                    }

                    Debug.Log("Animation clips assigned successfully.");
                }
                else
                {
                    Debug.LogWarning("Invalid layer index. Please enter a valid layer index.");
                }
            }
            else
            {
                Debug.LogWarning("Selected Animator does not have a valid Animator Controller.");
            }
        }
        private void ImportAnimations()
        {
            if (targetObject == null)
            {
                Debug.LogError("Target Object is not assigned.");
                return;
            }

            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Target Object does not have an Animator component.");
                return;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null)
            {
                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
                return;
            }

            InitialPose = GetAnimationClipByKeyword(animatorController, "InitialPose");
            Wield = GetAnimationClipByKeyword(animatorController, "Wield");

            Remove = GetAnimationClipByKeyword(animatorController, "Remove");
            Reload = GetAnimationClipByKeyword(animatorController, "Reload");
            Idle = GetAnimationClipByKeyword(animatorController, "Idle");
            Run = GetAnimationClipByKeyword(animatorController, "Run");
            Walk = GetAnimationClipByKeyword(animatorController, "Walk");

            ReloadEmpty = GetAnimationClipByKeyword(animatorController, "ReloadEmpty");
            Shoot = GetAnimationClipByKeyword(animatorController, "Shoot");
        }
        private void SaveAnimations()
        {
            ModifyAnimationSettingsWithFullChanges(InitialPose, "InitialPose", true);
            ModifyAnimationSettingsWithFullChanges(Wield, "Wield", false);
            ModifyAnimationSettingsWithFullChanges(Remove, "Remove", false);
            ModifyAnimationSettingsWithFullChanges(Reload, "Reload", false);
            ModifyAnimationSettingsWithFullChanges(Idle, "Idle", true);
            ModifyAnimationSettingsWithFullChanges(Run, "Run", true);
            ModifyAnimationSettingsWithFullChanges(Walk, "Walk", true);
            ModifyAnimationSettingsWithFullChanges(ReloadEmpty, "ReloadEmpty", false);
            ModifyAnimationSettingsWithFullChanges(Shoot, "Shoot", false);

            if (targetObject == null)
            {
                Debug.LogError("Target Object is not assigned.");
                return;
            }

            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Target Object does not have an Animator component.");
                return;
            }

            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null)
            {
                Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
                return;
            }

            // Set animation clips by keyword
            SetAnimationClipByKeyword(animatorController, "InitialPose", InitialPose);
            SetAnimationClipByKeyword(animatorController, "Wield", Wield);
            SetAnimationClipByKeyword(animatorController, "Remove", Remove);
            SetAnimationClipByKeyword(animatorController, "Reload", Reload);
            SetAnimationClipByKeyword(animatorController, "Idle", Idle);
            SetAnimationClipByKeyword(animatorController, "Run", Run);
            SetAnimationClipByKeyword(animatorController, "Walk", Walk);
            SetAnimationClipByKeyword(animatorController, "ReloadEmpty", ReloadEmpty);
            SetAnimationClipByKeyword(animatorController, "Shoot", Shoot);

            // Save the changes made to the animator controller
            EditorUtility.SetDirty(animatorController);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void ModifyAnimationSettingsWithFullChanges(AnimationClip clip, string fieldName,bool ShouldLoop)
        {
            if (clip != null)
            {
                // Set the new clip's name
                clip.name = fieldName;

                // Get the AnimationClipSettings
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);

                // Modify the loop time and loop pose properties
                settings.loopTime = ShouldLoop;
                settings.loopBlend = ShouldLoop;

                // Apply the modified settings to the AnimationClip
                AnimationUtility.SetAnimationClipSettings(clip, settings);

                // Debug the path where the clip is located
                string assetPath = AssetDatabase.GetAssetPath(clip);
               // Debug.Log("Original Asset Path: " + assetPath);

                // Save the changes made to the clip
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

               // Debug.Log("Animation clip renamed successfully.");
            }
            else
            {
               // Debug.LogWarning("Animation clip is null.");
            }
        }
        private void ResetAnimations()
        {
            InitialPose = null;
            Wield = null;
            Remove = null;
            Reload = null;
            Idle = null;

            Run = null;
            Walk = null;
            ReloadEmpty = null;
            Shoot = null;
        }

        private AnimationClip GetAnimationClipByKeyword(AnimatorController animatorController, string keyword)
        {
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.name.Contains(keyword))
                    {
                        return state.state.motion as AnimationClip;
                    }
                }
            }

            return null;
        }
        private void SetAnimationClipByKeyword(AnimatorController animatorController, string keyword, AnimationClip animationClip)
        {
            foreach (AnimatorControllerLayer layer in animatorController.layers)
            {
                foreach (ChildAnimatorState state in layer.stateMachine.states)
                {
                    if (state.state.name.Contains(keyword))
                    {
                        state.state.motion = animationClip;
                    }
                }
            }
        }
        //private void DuplicateAnimator()
        //{
        //    if (string.IsNullOrEmpty(AnimatorControllerName))
        //    {
        //        Debug.LogError("Animator Name is not provided.");
        //        return;
        //    }

        //    AnimatorControllerName = AnimatorControllerName.Trim(); // Trim leading and trailing spaces

        //    if (targetObject == null)
        //    {
        //        Debug.LogError("Target Object is not assigned.");
        //        return;
        //    }

        //    Animator sourceAnimator = targetObject.GetComponent<Animator>();
        //    if (sourceAnimator == null)
        //    {
        //        Debug.LogError("Target Object does not have an Animator component.");
        //        return;
        //    }

        //    AnimatorController sourceController = sourceAnimator.runtimeAnimatorController as AnimatorController;
        //    if (sourceController == null)
        //    {
        //        Debug.LogError("Target Object's Animator does not have an AnimatorController assigned.");
        //        return;
        //    }
        //    string controllerPath = "Assets/" + AnimatorControllerName + ".controller";
        //    AnimatorController duplicatedController = new AnimatorController();
        //    duplicatedController.name = AnimatorControllerName;
        //    EditorUtility.CopySerialized(sourceController, duplicatedController);
        //    AssetDatabase.CreateAsset(duplicatedController, controllerPath);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //    Debug.Log("Animator duplicated and saved as: " + controllerPath);
        //    sourceAnimator.runtimeAnimatorController = duplicatedController;
        //    Debug.Log("Animator duplicated and assigned to target object: " + controllerPath);
        //}
        //private void ModifyAnimationSettings(AnimationClip clip)
        //{
        //    if (clip != null)
        //    {
        //        ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

        //        if (modelImporter != null)
        //        {
        //            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

        //            foreach (var clipAnimation in clipAnimations)
        //            {
        //                // Activate "Bake Into Pose"
        //                clipAnimation.lockRootRotation = true;
        //                clipAnimation.lockRootHeightY = true;

        //                // Choose wrap mode based on original animation
        //                clipAnimation.loop = clip.isLooping;

        //                // Set Root Transform Rotation, Loop Pose, and Loop Time to Original
        //                clipAnimation.loopTime = true;
        //                clipAnimation.loopPose = true;

        //                clipAnimation.keepOriginalOrientation = true;

        //                // Set Based Upon to Original for Root Transform Rotation
        //                clipAnimation.cycleOffset = 0f;

        //            }

        //            modelImporter.clipAnimations = clipAnimations;
        //            modelImporter.SaveAndReimport();
        //        }
        //    }
        //}
        //private void ModifyAnimationSettingsWithFullChanges(AnimationClip clip, string FieldName)
        //{
        //    if (clip != null)
        //    {
        //        ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as ModelImporter;

        //        if (modelImporter != null)
        //        {
        //            //ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

        //            //foreach (var clipAnimation in clipAnimations)
        //            //{
        //            //    if(clipAnimations.ToString() == clip.name)
        //            //    {
        //            //        clipAnimation.name = FieldName;
        //            //    }
        //            //}

        //            //  modelImporter.clipAnimations = clipAnimations;

        //            clip.name = FieldName;
        //            modelImporter.SaveAndReimport();
        //        }
        //    }
        //}
    }
}