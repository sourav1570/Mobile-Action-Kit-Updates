using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
	public class DeactivateOnPlay : MonoBehaviour
	{

		public MeshRenderer[] AddMeshRenderers;
		public SkinnedMeshRenderer[] AddSkinMeshRenderers;
		public GameObject[] AddGameObjects;

		void Start()
		{
			for (int x = 0; x < AddMeshRenderers.Length; x++)
			{
				AddMeshRenderers[x].enabled = false;
			}
			for (int x = 0; x < AddSkinMeshRenderers.Length; x++)
			{
				AddSkinMeshRenderers[x].enabled = false;
			}
			for (int x = 0; x < AddGameObjects.Length; x++)
			{
				AddGameObjects[x].SetActive(false);
			}
		}

	}
}
