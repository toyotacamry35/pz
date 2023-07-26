using System.Collections.Generic;
using JetBrains.Annotations;
using Assets.Src.RubiconAI.KnowledgeSystem.Memory;
using Assets.Src.RubiconAI.BehaviourTree;
using SharedCode.EntitySystem;
using SharedCode.MovementSync;
using System.Threading.Tasks;
using System.Linq;

namespace Assets.Src.RubiconAI.KnowledgeSystem
{
    public class Knowledge
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public Legionary Legionary { get; }
        public MetaExpression Meta { get; }
        public TimedMemory Memory  { get; }
        Strategy str;
        public Dictionary<KnowledgeCategoryDef, IKnowledgeSource> KnownStuff { get; } = new Dictionary<KnowledgeCategoryDef, IKnowledgeSource>();
        public Knowledge TranscendentKnowledge { get; private set; }
        // Ctor:
        public Knowledge(MobLegionary legionary)
        {
            Legionary = legionary;
            Memory  = new TimedMemory(legionary.Repository);

            // Hack to get correctly initialized MetaExpression (via no-need strategy - is the only(easiest) way):
            str = new Strategy(null);
            Meta = str.MetaExpression;
        }
        public async ValueTask Init() => await str.Init((MobLegionary)Legionary);

        // --- API: -------------------------------------------------------------------------

        public void Register([NotNull] IKnowledgeSource source) => Register(source.Category, source);
        public void Register(KnowledgeCategoryDef def, [NotNull]IKnowledgeSource source)
        {
            KnownStuff[def] = source;
        }
        public async ValueTask Update()
        {
            foreach (var source in KnownStuff.Values)
            {
                await source.UpdateKnowledge();
            }
        }
        public void Connect([NotNull]Knowledge knowledge)
        {
            knowledge.TranscendentKnowledge = this;
        }
        public void Disconnect([NotNull]Knowledge knowledge)
        {
            if (knowledge.TranscendentKnowledge == this)
                knowledge.TranscendentKnowledge = null;
        }

        [CanBeNull]
        public IKnowledgeSource GetKnownStuff([NotNull]KnowledgeCategoryDef category)
        {
            IKnowledgeSource source;
            if (KnownStuff.TryGetValue(category, out source))
                return source;
            return TranscendentKnowledge?.GetKnownStuff(category);
        }
    }
}
