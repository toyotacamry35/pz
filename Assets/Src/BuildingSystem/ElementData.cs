using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Building;
using SharedCode.DeltaObjects.Building;
using SharedCode.EntitySystem;
using System;
using UnityEngine;

namespace Assets.Src.BuildingSystem
{
    public abstract class ElementData : VisualData
    {
        public Vector3 PrefabPosition { get; set; } = Vector3.zero;
        public Quaternion PrefabRotation { get; set; } = Quaternion.identity;
        public Vector3 PrefabScale { get; set; } = Vector3.one;

        public ElementGameObject ElementGameObject { get; private set; } = null;
        public void CreateElementGameObject(GameObject gameObject, bool enable)
        {
            DestroyElementGameObject();
            ElementGameObject = new ElementGameObject(gameObject, enable);
        }
        public void DestroyElementGameObject()
        {
            if (ElementGameObject != null)
            {
                ElementGameObject.Destroy();
                ElementGameObject = null;
            }
        }

        [Bind]
        public BuildState State { get; protected set; } = BuildState.None;
        [Bind]
        public long BuildTimestamp { get; protected set; } = 0;
        [Bind]
        public float BuildTime { get; protected set; } = 0.0f;
        [Bind]
        public float Health { get; protected set; } = 0.0f;
        [Bind]
        public int Visual { get; protected set; } = -1;
        [Bind]
        public int Interaction { get; protected set; } = -1;
        [Bind]
        public int Depth { get; protected set; } = -1;

        public BuildRecipeDef BuildRecipeDef { get; set; }

        public Guid OwnerId { get; set; } = Guid.Empty;
        public Guid PlaceId { get; set; } = Guid.Empty;
        public Guid ElementId { get; set; } = Guid.Empty;

        public abstract bool IsDestroyed();
    }

    public abstract class ElementDataEx<ElementDeltaObjectType, ElementDeltaObjectTypeReplica> : ElementData
        where ElementDeltaObjectType : IDeltaObject
        where ElementDeltaObjectTypeReplica : ElementReplica
    {
        public void Bind(ElementDeltaObjectTypeReplica elementReplica)
        {
            if (elementReplica != null)
            {
                BuildRecipeDef = ExtractBuildRecipeDef(elementReplica);
                OwnerId = ExtractOwnerId(elementReplica);
                ElementId = ExtractElementId(elementReplica);
                SetInitialState(elementReplica);
            }
            InvokeBindFinished();
        }

        public void Unbind(ElementDeltaObjectTypeReplica elementReplica)
        {
            if (elementReplica != null)
            {
                SetInitialState(elementReplica);
            }
            ElementId = Guid.Empty;
            InvokeUnbindFinished();
        }

        protected override VisualBehaviour.VisualState GetVisualStateEx()
        {
            if (State == BuildState.Completed) { return VisualBehaviour.VisualState.Constructed; }
            else if (State == BuildState.InProgress) { return VisualBehaviour.VisualState.UnderConstruction; }
            else if (State == BuildState.Canceled) { return VisualBehaviour.VisualState.UnderConstruction; }
            else if (State == BuildState.Damaged) { return VisualBehaviour.VisualState.Constructed; }
            else if (State == BuildState.Destroyed) { return VisualBehaviour.VisualState.Destroyed; }
            else if (State == BuildState.Paused) { return VisualBehaviour.VisualState.UnderConstruction; }
            else { return VisualBehaviour.VisualState.Invisible; }
        }

        protected override string GetVisualCommonNameEx()
        {
            if (BuildRecipeDef != null)
            {
                return BuildRecipeDef.GetVisualCommonName(Visual);
            }
            else
            {
                return string.Empty;
            }
        }

        protected override string GetVisualVersionNameEx()
        {
            if (BuildRecipeDef != null)
            {
                return BuildRecipeDef.GetVisualVersionName(Visual);
            }
            else
            {
                return string.Empty;
            }
        }

        protected override float GetFracturedChunkScaleEx()
        {
            return SharedCode.Utils.BuildUtils.BuildParamsDef.FracturedChunkScale;
        }

        // abstract methods -----------------------------------------------------------------------
        public abstract BuildRecipeDef ExtractBuildRecipeDef(ElementDeltaObjectTypeReplica element);

        public abstract Guid ExtractOwnerId(ElementDeltaObjectTypeReplica element);

        public abstract Guid ExtractElementId(ElementDeltaObjectTypeReplica element);

        public abstract UnityRef<GameObject> ExtractPrefab();

        public abstract ResourceRef<UnityGameObjectDef> ExtractPrefabDef();
    }
}
