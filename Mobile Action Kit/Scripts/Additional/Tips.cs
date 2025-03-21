using UnityEngine.UI;
using UnityEngine;
using TMPro;

// This Script is Responsible For Tips To Give While Loading The Scene
// This script handles the displaying of tips when loading another scene.

namespace MobileActionKit
{
    public class Tips : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles the displaying of tips when loading another scene.";

        [Tooltip("Reference to the Hint gameObject from the hierarchy.")]
        public GameObject HintGameObject;

        [Tooltip("Reference to the TextMeshProUGUI component from the hierarchy.")]
        public TextMeshProUGUI HintText;

        [Tooltip("Enter the hints to display randomly upon loading another scene.")]
        public string[] Hints;

        private void OnEnable()
        {
            ShowHints();
        }

        public void ShowHints()
        {
            if(HintGameObject != null && HintText != null)
            {
                HintGameObject.SetActive(true);
                int randomisetips = Random.Range(0, Hints.Length);
                HintText.text = Hints[randomisetips];
            }
         
        }
    }
}
