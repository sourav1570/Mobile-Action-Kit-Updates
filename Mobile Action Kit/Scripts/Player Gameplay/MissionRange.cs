using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class MissionRange : MonoBehaviour
    {
        public GameObject OutOfRangeUI;
        public float WarningTime = 10;
        public float TimerUnit = 1f;
        public Text TimerText;

        public GameObject[] UIToActivateOnMissionFailed;
        public GameObject[] UIToDeactivateOnMissionFailed;

        bool StopTimer = false;

        float DefaultWarningTimer;

        void Start()
        {
            DefaultWarningTimer = WarningTime;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                StopTimer = false;
                OutOfRangeUI.SetActive(true);
                StartCoroutine(WarningCoroutine());

            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                StopTimer = true;
                OutOfRangeUI.SetActive(false);
                StopCoroutine(WarningCoroutine());
                WarningTime = DefaultWarningTimer;
            }
        }
        IEnumerator WarningCoroutine()
        {
            while (WarningTime > 0 && StopTimer == false)
            {
                TimerText.text = WarningTime.ToString();
                yield return new WaitForSeconds(TimerUnit);
                WarningTime--;
            }
            if (WarningTime <= 0)
            {
                for (int x = 0; x < UIToActivateOnMissionFailed.Length; x++)
                {
                    UIToActivateOnMissionFailed[x].SetActive(true);
                }
                for (int x = 0; x < UIToDeactivateOnMissionFailed.Length; x++)
                {
                    UIToDeactivateOnMissionFailed[x].SetActive(false);
                }
                OutOfRangeUI.SetActive(false);
            }
        }
    }
}