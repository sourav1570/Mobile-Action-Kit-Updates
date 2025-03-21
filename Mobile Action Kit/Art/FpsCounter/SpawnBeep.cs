using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

    public class SpawnBeep : MonoBehaviour
    {

        public Transform FpsClock;
        public Transform Parent;
        public GameObject spawnee;
        void OnTriggerEnter2D(Collider2D other)
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
            s.transform.SetParent(Parent);
            s.transform.localPosition = Vector3.zero;
            s.transform.SetParent(FpsClock);
        }
    }
}