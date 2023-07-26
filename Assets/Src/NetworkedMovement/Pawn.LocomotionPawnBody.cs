using Assets.Src.NetworkedMovement.MoveActions;
using Src.Locomotion;
using UnityEngine;

namespace Assets.Src.NetworkedMovement
{
    public partial class Pawn
    {
        private class LocomotionPawnBody : LocomotionVirtualBody, IMoveActionPosition
        {
            private readonly Pawn _pawn;

            public LocomotionPawnBody(Pawn pawn)
            {
                _pawn = pawn;
            }

            bool IMoveActionPosition.Valid => _pawn;

            Vector3 IMoveActionPosition.Position => (Vector3) LocomotionHelpers.LocomotionToWorldVector(base.Position);

            bool IMoveActionPosition.IsSameObject(IMoveActionPosition other) => _pawn && other.IsSameObject(_pawn.gameObject);

            public bool IsSameObject(GameObject other) => _pawn && _pawn.gameObject == other;
        }
    }
}