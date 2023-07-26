using Assets.Src.GameObjectAssembler;
using Assets.Src.SpawnSystem;
using SharedCode.EntitySystem;
using System;
using System.Linq;
using Core.Environment.Logging.Extension;
using NLog;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Utils;
using UnityEngine;

namespace Assets.Src.Character.Events
{
    public class VisualEventProxy : EntityGameObjectComponent, IFromDef<VisualEventProxyDef>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private GameObject _root;
        private FXEvents _fxEvents;
        private Animator _animator;

        public VisualEventProxyDef Def { get; set; }
        public OuterRef<IEntity> Entity { get; private set; } = OuterRef<IEntity>.Invalid;
        public IEntitiesRepository Repository { get; private set; } = default(IEntitiesRepository);

        public void SetRoot(GameObject root)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _animator = _root.GetComponentInChildren<FXAnimatorReference>()?.Animator;
        }
        
        protected override void GotClient()
        {
            Entity = new OuterRef<IEntity>() { Guid = EntityId, TypeId = TypeId };
            Repository = ClientRepo;
            if (!_root) SetRoot(gameObject);
        }

        protected override void LostClient()
        {
            Entity = OuterRef<IEntity>.Invalid;
            Repository = default(IEntitiesRepository);
        }
        
        public void PostEvent(VisualEvent evt)
        {
            UnityQueueHelper.AssertInUnityThread();

            if (!Constants.WorldConstants.EnableFX)
                return;
            if (!Entity.IsValid)
                return;
            if (Def?.Events == null)
                return;
            if (Repository == null)
                return;
            
            if (_fxEvents == null)
                _fxEvents = new FXEvents(Def.Events.Target);

            evt.casterGameObject = _root;
            evt.casterEntityRef = Entity;
            evt.casterRepository = Repository;
            if (Logger.IsDebugEnabled) Logger.IfDebug()?.Message(evt.ToStringLong()).Write();
            _fxEvents.Update(evt, _animator);
        }
    }
}
