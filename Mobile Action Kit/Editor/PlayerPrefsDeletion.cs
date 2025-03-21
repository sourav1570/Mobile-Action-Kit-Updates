using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class PlayerPrefsDeletion : Editor
    {
        [MenuItem("Tools/Mobile Action Kit/DeletePlayerPrefs", priority = 3)]
        public static void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}