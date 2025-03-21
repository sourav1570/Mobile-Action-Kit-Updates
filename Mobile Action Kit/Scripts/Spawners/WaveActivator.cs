using System.Collections;
using System.Collections.Generic;
using MobileActionKit;
using UnityEngine;
using UnityEngine.UI;

namespace MobileActionKit
{
    public class WaveActivator : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script sets the various parameters of wave activation and UI displayed during this process.";

        [Tooltip("Drag&drop the gameobject(usually player) from the hierarchy into this field to trigger spawn event.")]
        public GameObject TriggeringGameObject;

        [Tooltip("Drag and drop the 'Wave Spawner' script attached with the parent of this gameObject into this field.")]
        public WaveSpawner WaveSpawnerScript;

        [Tooltip("Specify the delay time of this wave.")]
        public float WaveStartingDelay = 10f;

        [Tooltip("If checked than the count down timer will be displayed before starting the next wave.")]
        public bool DisplayCountDownTimer = true;
        [Tooltip("Drag and drop the Text UI from the 2D canvas hierarchy into this field to display the count down")]
        public Text CountDownTimerText;

        [Tooltip("If checked wave counter text will be displayed in the form of '2D UI' that will show the number of next wave.")]
        public bool DisplayWaveCounterText = true;
        [Tooltip("Write the message to display before showing the wave number for example : Round or Wave")]
        public string TextBeforeWaveNumber;

        [Tooltip("Drag and drop the Text UI from the 2D canvas hierarchy into this field")]
        public Text WaveText;
        [Tooltip("Delay before deactivation of the wave related text.")]
        public float WaveCounterTextDeactivationTime = 2f;

        bool BeginTimer = false;

        private void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (WaveSpawnerScript.ActivationType != WaveSpawner.WaveType.EnterEachWaveTrigger)
            {
                gameObject.SetActive(false);
            }
        }
        public void StartWave()
        {
            StartCoroutine(WaveStartTimer());
        }
        IEnumerator WaveStartTimer()
        {
            if (DisplayCountDownTimer == true)
            {
                CountDownTimerText.gameObject.SetActive(true);
            }
            for (int countdown = Mathf.FloorToInt(WaveStartingDelay); countdown > 0; countdown--)
            {
                if (DisplayCountDownTimer == true)
                {
                    CountDownTimerText.text = countdown.ToString();
                }
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.1f);// When using global AI list its important so that both AI Maintaining spawner and wave spawner can spawn AI agents using trigger as there will be enough time for OnTriggerEnter to call its function.
            CountDownTimerText.text = "0";
            CountDownTimerText.gameObject.SetActive(false);
            WaveSpawnerScript.SpawnAgentsOnTrigger();
            if (DisplayWaveCounterText == true)
            {
                WaveText.gameObject.SetActive(true);
                WaveText.text = TextBeforeWaveNumber + " " + (WaveSpawnerScript.CurrentWaveRunning + 1);
                StartCoroutine(RemoveText());
            }
            else
            {
                BeginTimer = false;
                gameObject.SetActive(false);
            }
        }
        IEnumerator RemoveText()
        {
            yield return new WaitForSeconds(WaveCounterTextDeactivationTime);
            WaveText.text = "";
            BeginTimer = false;
            gameObject.SetActive(false);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == TriggeringGameObject)
            {
                if (BeginTimer == false)
                {
                    if (other.gameObject.GetComponent<Targets>() != null)
                    {
                        WaveSpawnerScript.TargetsScript = other.gameObject.GetComponent<Targets>();
                    }
                    StartWave();
                    BeginTimer = true;
                }
            }
        }
    }
}