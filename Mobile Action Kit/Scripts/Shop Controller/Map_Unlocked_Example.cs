using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MobileActionKit
{
    public class Map_Unlocked_Example : MonoBehaviour
    {
        public Button BuyButton;
        public Button Map_PurchaseButton;

        public void ActivateMap()
        {
            BuyButton.gameObject.SetActive(false);
            Map_PurchaseButton.interactable = true;
        }
    }
}