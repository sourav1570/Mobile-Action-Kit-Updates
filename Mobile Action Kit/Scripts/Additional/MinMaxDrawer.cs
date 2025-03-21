namespace MobileActionKit
{
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [System.Serializable]
    public struct MinMax
    {
        public float Min;
        public float Max;

        public MinMax(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMax))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Calculate the rect for the Min and Max fields
            var minMaxRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Calculate the rect for Min
            var minRect = new Rect(minMaxRect.x, minMaxRect.y, minMaxRect.width * 0.4f, EditorGUIUtility.singleLineHeight);

            // Calculate the rect for Max
            var maxRect = new Rect(minMaxRect.x + minMaxRect.width * 0.5f, minMaxRect.y, minMaxRect.width * 0.4f, EditorGUIUtility.singleLineHeight);

            // Find the SerializedProperties for Min and Max
            var minProp = property.FindPropertyRelative("Min");
            var maxProp = property.FindPropertyRelative("Max");

            // Draw the "Min" label before the Min input field
            EditorGUI.LabelField(minRect, new GUIContent("Min"));
            minRect.x += 30f;

            // Draw the "Max" label before the Max input field
            EditorGUI.LabelField(maxRect, new GUIContent("Max"));
            maxRect.x += 30f;//maxRect.width;

            // Draw the Min input field
            EditorGUI.PropertyField(minRect, minProp, GUIContent.none);

            // Draw the Max input field
            EditorGUI.PropertyField(maxRect, maxProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight; // Height is just for one line now
        }
    }
#endif




    //using UnityEngine;
    //#if UNITY_EDITOR
    //using UnityEditor;
    //#endif

    //[System.Serializable]
    //public struct MinMax
    //{
    //    public float Min;
    //    public float Max;

    //    public MinMax(float min, float max)
    //    {
    //        Min = min;
    //        Max = max;
    //    }
    //}
    //#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(MinMax))]
    //public class MinMaxDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        EditorGUI.BeginProperty(position, label, property);

    //        // Calculate the rect for the Min and Max fields
    //        var minMaxRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //        // Calculate the rect for Min
    //        var minRect = new Rect(minMaxRect.x, minMaxRect.y, minMaxRect.width * 0.45f, EditorGUIUtility.singleLineHeight);

    //        // Calculate the rect for Max
    //        var maxRect = new Rect(minMaxRect.x + minMaxRect.width * 0.5f, minMaxRect.y, minMaxRect.width * 0.45f, EditorGUIUtility.singleLineHeight);

    //        // Find the SerializedProperties for Min and Max
    //        var minProp = property.FindPropertyRelative("Min");
    //        var maxProp = property.FindPropertyRelative("Max");

    //        // Draw the Min and Max fields
    //        EditorGUI.PropertyField(minRect, minProp, GUIContent.none);
    //        EditorGUI.PropertyField(maxRect, maxProp, GUIContent.none);

    //        EditorGUI.EndProperty();
    //    }

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return EditorGUIUtility.singleLineHeight; // Height is just for one line now
    //    }
    //}
    //#endif


    //using UnityEngine;
    //#if UNITY_EDITOR
    //using UnityEditor;
    //#endif

    //[System.Serializable]
    //public struct MinMax
    //{
    //    public float Min;
    //    public float Max;

    //    public MinMax(float min, float max)
    //    {
    //        Min = min;
    //        Max = max;
    //    }
    //}

    //#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(MinMax))]
    //public class MinMaxDrawer : PropertyDrawer
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        EditorGUI.BeginProperty(position, label, property);

    //        // Calculate the rect for the Min and Max fields
    //        var minRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width * 0.3f - 1, EditorGUIUtility.singleLineHeight);
    //        var maxRect = new Rect(position.x + position.width * 0.35f + 0.5f, position.y + EditorGUIUtility.singleLineHeight, position.width * 0.3f - 1, EditorGUIUtility.singleLineHeight);

    //        // Find the SerializedProperties for Min and Max
    //        var minProp = property.FindPropertyRelative("Min");
    //        var maxProp = property.FindPropertyRelative("Max");

    //        // Draw the label for the entire MinMax field
    //        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //        // Draw the Min and Max fields using EditorGUI.MultiPropertyField
    //        EditorGUI.MultiPropertyField(minRect, new GUIContent[] { new GUIContent("Min") }, minProp);
    //        EditorGUI.MultiPropertyField(maxRect, new GUIContent[] { new GUIContent("Max") }, maxProp);

    //        EditorGUI.EndProperty();
    //    }

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        return EditorGUIUtility.singleLineHeight * 2; // Adjust the height to accommodate both fields and labels
    //    }
    //}
    //#endif
}