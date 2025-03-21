using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
	public class Unparenting : MonoBehaviour
	{

		GameObject[] AllChilds;

		void Awake()
		{
			AllChilds = new GameObject[transform.childCount];
		}
		void Start()
		{
			for (int x = 0; x < transform.childCount; x++)
			{
				AllChilds[x] = transform.GetChild(x).gameObject;
			}
			for (int x = 0; x < AllChilds.Length; x++)
			{
				AllChilds[x].transform.parent = null;
			}

		}

	}
}
