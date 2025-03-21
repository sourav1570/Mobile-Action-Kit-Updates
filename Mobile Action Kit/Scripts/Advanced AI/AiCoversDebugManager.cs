using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MobileActionKit
{
    public class AiCoversDebugManager : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "The 'AiCoversDebugManager' script is responsible for managing debug information related to cover states for AI agents in unity editor. " +
            "It retrieves data from DebugCoverState components attached to various game objects and updates debug visuals in the SceneView during Unity Editor mode. " +
            "This script helps developers in visualizing and debugging cover state information for Humanoid AI agents behaviour.";

        DebugCoverState[] AllDebugCoverStatesPoints;

        int AssignedCombatCover;
        int AssignedEmergencyCover;

        CoverNode[] AllCombatCoverNodes;
        EmergencyCoverNode[] AllEmergencyCoverNode;

        int ExistingCombatCoverNodes;
        int ExistingEmergencyCoverNodes;

        private void Start()
        {
            AllDebugCoverStatesPoints = FindObjectsOfType<DebugCoverState>();

            for (int x = 0; x < AllDebugCoverStatesPoints.Length; x++)
            {
                ExistingCombatCoverNodes = ExistingCombatCoverNodes + AllDebugCoverStatesPoints[x].combatcoverNodes.Length;
                ExistingEmergencyCoverNodes = ExistingEmergencyCoverNodes + AllDebugCoverStatesPoints[x].emergencycoverNodes.Length;
            }

            AllCombatCoverNodes = new CoverNode[ExistingCombatCoverNodes];
            AllEmergencyCoverNode = new EmergencyCoverNode[ExistingEmergencyCoverNodes];

            for (int x =0;x < AllDebugCoverStatesPoints.Length; x++)
            {
                for(int p = 0; p < AllDebugCoverStatesPoints[x].combatcoverNodes.Length; p++)
                {
                    AllCombatCoverNodes[AssignedCombatCover] = AllDebugCoverStatesPoints[x].combatcoverNodes[p].GetComponent<CoverNode>();
                    ++AssignedCombatCover;
                }

                for (int p = 0; p < AllDebugCoverStatesPoints[x].emergencycoverNodes.Length; p++)
                {
                    AllEmergencyCoverNode[AssignedEmergencyCover] = AllDebugCoverStatesPoints[x].emergencycoverNodes[p].GetComponent<EmergencyCoverNode>();
                    ++AssignedEmergencyCover;
                }
            }
        }
        void Update()
        {
#if UNITY_EDITOR
            // Rotate towards the SceneView camera

            for(int x = 0;x < AllCombatCoverNodes.Length; x++)
            {
                if (AllCombatCoverNodes[x].spawnedText != null)
                {
                    if(AllCombatCoverNodes[x].spawnedText.gameObject.activeInHierarchy == true)
                    {
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView != null)
                        {
                            AllCombatCoverNodes[x].spawnedText.transform.rotation = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);
                        }
                        AllCombatCoverNodes[x].spawnedText.transform.position = AllCombatCoverNodes[x].transform.position + AllCombatCoverNodes[x].DebugCoverStateScript.DebugInfoTextUIOffset;
                    }
                
                }
            }

            for (int x = 0; x < AllEmergencyCoverNode.Length; x++)
            {
                if (AllEmergencyCoverNode[x].spawnedText != null)
                {
                    if (AllEmergencyCoverNode[x].spawnedText.gameObject.activeInHierarchy == true)
                    {
                        SceneView sceneView = SceneView.lastActiveSceneView;
                        if (sceneView != null)
                        {
                            AllEmergencyCoverNode[x].spawnedText.transform.rotation = Quaternion.LookRotation(sceneView.camera.transform.forward, sceneView.camera.transform.up);
                        }
                        AllEmergencyCoverNode[x].spawnedText.transform.position = AllEmergencyCoverNode[x].transform.position + AllEmergencyCoverNode[x].DebugCoverStateScript.DebugInfoTextUIOffset;
                    }
                }
            }


#endif
        }
    }
}