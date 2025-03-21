using UnityEngine;

namespace MobileActionKit
{
    public class AiUpdater : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script creates the lists of enemies and allies of each AI agent upon its spawn and also refreshes those lists for Ai agents that were present in game from the beginning. " +
            "It is required as an additional script for Humanoid Ai manager in case of spawning of Ai agents is taking place.";

        public void Checking()
        {
            //if (DroneAiManager.instance != null)
            //{
            //    DroneAiManager.instance.CheckingForNewEnemies();
            //}
            //if (MasterAiManager.instance != null)
            //{
            //    MasterAiManager.instance.CheckingForNewEnemies();
            //}
            if (HumanoidAiManager.instance != null)
            {
                HumanoidAiManager.instance.CheckingForNewEnemies();
            }
            //if (ZombieAIManager.instance != null)
            //{
            //    ZombieAIManager.instance.Finding();
            //}
            //TankAiManager.instance.CheckingForNewEnemies();
            //if (RotaryWingAiBehaviour.instance != null)
            //{
            //    RotaryWingAiBehaviour.instance.FindNewEnemies();
            //}
            //if (Turret.instance != null)
            //{
            //    Turret.instance.FindNewEnemies();
            //}
        }
    }
}