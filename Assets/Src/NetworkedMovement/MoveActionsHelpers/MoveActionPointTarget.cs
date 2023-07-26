using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public class MoveActionPointTarget : IMoveActionPosition
    {
        public MoveActionPointTarget(Vector3 point) 
        {
            Position = point;
            Valid = point != Vector3.positiveInfinity && point != Vector3.negativeInfinity;
        }

        public bool Valid { get; }
        
        public Vector3 Position { get; }
        
        public bool IsSameObject(IMoveActionPosition other) => false;
        
        public bool IsSameObject(GameObject other) => false;

        public override string ToString() => Position.ToString("F1");
    }
}