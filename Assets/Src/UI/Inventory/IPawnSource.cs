using Assets.Src.SpawnSystem;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ReactivePropsNs;
using ReactivePropsNs.Touchables;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public interface IPawnSource
    {
        GameObject OurUserPawn { get; }

        /// <summary>
        /// (PrevEgo, NewEgo)
        /// </summary>
        IStream<(EntityGameObject, EntityGameObject)> PawnChangesStream { get; }

        IStream<ListStream<TechnologyDef>> KnownTechnologiesStream { get; }

        ITouchable<IWorldCharacterClientFull> TouchableEntityProxy { get; }

        IStream<int> AccountLevelStream { get; }
    }
}