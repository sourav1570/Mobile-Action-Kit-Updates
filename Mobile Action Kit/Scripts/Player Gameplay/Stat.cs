using System;
using UnityEngine;

namespace MobileActionKit
{
    [Serializable]
    public class Stat
    {

        [SerializeField]
        private HealthBarScript PlayerHealthBarScript;
        [SerializeField]
        private float MaxHealth;
        [SerializeField]
        [ReadOnly]
        private float CurrentHealth;

        public float Curvalue
        {
            get
            {
                return CurrentHealth;
            }

            set
            {
                this.CurrentHealth = Mathf.Clamp(value, 0, Maxvalue);
                if (PlayerHealthBarScript != null)
                    PlayerHealthBarScript.Value = CurrentHealth;
            }
        }

        public float Maxvalue
        {
            get
            {
                return MaxHealth;
            }

            set
            {
                this.MaxHealth = value;
                if (PlayerHealthBarScript != null)
                    PlayerHealthBarScript.Maxvalue = MaxHealth;
            }
        }
        public void Initialize()
        {
            this.Maxvalue = MaxHealth;
            this.Curvalue = MaxHealth;
        }
    }
}