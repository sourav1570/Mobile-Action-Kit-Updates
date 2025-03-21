using UnityEngine.UI;
using UnityEngine;

// Script For Debugging Number of Enemies Alive !

namespace MobileActionKit
{
    public class DeveloperDebugger : MonoBehaviour
    {

        public bool ShouldDebug = true;
        public Text TextToDebugAliveEnemies;

        void Update()
        {
            if (ShouldDebug == true)
            {
                GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                int Myenemy = Enemies.Length;
                TextToDebugAliveEnemies.text = "CURRENT" + " " + "ENEMIES" + " " + "-" + Myenemy.ToString();
            }
        }
    }
}
