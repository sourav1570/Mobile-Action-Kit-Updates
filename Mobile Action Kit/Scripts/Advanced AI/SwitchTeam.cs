using UnityEngine;
using System.Collections.Generic;

namespace MobileActionKit
{
    public class SwitchTeam : MonoBehaviour
    {
        [TextArea]
        [Tooltip("This script ")]
        public string ScriptInfo = "This Script switches the team and uniform of Ai agent. It does so by automatically changing the team ID and the materials of AI agent and its child game objects(pouches,backpacks,goggles etc.)";

        private Targets TargetsScript;

        [System.Serializable]
        public class Teamid
        {
            [Tooltip("The name of the team to be assigned to this AI agent.")]
            public string EnterTeamName;

            [Tooltip("An array of skin materials to be applied to the SkinnedMeshRenderer of the AI agent when switching teams.")]
            public Material[] BodySkinnedMeshMaterials;

            [Tooltip("An array of materials to be applied to the MeshRenderer of the AI agent when switching teams.")]
            public Material[] BodyMeshRendererMaterials;

            [Tooltip("An array of materials to be applied to the MeshRenderer of the AI agent when switching teams.")]
            public Material[] WeaponMeshRendererMaterials;
        }

        [Tooltip("List of TeamIDs and materials set for each team.")]
        public List<Teamid> TeamsAndMaterials = new List<Teamid>();

        [HideInInspector]
        [Tooltip("The index of the current team configuration in the AddNewTeam list.")]
        public int teamNameIndex = 0;

        [Tooltip("Drag&Drop AI agent body skinned mesh to reference its SkinnedMeshRenderer component to.")]
        public SkinnedMeshRenderer[] BodySkinnedMesh;

        [Tooltip("Drag&Drop AI agent body child meshes to reference MeshRenderer components of the AI body child game objects(pouches,backpacks,weapons etc.).")]
        public MeshRenderer[] BodyMeshRenderer;

        [Tooltip("References to the MeshRenderer component of the AI weapon.")]
        public MeshRenderer[] WeaponMeshRenderer;

        [HideInInspector]
        [Tooltip("The index of the current skin material in the SkinMaterials array of the active team.")]
        public int skinnedMaterialIndex = 0;

        [HideInInspector]
        [Tooltip("The index of the current material in the MeshMaterials array of the active team.")]
        public int meshMaterialIndex = 0;

        public void ChangeTargetScriptTeamName()
        {
            TargetsScript = GetComponent<Targets>();
            if (TargetsScript != null)
            {
                teamNameIndex = (teamNameIndex + 1) % TeamsAndMaterials.Count;
                TargetsScript.MyTeamID = TeamsAndMaterials[teamNameIndex].EnterTeamName;

                if (BodySkinnedMesh != null)
                {
                    if (TeamsAndMaterials[teamNameIndex].BodySkinnedMeshMaterials != null && TeamsAndMaterials[teamNameIndex].BodySkinnedMeshMaterials.Length > 0)
                    {
                        for (int i = 0; i < BodySkinnedMesh.Length; i++)
                        {
                            BodySkinnedMesh[i].material = TeamsAndMaterials[teamNameIndex].BodySkinnedMeshMaterials[skinnedMaterialIndex];
                        }
                        skinnedMaterialIndex = (skinnedMaterialIndex + 1) % TeamsAndMaterials[teamNameIndex].BodySkinnedMeshMaterials.Length;
                    }
                }

                if (BodyMeshRenderer != null)
                {
                    if (TeamsAndMaterials[teamNameIndex].BodyMeshRendererMaterials != null && TeamsAndMaterials[teamNameIndex].BodyMeshRendererMaterials.Length > 0)
                    {
                        for (int i = 0; i < BodyMeshRenderer.Length; i++)
                        {
                            BodyMeshRenderer[i].material = TeamsAndMaterials[teamNameIndex].BodyMeshRendererMaterials[meshMaterialIndex];
                        }
                        meshMaterialIndex = (meshMaterialIndex + 1) % TeamsAndMaterials[teamNameIndex].BodyMeshRendererMaterials.Length;
                    }
                }

                if (WeaponMeshRenderer != null)
                {
                    if (TeamsAndMaterials[teamNameIndex].WeaponMeshRendererMaterials != null && TeamsAndMaterials[teamNameIndex].WeaponMeshRendererMaterials.Length > 0)
                    {
                        for (int i = 0; i < WeaponMeshRenderer.Length; i++)
                        {
                            WeaponMeshRenderer[i].material = TeamsAndMaterials[teamNameIndex].WeaponMeshRendererMaterials[meshMaterialIndex];
                        }
                        meshMaterialIndex = (meshMaterialIndex + 1) % TeamsAndMaterials[teamNameIndex].WeaponMeshRendererMaterials.Length;
                    }
                }


            }
        }
    }
}




//using UnityEngine;
//using System.Collections.Generic;

//namespace MobileActionKit
//{
//    public class SwitchTeam : MonoBehaviour
//    {
//        [TextArea]
//        [Tooltip("This script ")]
//        public string ScriptInfo = "This Script is responsible for changing team of Ai agent. It is useful in cases when you want to duplicate Ai agent after some tweaks have been made to its scripts. " +
//            "It facilitate a process of transfering any tweaked properties or values between different teams.";

//        private Targets TargetsScript;

//        [System.Serializable]
//        public class Teamid
//        {
//            [Tooltip("The name of the team to be assigned to the AI agent.")]
//            public string EnterTeamName;

//            [Tooltip("An array of skin materials to be applied to the SkinnedMeshRenderer of the AI agent when switching teams.")]
//            public Material[] SkinMaterials;

//            [Tooltip("An array of materials to be applied to the MeshRenderer of the AI agent when switching teams.")]
//            public Material[] MeshMaterials;
//        }

//        [Tooltip("A list of Teamid objects, each representing a different team configuration for the AI agent.")]
//        public List<Teamid> AddNewTeam = new List<Teamid>();

//        [HideInInspector]
//        [Tooltip("The index of the current team configuration in the AddNewTeam list.")]
//        public int teamNameIndex = 0;

//        [Tooltip("Reference to the SkinnedMeshRenderer component of the AI agent.")]
//        public SkinnedMeshRenderer SkinnedMeshRenderer;

//        [Tooltip("Reference to the MeshRenderer component of the AI agent.")]
//        public MeshRenderer MeshRenderer;

//        [HideInInspector]
//        [Tooltip("The index of the current skin material in the SkinMaterials array of the active team.")]
//        public int skinnedMaterialIndex = 0;

//        [HideInInspector]
//        [Tooltip("The index of the current material in the MeshMaterials array of the active team.")]
//        public int meshMaterialIndex = 0;

//        public void ChangeTargetScriptTeamName()
//        {
//            TargetsScript = GetComponent<Targets>();
//            if (TargetsScript != null)
//            {
//                teamNameIndex = (teamNameIndex + 1) % AddNewTeam.Count;
//                TargetsScript.MyTeamTag = AddNewTeam[teamNameIndex].EnterTeamName;

//                if (SkinnedMeshRenderer != null)
//                {
//                    if (AddNewTeam[teamNameIndex].SkinMaterials != null && AddNewTeam[teamNameIndex].SkinMaterials.Length > 0)
//                    {
//                        SkinnedMeshRenderer.material = AddNewTeam[teamNameIndex].SkinMaterials[skinnedMaterialIndex];
//                        skinnedMaterialIndex = (skinnedMaterialIndex + 1) % AddNewTeam[teamNameIndex].SkinMaterials.Length;
//                    }
//                }

//                if (MeshRenderer != null)
//                {
//                    if (AddNewTeam[teamNameIndex].MeshMaterials != null && AddNewTeam[teamNameIndex].MeshMaterials.Length > 0)
//                    {
//                        MeshRenderer.material = AddNewTeam[teamNameIndex].MeshMaterials[meshMaterialIndex];
//                        meshMaterialIndex = (meshMaterialIndex + 1) % AddNewTeam[teamNameIndex].MeshMaterials.Length;
//                    }
//                }

//            }

//        }

//    }
//}
