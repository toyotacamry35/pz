using System.Collections.Generic;
using System.Linq;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem;
using Assets.Src.SpawnSystem;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using JetBrains.Annotations;
using ReactivePropsNs;
using SharedCode.Aspects.Item.Templates;
using SharedCode.Aspects.Science;
using UnityEngine;

namespace Uins
{
    public delegate void TechPointsCountChangedDelegate(CurrencyResource techPointDef, int count, bool isInitial);

    public delegate void SciencePointsCountChangedDelegate(ScienceDef scienceDef, int count, bool isInitial);

    public class PlayerPointsSource : BindingViewModel, ICharacterPoints
    {
        private const float SilenceAfterPawnChanged = 2;

        public event TechPointsCountChangedDelegate TechPointsCountChanged;
        public event SciencePointsCountChangedDelegate SciencePointsCountChanged;

        [SerializeField, UsedImplicitly]
        private PlayerPointsRef _playerPointsRef;

        [SerializeField, UsedImplicitly]
        private ExtraAreaSciences _extraAreaSciences;

        [SerializeField, UsedImplicitly]
        private ExtraAreaTechPoints _extraAreaTechPoints;

        [SerializeField, UsedImplicitly]
        private TechPointsChangedMessage _techPointsChangedMessage;

        private static PlayerPointsSource _instance;

        private MutationStageDef _stage;
        private float _lastPawnChangingTime;
        private List<KnowledgeDef> _knownKnowledges = new List<KnowledgeDef>();
        private List<TechnologyDef> _knownTechnologies = new List<TechnologyDef>();

        private EntityApiWrapper<KnowledgeEngineFullApi> _knowledgeEngineFullApiWrapper;
        private EntityApiWrapper<HasFactionFullApi> _hasFactionFullApiWrapper;
 

        //=== Props ===========================================================

        public IStream<DictionaryStream<CurrencyResource, int>> TechPointsChangesStream { get; private set; }

        public IStream<DictionaryStream<ScienceDef, int>> SciencesChangesStream { get; private set; }

        public DictionaryStream<CurrencyResource, int> CurrenciesStream { get; } = new DictionaryStream<CurrencyResource, int>();

        public DictionaryStream<ScienceDef, int> SciencesStream { get; } = new DictionaryStream<ScienceDef, int>();

        private PlayerPoints _startPlayerPoints;

        public PlayerPoints StartPlayerPoints => _playerPointsRef?.Target;

        private ListStream<ScienceDef> _availableSciences;

        public ListStream<ScienceDef> AvailableSciences
        {
            get
            {
                if (_availableSciences == null)
                {
                    _availableSciences = new ListStream<ScienceDef>();
                    StartPlayerPoints.UsedSciences.ForEach(sRef =>
                    {
                        if (sRef.IsValid && !_availableSciences.Contains(sRef.Target))
                            _availableSciences.Add(sRef.Target);
                        else
                            UI.Logger.IfError()?.Message($"Empty or double science in {nameof(StartPlayerPoints.UsedSciences)}").Write();
                    });
                }

                return _availableSciences;
            }
        }

        private ListStream<CurrencyResource> _availableCurrencies;

        public ListStream<CurrencyResource> AvailableCurrencies
        {
            get
            {
                if (_availableCurrencies == null)
                {
                    _availableCurrencies = new ListStream<CurrencyResource>();
                    StartPlayerPoints.UsedTechPoints.ForEach(sRef =>
                    {
                        if (sRef.IsValid && !_availableCurrencies.Contains(sRef.Target))
                            _availableCurrencies.Add(sRef.Target);
                        else
                            UI.Logger.IfError()?.Message($"Empty or double currence in {nameof(StartPlayerPoints.UsedTechPoints)}").Write();
                    });
                }

                return _availableCurrencies;
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            _instance = SingletonOps.TrySetInstance(this, _instance);
            _playerPointsRef.Target.AssertIfNull(nameof(_playerPointsRef));
            _extraAreaSciences.AssertIfNull(nameof(_extraAreaSciences));
            _extraAreaTechPoints.AssertIfNull(nameof(_extraAreaTechPoints));
            _techPointsChangedMessage.AssertIfNull(nameof(_techPointsChangedMessage));
            StartPlayerPoints.AssertIfNull(nameof(StartPlayerPoints));

            TechPointsChangesStream = CurrenciesStream.DictionaryChanges(D);
            SciencesChangesStream = SciencesStream.DictionaryChanges(D);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _instance = null;
        }

        //=== Public ==========================================================

        public int GetTechPointsCount(CurrencyResource techPoint)
        {
            return CurrenciesStream.FirstOrDefault(kvp => kvp.Key == techPoint).Value;
        }

        public int GetSciencesCount(ScienceDef scienceDef)
        {
            return SciencesStream.FirstOrDefault(kvp => kvp.Key == scienceDef).Value;
        }

        public List<CurrencyResource> GetAvailableTechPoints()
        {
            return AvailableCurrencies.ToList();
        }

        public List<ScienceDef> GetAvailableSciences()
        {
            return AvailableSciences.ToList();
        }

        public void Init(IPawnSource pawnSource, IGuiWindow inventoryWindow)
        {
            ResetScienceCounts();
            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
            _extraAreaSciences.Init(this, inventoryWindow);
            _extraAreaTechPoints.Init(this);
            _techPointsChangedMessage.Init(this, inventoryWindow);
        }


        //=== Private =========================================================

        private void ResetScienceCounts()
        {
            var availableSciences = GetAvailableSciences();
            foreach (var scienceDef in availableSciences)
                SciencesStream[scienceDef] = 0;
        }

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            _lastPawnChangingTime = Time.time;
            if (prevEgo != null)
            {
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromRPoints(OnRPointCountChanged);
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromKnowledge(OnKnowledgeAddedRemoved);
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromTechnologies(OnTechnologyAddedRemoved);
                _hasFactionFullApiWrapper.EntityApi.UnsubscribeFromStage(OnStageChanged);
                _stage = null;
                _knownKnowledges.Clear();
                _knownTechnologies.Clear();
                ResetScienceCounts();

                _knowledgeEngineFullApiWrapper.Dispose();
                _knowledgeEngineFullApiWrapper = null;
                _hasFactionFullApiWrapper.Dispose();
                _hasFactionFullApiWrapper = null;
                UpdateScienceCounts();
            }

            if (newEgo != null)
            {
                _knowledgeEngineFullApiWrapper = EntityApi.GetWrapper<KnowledgeEngineFullApi>(newEgo.OuterRef);
                _hasFactionFullApiWrapper = EntityApi.GetWrapper<HasFactionFullApi>(newEgo.OuterRef);

                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToRPoints(OnRPointCountChanged);
                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToKnowledge(OnKnowledgeAddedRemoved);
                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToTechnologies(OnTechnologyAddedRemoved);
                _hasFactionFullApiWrapper.EntityApi.SubscribeToStage(OnStageChanged);
                UpdateScienceCounts();
            }
        }

        private void OnRPointCountChanged(CurrencyResource techPointDef, int count, bool isInitial)
        {
            if (techPointDef.AssertIfNull(nameof(techPointDef)))
                return;

            CurrenciesStream[techPointDef] = count;
            //var isInitial = Time.time - _lastPawnChangingTime < SilenceAfterPawnChanged;
            TechPointsCountChanged?.Invoke(techPointDef, count, isInitial);
        }

        private void OnTechnologyAddedRemoved(TechnologyDef technologyDef, bool isRemoved, bool isInitial)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return;

            if (isRemoved)
                _knownTechnologies.Remove(technologyDef);
            else
                _knownTechnologies.Add(technologyDef);

            if (IsTechnologyAffectsSciences(technologyDef))
                UpdateScienceCounts();
        }

        private void OnKnowledgeAddedRemoved(KnowledgeDef knowledgeDef, bool isRemoved, bool isInitial)
        {
            if (knowledgeDef.AssertIfNull(nameof(knowledgeDef)))
                return;

            if (isRemoved)
                _knownKnowledges.Remove(knowledgeDef);
            else
                _knownKnowledges.Add(knowledgeDef);

            if (IsKnowledgeAffectsSciences(knowledgeDef))
                UpdateScienceCounts();
        }

        private void OnStageChanged(MutationStageDef stage)
        {
            if (stage == _stage)
                return;

            _stage = stage;
            UpdateScienceCounts();
        }

        public List<KnowledgeDef> GetKnownAvailableKnowledges()
        {
            return KnowledgeLogic.GetKnownKnowledges(_knownTechnologies, _knownKnowledges, _stage, false);
        }

        private void UpdateScienceCounts()
        {
            var knownAvailableKnowledges = GetKnownAvailableKnowledges();
            var sciences = SciencesStream.Keys.ToList();
            foreach (var scienceDef in sciences)
            {
                var scienceCount = KnowledgeLogic.GetScienceCount(scienceDef, knownAvailableKnowledges);
                if (scienceCount == SciencesStream[scienceDef])
                    continue;

                SciencesStream[scienceDef] = scienceCount;
                var isFirstTime = Time.time - _lastPawnChangingTime < SilenceAfterPawnChanged;
                SciencePointsCountChanged?.Invoke(scienceDef, scienceCount, isFirstTime);
                //UI.Logger.IfInfo()?.Message($"SciencePointsCountChanged({scienceDef.____GetDebugShortName()} = {scienceCount}, iFT{isFirstTime})").Write(); //DEBUG
            }
        }

        private bool IsTechnologyAffectsSciences(TechnologyDef technologyDef)
        {
            return !technologyDef.AssertIfNull(nameof(technologyDef)) &&
                   technologyDef.ActivateConditions.RewardKnowledges != null &&
                   technologyDef.ActivateConditions.RewardKnowledges.Any(k => IsKnowledgeAffectsSciences(k.Target));
        }

        private bool IsKnowledgeAffectsSciences(KnowledgeDef knowledgeDef)
        {
            return !knowledgeDef.AssertIfNull(nameof(knowledgeDef)) &&
                   knowledgeDef.Sciences != null &&
                   knowledgeDef.Sciences.Length > 0;
        }
    }
}