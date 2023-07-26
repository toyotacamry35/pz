using Assets.Src.Aspects.Impl.Factions.Template;
using GeneratorAnnotations;
using SharedCode.DeltaObjects;
using SharedCode.Entities.Engine;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;

namespace GeneratedCode.DeltaObjects
{
    public partial class QuestCounter
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Task OnInitImpl(QuestDef questDef, QuestCounterDef counterDef, IEntitiesRepository repository)
        {
            //Logger.Error($"QuestCounter({GetType().NiceName()}:{LocalId}).OnInitImpl({counterDef})");
            return Task.CompletedTask;
        }

        public Task OnDatabaseLoadImpl(IEntitiesRepository repository)
        {
            return Task.CompletedTask;
        }

        public Task OnDestroyImpl(IEntitiesRepository repository)
        {
            return Task.CompletedTask;
        }
        public Task PreventOnCompleteEventImpl()
        {
            return Task.CompletedTask;
        }
    }
}
