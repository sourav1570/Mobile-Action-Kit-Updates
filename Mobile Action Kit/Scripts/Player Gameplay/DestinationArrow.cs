using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MobileActionKit
{
	public class DestinationArrow : MonoBehaviour
	{
		[TextArea]
		public string ScriptInfo = "This script saves and restores the player's position and rotation using PlayerPrefs when they enter a checkpoint.";
		[Space(10)]

		[Tooltip("List of waypoints the arrow will point towards.")]
		public Transform[] PlayerWaypoints;
		[Tooltip("Speed at which the arrow rotates towards the next waypoint.")]
		public float RotationSpeed;
		int MyPoint;

		[Tooltip("Minimum distance to the waypoint before updating to the next one.")]
		public float MinWaypointSwitchDistance = 3f;

		[Tooltip("Maximum distance to the waypoint before updating to the next one.")]
		public float MaxWaypointSwitchDistance = 6f;

		float MyDistance;

		void Start()
		{
			if(PlayerPrefs.GetInt("PlayerHasReachedAllCheckpoints") == 0)
            {
				MyDistance = Random.Range(MinWaypointSwitchDistance, MaxWaypointSwitchDistance);
			}
            else
            {
				gameObject.SetActive(false);
			}
			
		}
		void Update()
		{
			if (PlayerPrefs.GetInt("PlayerHasReachedAllCheckpoints") == 0)
			{
				if (MyPoint < PlayerWaypoints.Length)
				{
					var lookPos = PlayerWaypoints[MyPoint].position - transform.position;
					lookPos.y = 0;

					if (lookPos.magnitude < MyDistance)
					{
						GetPoint();
						if (MyPoint >= PlayerWaypoints.Length)
						{
							gameObject.SetActive(false);
							PlayerPrefs.SetInt("PlayerHasReachedAllCheckpoints", 1);
						}
					}
					else
					{
						var rotation = Quaternion.LookRotation(lookPos);
						transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);
					}
				}
			}
		}
		public void GetPoint()
		{
			++MyPoint;

		}
	}
}
