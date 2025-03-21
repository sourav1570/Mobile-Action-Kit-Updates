using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

    public class SpawnerTimed : MonoBehaviour
    {
        public Transform Parent;
        public GameObject spawnee;
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Noon")
            {
                StopCoroutine("SpawnDelay");
                StartCoroutine("SpawnDelay");
            }
        }
        IEnumerator SpawnDelay()
        {
            yield return new WaitForSeconds(1);
            GameObject s = Instantiate(spawnee, transform.position, transform.rotation);
            s.transform.parent = Parent;
        }
    }
}