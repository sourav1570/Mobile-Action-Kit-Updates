using UnityEngine.UI;
using UnityEngine;

namespace MobileActionKit
{
    public class HealthBarScript : MonoBehaviour
    {
        [TextArea]
        [ContextMenuItem("Reset Description", "ResettingDescription")]
        public string ScriptInfo = "This Script Reduces The Player UI Health Bar in game";
        [Space(10)]

        private float fillamount = 1;
        [SerializeField]
        private float HealthBarSpeed;
        [SerializeField]
        private Image HealthBarImage;

        public float Maxvalue { get; set; }

        public void ResettingDescription()
        {
            ScriptInfo = "This Script Reduces The Player UI Health Bar in game";
        }
        public float Value
        {
            set
            {
                fillamount = Map(value, 0, Maxvalue, 0, 1);
            }
        }
        void Update()
        {
            Handleabar();
        }
        private void Handleabar()
        {
            if (fillamount != HealthBarImage.fillAmount)
            {
                HealthBarImage.fillAmount = Mathf.Lerp(HealthBarImage.fillAmount, fillamount, Time.deltaTime * HealthBarSpeed);
            }
        }
        private float Map(float value, float inmin, float inmax, float outmin, float outmax)
        {
            return (value - inmin) * (outmax - outmin) / (inmax - inmin) + outmin;
        }
    }
}
