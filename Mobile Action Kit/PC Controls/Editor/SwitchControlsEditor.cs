using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    [CustomEditor(typeof(SwitchGameControls))]
    public class SwitchControlsEditor : Editor
    {
        string buttonText;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SwitchGameControls script = (SwitchGameControls)target;

            if(script.PcControls != null)
            {
                buttonText = script.PcControls.activeSelf ? "Activate Mobile Controls" : "Activate PC Controls";
            }

            if (GUILayout.Button(buttonText))
            {
                script.ToggleControls();
            }
        }
    }
}