using UnityEngine.AI;
using UnityEngine;

namespace MobileActionKit
{
    public class GenerateRandomNavmeshLocation : MonoBehaviour
    {

        public static Vector3 RandomLocation(Transform direction, float radius)
        {
            //Vector3 randomDirection = Random.insideUnitSphere * radius;
            //randomDirection += direction.position;
            //NavMeshHit hit;
            //Vector3 finalPosition = randomDirection;//new Vector3(83f, -24f, -21f);//= Vector3.zero;
            //if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            //{
            //    finalPosition = hit.position;
            //}
            //return finalPosition;


            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += direction.position;

            NavMeshHit hit;
            Vector3 finalPosition = direction.position;

            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
            }

            return finalPosition;
        }
        public static Vector3 RandomLocationInVector3(Vector3 direction, float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += direction;

            NavMeshHit hit;
            Vector3 finalPosition = direction;

            if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
            }

            return finalPosition;
        }
    }
}