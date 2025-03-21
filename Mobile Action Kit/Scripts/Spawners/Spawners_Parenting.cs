using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class Spawners_Parenting : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script is designed as a helper script for the situation when it is needed to respawn died friendly Ai agents at the beginning" +
            " of next enemy wave without requiring the player to reach a trigger location for that to happen.  By attaching this script to a AiMaintainingSpawner with a trigger " +
            "collider and making it a child of the WaveSpawner and parenting this assembly to player you synchronise activation of AiMaintainingSpawner with next enemy wave thus having same trigger entering effect." +
            "When the game begins the script will unparent the EnemyWave1 from WaveSpawner child game object of the player and will revert this operation after delay set inside Min and MaxTimeToReparent fields." +
            "Reparenting will also reset position of EnemyWave1 gameobject along with its trigger, forcing it to snap back to Player`s coordinate, thus creating trigger enter event that would spawn player`s friendlies" +
            " and first wave of enemies at the same time." +
            "AiMaintainingSpawner will track the number of Ai agents it has to maintain and depending on the outcome of the previous wave will respawn at the begining of the next wave those Ai agents that died fighting previous wave." +
            "Immediately after previous wave is completely destroyed the unparenting cycle will be performed again.And this process will be repeated untill total number of waves is reached.  ";

        private Transform MyParent;

        [Tooltip("Minimum time until unparented collider will be reparented to its original parent game object.")]
        public float MinTimeTillReparent = 0.1f;
        [Tooltip("Maximum time until unparented collider will be reparented to its original parent game object.")]
        public float MaxTimeTillReparent = 0.2f;

        float TimeToReparenting;

        Vector3 CurrentPosition;

        private void OnEnable()
        {
            CurrentPosition = transform.localPosition;
            TimeToReparenting = Random.Range(MinTimeTillReparent, MaxTimeTillReparent);
            if (transform.parent != null)
            {
                MyParent = transform.parent;
            }
            transform.parent = null;
            StartCoroutine(Coro());
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(TimeToReparenting);
            if (MyParent != null)
            {
                transform.parent = MyParent;
                transform.localPosition = CurrentPosition;
            }
        }
    }
}
