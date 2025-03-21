using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class BulletHole : MonoBehaviour
    {

        public float TimeToDeactive = 0.15f;

        private void OnEnable()
        {
            StartCoroutine(DeactivebulletHole());
        }
        IEnumerator DeactivebulletHole()
        {
            yield return new WaitForSeconds(TimeToDeactive);
            gameObject.SetActive(false);
        }


    }
}
