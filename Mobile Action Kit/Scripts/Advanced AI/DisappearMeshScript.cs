using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class DisappearMeshScript : MonoBehaviour
    {
        [HideInInspector]
        public float BodyDisappearTime;

        public void StartDisappearMethod()
        {
            StartCoroutine(Disappear());
        }
        IEnumerator Disappear()
        {
            yield return new WaitForSeconds(BodyDisappearTime);           
            Destroy(transform.gameObject);
        }
    }
}
