using UnityEngine;

namespace MobileActionKit
{
    public class TouchPadController : MonoBehaviour
    {

        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Updates The Camera X and Y Rotation Every Frame";
        [Space(10)]
        public FirstPersonController FirstPersonControllerScript;
        public TouchPad TouchPadScript;
        public void ResettingDescription()
        {
            ScriptInfo = "This Script Updates The Camera X and Y Rotation Every Frame";
        }
        void Update()
        {
            FirstPersonControllerScript.TouchpadLook.LookAxis = TouchPadScript.TouchDistance;
        }
    }
}