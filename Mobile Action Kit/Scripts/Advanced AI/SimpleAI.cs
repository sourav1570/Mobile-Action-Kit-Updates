using UnityEngine.AI;
using UnityEngine;

namespace MobileActionKit
{
    public class SimpleAI : MonoBehaviour
    {

        public float speed;
        public NavMeshAgent Agent;
        public Transform Destination;

        private void Update()
        {
            Agent.speed = speed;
            Agent.destination = Destination.position;
        }


    }
}