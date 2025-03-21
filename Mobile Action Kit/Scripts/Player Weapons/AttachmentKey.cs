using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class AttachmentKey : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "Enter the same key name for this attachment that you have entered in the attachment script of this weapon.";

        [Tooltip("Enter the same key name for this attachment that you have entered in the attachment script of this weapon.")]
        public string KeyName;
    }
}