using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using SharedCode.Repositories;
using SharedCode.Utils;

namespace Assets.Src.RubiconAI.KnowledgeSystem
{
    class StaticsKnowledgeSource : IKnowledgeSource
    {
        private readonly List<(Guid, ScenicEntityDef)> _objects;

        public StaticsKnowledgeSource(KnowledgeCategoryDef category, List<(Guid, ScenicEntityDef)> objects)
        {
            _objects = objects;
            Category = category;
            _legionaries = new Dictionary<OuterRef<IEntity>, VisibilityDataSample>();
            foreach (var gameObject in _objects)
            {
                var oref = new OuterRef<IEntity>(gameObject.Item1,
                   ReplicaTypeRegistry.GetIdByType(DefToType.GetEntityType(gameObject.Item2.Object.Target.GetType())));

                var vds = new VisibilityDataSample() { Ref = oref, Def = gameObject.Item2.Object.Target, Pos = gameObject.Item2.Position };
                if (_legionaries.ContainsKey(vds.Ref))
                    continue;
                _legionaries.Add(vds.Ref, vds);
            }
        }

        public KnowledgeCategoryDef Category { get; }
        private Dictionary<OuterRef<IEntity>, VisibilityDataSample> _legionaries;
        public Dictionary<OuterRef<IEntity>, VisibilityDataSample> Legionaries
        {
            get
            {
                return _legionaries;
            }
        }


        public KnowledgeSourceDef Def { get; set; }

        public event Func<OuterRef<IEntity>, ValueTask> OnLearnedAboutLegionary;
        public event Func<OuterRef<IEntity>, ValueTask> OnForgotAboutLegionary;

        public async ValueTask LoadDef(Knowledge knowledge, KnowledgeSourceDef def)
        {
        }

        public ValueTask UpdateKnowledge()
        {
            return default;
        }

        public BaseResource GetId()
        {
            return Category;
        }
    }
}
