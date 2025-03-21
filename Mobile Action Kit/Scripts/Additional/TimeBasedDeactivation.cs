using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class TimeBasedDeactivation : MonoBehaviour
    {
        public float TimeToDeactive = 2f;

        void OnEnable()
        {
            StartCoroutine(Coro());
        }
        IEnumerator Coro()
        {
            yield return new WaitForSeconds(TimeToDeactive);
            gameObject.SetActive(false);
        }
    }
}
