using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using UnityEngine;

namespace Assets.Src.NetworkedMovement.MoveActions
{
    public class MoveActionTransformTarget : IMoveActionPosition
    {
        private Transform _transform => EDef.Target.Target.GetGo(CastData)?.transform;
        Vector3 _cachedPos = default;
        private Vector3 _pos => _transform == null ? _cachedPos == default ? Self.position : _cachedPos : _cachedPos = _transform.position;
        public MoveActionTransformTarget(MoveEffectDef eDef, SpellPredCastData castData, Transform self)
        {
            EDef = eDef;
            CastData = castData;
            Self = self;
        }

        public bool Valid => _transform;
        
        public Vector3 Position => _pos;

        public MoveEffectDef EDef { get; }
        public SpellPredCastData CastData { get; }
        public Transform Self { get; }

        public bool IsSameObject(IMoveActionPosition other) => _transform && other.IsSameObject(_transform.gameObject);

        public bool IsSameObject(GameObject other) => _transform && _transform.gameObject == other;

        public override string ToString() => _transform ? _transform.ToString() : "<null>";
    }
}