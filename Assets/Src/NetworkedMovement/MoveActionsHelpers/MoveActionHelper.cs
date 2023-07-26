using Src.Locomotion;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public static class MoveActionHelper
    {
        public static Vector3 InvalidVector = Vector3.positiveInfinity;
        
        public static bool FindReachablePoint(Vector3 point, NavMeshAgent agent, float searchRadius, out Vector3 reachablePoint)
        {
            NavMeshHit tpHit;
            if (NavMesh.SamplePosition(point, out tpHit, searchRadius, new NavMeshQueryFilter {areaMask = 1, agentTypeID = agent.agentTypeID})) ///#PZ-13568: #??(to Andrey): diff. settings for this call (see all "NavMesh.SamplePosition(" in entire solution)) Agent of mob has all area flags on: Walkable/NotWalkable/Jump
            {
                reachablePoint = tpHit.position;
                return true;
            }
            reachablePoint = point;
            return false;
        }
        
        public static bool CheckDestinationReached(Vector3 currentPosition, Vector3 destinationPoint, Vector3 reachablePoint, float acceptedRange)
        {
//            var sqrDistanceToDestination = (currentPosition - destinationPoint).sqrMagnitude;
//            var sqrReachablePointDifference = (destinationPoint - reachablePoint).sqrMagnitude;
//            return sqrDistanceToDestination <= Mathf.Max(acceptedRange.Sqr(), sqrReachablePointDifference);
            
            var sqrDistanceToDestination = (currentPosition - destinationPoint).sqrMagnitude;
            if (sqrDistanceToDestination <= acceptedRange.Sqr())
                return true;

            if (reachablePoint != InvalidVector)
            {
                var sqrDistanceToReachable = (currentPosition - reachablePoint).sqrMagnitude;
                if (sqrDistanceToReachable <= 0.01)
                    return true;
            }

            return false;
        }
    }
}