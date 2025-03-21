using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MobileActionKit
{
    public class PlayerArmsMaterialsToReplace : MonoBehaviour
    {
        public SkinnedMeshRenderer[] PlayerArms;
        public Material Attire1Material;

        public void Attire1()
        {
            for (int x = 0; x < PlayerArms.Length; x++)
            {
                PlayerArms[x].material = Attire1Material;
            }
        }
    }
}