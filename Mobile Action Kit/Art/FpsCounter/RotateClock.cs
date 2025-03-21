using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This Script is Responsible For Different Settings in Game
namespace MobileActionKit
{

	public class RotateClock : MonoBehaviour
	{
		void Update()
		{
			transform.Rotate(0, 0, -6);
		}
	}
}