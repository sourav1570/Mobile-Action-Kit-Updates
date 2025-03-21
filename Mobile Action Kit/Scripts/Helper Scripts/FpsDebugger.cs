using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class FpsDebugger : MonoBehaviour
    {

        public bool ShouldDebug = true;
        public Text TextToShowFramesPerSecond;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        void Update()
        {
            if (ShouldDebug == true)
            {
                float fps = 1.0f / Time.smoothDeltaTime;
                TextToShowFramesPerSecond.text = "FPS" + " " + Mathf.FloorToInt(fps).ToString();
            }
        }
    }
}
