using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MobileActionKit
{
    public class SceneSwitcher : MonoBehaviour
    {
        public void SwitchScene(int LevelIndex)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(LevelIndex);
        }
    }
}