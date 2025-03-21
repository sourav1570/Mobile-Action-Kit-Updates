using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
    public class EnemyMovingBullets : MonoBehaviour
    {

        public float BulletSpeed = 20f;

        public float TimeToDeactiveBullet = 3f;

        private void OnEnable()
        {
            StartCoroutine(DeactiveBullets());
        }
        private void Update()
        {
            gameObject.transform.position += transform.forward * BulletSpeed * Time.deltaTime;
        }
        IEnumerator DeactiveBullets()
        {
            yield return new WaitForSeconds(TimeToDeactiveBullet);
            gameObject.SetActive(false);
        }
    }
}