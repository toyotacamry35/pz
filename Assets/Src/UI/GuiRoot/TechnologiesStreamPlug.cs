using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using ReactivePropsNs;
using SharedCode.Aspects.Science;

namespace Uins
{
    public class TechnologiesStreamPlug : BindingVmodel
    {
        private ListStream<TechnologyDef> _knownTechnologiesStream = new ListStream<TechnologyDef>();
        private EntityApiWrapper<KnowledgeEngineFullApi> _knowledgeEngineFullApiWrapper;

        public IStream<ListStream<TechnologyDef>> GetKnownTechnologiesStream(IStream<(EntityGameObject, EntityGameObject)> pawnStream)
        {
            pawnStream.Subscribe(D,
                (prevEgo, newEgo) =>
                {
                    if (prevEgo != null)
                    {
                        _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromTechnologies(OnTechnologyAddedOrRemoved);
                        _knownTechnologiesStream.Clear();
                        _knowledgeEngineFullApiWrapper.Dispose();
                        _knowledgeEngineFullApiWrapper = null;
                    }

                    if (newEgo != null)
                    {
                        _knowledgeEngineFullApiWrapper = EntityApi.GetWrapper<KnowledgeEngineFullApi>(newEgo.OuterRef);
                        _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToTechnologies(OnTechnologyAddedOrRemoved);
                    }
                },
                () => _knownTechnologiesStream.Clear());

            return _knownTechnologiesStream.ListChanges(D);
        }

        private void OnTechnologyAddedOrRemoved(TechnologyDef technologyDef, bool isRemoved, bool isInitial)
        {
            if (isRemoved)
            {
                if (_knownTechnologiesStream.Contains(technologyDef))
                {
                    _knownTechnologiesStream.Remove(technologyDef);
                }
            }
            else
            {
                if (!_knownTechnologiesStream.Contains(technologyDef))
                {
                    _knownTechnologiesStream.Add(technologyDef);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _knownTechnologiesStream.Dispose();
        }
    }
}