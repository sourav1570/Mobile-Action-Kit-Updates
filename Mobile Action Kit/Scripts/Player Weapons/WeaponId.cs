using UnityEngine;

namespace MobileActionKit
{
    public class WeaponId : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This Script Store and Save The Weapon Information using the Unique Weapon Name ( Provided Below )";
        [Space(10)]

        [Tooltip("A Unique Name For Each Weapon To Save The Game Data * Weapon Names Cannot Be Similar")]
        public string WeaponName;

    }
}
