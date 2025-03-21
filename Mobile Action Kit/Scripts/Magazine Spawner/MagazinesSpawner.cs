using UnityEngine;

namespace MobileActionKit
{
    public class MagazinesSpawner : MonoBehaviour
    {
        [Tooltip("How Much Bullets To give When Enemy Died. bullets can be Taken By Player or Any Other Enemy")]
        public int MagazineSize = 30;

        private PlayerWeapon gunscript;

        WeaponId id;

        private void Start()
        {
            id = GetComponent<WeaponId>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (PlayerManager.instance != null)
                {
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.TotalAmmo += MagazineSize;
                    PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.TotalAmmoText.text = PlayerManager.instance.CurrentHoldingPlayerWeapon.Reload.TotalAmmo.ToString();
                    gameObject.SetActive(false);
                }
            }
            else if (other.transform.root.tag == "AI")
            {
                //if ((other.transform.root.gameObject.GetComponent<MasterAiBehaviour>() != null && other.transform.root.gameObject.GetComponent<MasterAiBehaviour>().IsDead == false))
                //{
                //    if (other.transform.root.gameObject.GetComponent<MasterAiBehaviour>() != null)
                //    {
                //        other.transform.root.gameObject.GetComponent<MasterAiBehaviour>().HumanoidFiringBehaviourComponent.Reload.MaxAmmo += MagazineSize;
                //        gameObject.SetActive(false);
                //    }
                //}
                if ((other.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null && other.transform.root.gameObject.GetComponent<HumanoidAiHealth>().IsDied == false))
                {
                    if (other.transform.root.gameObject.GetComponent<CoreAiBehaviour>() != null)
                    {
                        other.transform.root.gameObject.GetComponent<CoreAiBehaviour>().Components.HumanoidFiringBehaviourComponent.Reload.MaxAmmo += MagazineSize;
                        gameObject.SetActive(false);
                    }
                }
                //else if (other.transform.root.gameObject.GetComponent<DroneAiBehaviour>() != null)
                //{
                //    if (other.transform.root.gameObject.GetComponent<DroneAiBehaviour>() != null)
                //    {
                //        for (int i = 0; i < other.transform.root.gameObject.GetComponent<DroneAiBehaviour>().FiringScripts.Length; i++)
                //        {
                //            other.transform.root.gameObject.GetComponent<DroneAiBehaviour>().FiringScripts[i].MagazineSize += MagazineSize;
                //        }
                //        gameObject.SetActive(false);
                //    }
                //}
            }

        }
    }
}