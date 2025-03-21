using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

    public class Sipuko : MonoBehaviour
    {


        // Use this for initialization
        void Start()
        {
            StartCoroutine("SelfTermination");
        }

        IEnumerator SelfTermination()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}
