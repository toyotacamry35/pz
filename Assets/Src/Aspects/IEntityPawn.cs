
using Assets.Src.SpawnSystem;
using ReactivePropsNs;
using ResourceSystem.Utils;
using SharedCode.EntitySystem;

namespace Assets.Src.Aspects
{
    public interface IEntityPawn
    {
        /// <summary>
        /// 
        /// </summary>
        OuterRef EntityRef { get; }
        /// <summary>
        /// Клиентский репозиторий
        /// </summary>
        IEntitiesRepository Repository { get; }
        /// <summary>
        /// Оставлено для UI
        /// </summary>
        EntityGameObject Ego { get; }
        /// <summary>
        /// 
        /// </summary>
        IReactiveProperty<IEntityView> View { get; }
    }
}