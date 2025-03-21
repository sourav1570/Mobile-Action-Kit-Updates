using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MobileActionKit
{
    public class ExtractAnimationClip : EditorWindow
    {
        private AnimationClip animationClip;
        private string clipName = "NewAnimationClipName";

        [MenuItem("Tools/Mobile Action Kit/Humanoid AI/Duplicate Animation Clip", priority = 2)]
        private static void Init()
        {
            ExtractAnimationClip window = (ExtractAnimationClip)EditorWindow.GetWindow(typeof(ExtractAnimationClip));
            window.titleContent = new GUIContent("Duplicate Animation Clip");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Drag and Drop Animation Clip Here:");
            animationClip = EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false) as AnimationClip;

            GUILayout.Label("Enter Name for Animation Clip:");
            clipName = EditorGUILayout.TextField("Clip Name:", clipName);

            if (GUILayout.Button("Duplicate clip"))
            {
                if (animationClip != null)
                {
                    // Get the path of the original animation clip
                    string originalClipPath = AssetDatabase.GetAssetPath(animationClip);

                    // Duplicate the animation clip
                    AnimationClip newClip = Instantiate(animationClip);

                    // Rename the duplicated clip
                    newClip.name = clipName;

                    // Construct the path for the duplicated clip
                    string duplicatedClipPath = System.IO.Path.GetDirectoryName(originalClipPath) + "/" + clipName + ".anim";

                    // Save the new clip at the same location
                    AssetDatabase.CreateAsset(newClip, duplicatedClipPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Debug.Log("Animation clip extracted and saved as: " + duplicatedClipPath);
                }
                else
                {
                    Debug.LogWarning("No animation clip selected to extract.");
                }
            }
        }
    }
}
