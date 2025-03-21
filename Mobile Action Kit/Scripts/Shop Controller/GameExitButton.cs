using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MobileActionKit
{
    public class GameExitButton : MonoBehaviour
    {
        [TextArea]
        public string ScriptInfo = "This script handles the function when player exit the game.";

        [Tooltip("Drag and drop the UI button for this item from the hierarchy into this field.")]
        public Button ExitButton;

        private void Start()
        {
            ExitButton.onClick.AddListener(ExitTheGame);
        }
        public void ExitTheGame()
        {
            Application.Quit();
        }
    }
}