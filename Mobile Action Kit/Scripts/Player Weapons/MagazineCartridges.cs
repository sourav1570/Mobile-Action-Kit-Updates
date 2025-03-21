using System.Collections;
using UnityEngine;

namespace MobileActionKit
{
	public class MagazineCartridges : MonoBehaviour
	{

		public static MagazineCartridges ins;

		public GameObject BulletsMag;
		public float TimeToActivateMag = 2f;


		void Start()
		{
			ins = this;
		}
		// Use this for initialization
		public void ActivateForReload()
		{
			StartCoroutine(Deactive());
		}
		IEnumerator Deactive()
		{
			yield return new WaitForSeconds(0f);
			BulletsMag.SetActive(false);
			yield return new WaitForSeconds(TimeToActivateMag);
			BulletsMag.SetActive(true);
		}
	}
}