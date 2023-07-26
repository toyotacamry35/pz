using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects;
using Assets.Src.Aspects.Impl.Factions.Template;
using Assets.Src.ContainerApis;
using Assets.Src.SpawnSystem;
using Core.Cheats;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ReactivePropsNs;
using SharedCode.Aspects.Science;

namespace Uins.Inventory
{
    public delegate void KnownRecipeAddDelegate(BaseRecipeDef baseRecipeDef, RecipeState recipeState, bool isFirstTime);

    public delegate void KnownRecipeRemoveDelegate(BaseRecipeDef baseRecipeDef);

    public delegate void KnownRecipeStateChangedDelegate(BaseRecipeDef baseRecipeDef, RecipeState recipeState);

    public class RecipesSource : IDisposable
    {
        public event KnownRecipeAddDelegate KnownRecipeAdd;
        public event KnownRecipeRemoveDelegate KnownRecipeRemove;
        public event KnownRecipeStateChangedDelegate KnownRecipeStateChanged;

        private static RecipesSource _instance;
        private DisposableComposite D = new DisposableComposite();

        private EntityApiWrapper<HasFactionFullApi> _hasFactionFullApiWrapper;
        private EntityApiWrapper<KnowledgeEngineFullApi> _knowledgeEngineFullApiWrapper;


        //=== Props ===========================================================

        public MutationStageDef Stage { get; private set; }

        public Dictionary<BaseRecipeDef, RecipeState> KnownRecipes { get; } = new Dictionary<BaseRecipeDef, RecipeState>();

        public List<KnowledgeDef> KnownKnowledges { get; } = new List<KnowledgeDef>();

        public List<TechnologyDef> KnownTechnologies { get; } = new List<TechnologyDef>();


        //=== Ctor ============================================================

        public RecipesSource(IPawnSource pawnSource)
        {
            if (pawnSource.AssertIfNull(nameof(pawnSource)))
                return;

            pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
            _instance = SingletonOps.TrySetInstance(this, _instance);
        }

        public void Dispose()
        {
            if (_instance == this)
                _instance = null;

            D.Dispose();
        }


        //=== Public ==========================================================

        [Cheat]
        public static void Recipes()
        {
            if (_instance.AssertIfNull(nameof(_instance)))
            {
                UI.Logger.IfError()?.Message($"{nameof(Recipes)}() Unable to show").Write();
                return;
            }

            UI.LoggerDefault.IfInfo()?.Message(_instance.KnownRecipes
                .Select(kr => $"{kr.Key.____GetDebugRootName()}{(kr.Value.IsAvailable ? "" : " BLOCKED")}")
                .ItemsToStringByLines())
                .Write();
        }

        [Cheat]
        public static void Knowledges()
        {
            if (_instance.AssertIfNull(nameof(_instance)))
            {
                UI.Logger.IfError()?.Message($"{nameof(Knowledges)}() Unable to show").Write();
                return;
            }

            UI.LoggerDefault.IfInfo()?.Message(
                "From technologies:\n" +
                _instance.KnownTechnologies.Select(t => t.ToStringShort(_instance.Stage)).ItemsToStringByLines() +
                "\n\nFrom knowledges:\n" +
                _instance.KnownKnowledges.Select(k => k.ToStringShort(_instance.Stage)).ItemsToStringByLines())
                .Write();
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                KnownRecipes.Clear();
                KnownKnowledges.Clear();
                KnownTechnologies.Clear();
                Stage = null;
                _hasFactionFullApiWrapper.EntityApi.UnsubscribeFromStage(OnStageChanged);
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromKnowledge(OnKnowledgeAddRemove);
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromTechnologies(OnTechnologyAddRemove);

                _hasFactionFullApiWrapper.Dispose();
                _hasFactionFullApiWrapper = null;

                _knowledgeEngineFullApiWrapper.Dispose();
                _knowledgeEngineFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _hasFactionFullApiWrapper = EntityApi.GetWrapper<HasFactionFullApi>(newEgo.OuterRef);
                _knowledgeEngineFullApiWrapper = EntityApi.GetWrapper<KnowledgeEngineFullApi>(newEgo.OuterRef);

                _hasFactionFullApiWrapper.EntityApi.SubscribeToStage(OnStageChanged);
                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToKnowledge(OnKnowledgeAddRemove);
                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToTechnologies(OnTechnologyAddRemove);
            }
        }

        private void OnStageChanged(MutationStageDef stage)
        {
            Stage = stage;

            foreach (var kvp in KnownRecipes)
            {
                var newIsAvailable = !KnowledgeLogic.IsKnowledgeBlocked(kvp.Value.ParentKnowledgeDef, Stage);
                if (newIsAvailable == kvp.Value.IsAvailable)
                    continue;

                kvp.Value.IsAvailable = newIsAvailable;
                KnownRecipeStateChanged?.Invoke(kvp.Key, kvp.Value);
            }
        }

        private void OnKnowledgeAddRemove(KnowledgeDef knowledgeDef, bool isRemoved, bool isFirstTime)
        {
            if (knowledgeDef.AssertIfNull(nameof(knowledgeDef)))
                return;

            if (isRemoved)
                KnownKnowledges.Remove(knowledgeDef);
            else
                KnownKnowledges.Add(knowledgeDef);

            OnKnowledgeWithRecipesAddRemove(knowledgeDef, isRemoved, isFirstTime);
        }

        private void OnKnowledgeWithRecipesAddRemove(KnowledgeDef knowledgeDef, bool isRemoved, bool isFirstTime)
        {
            var recipes = knowledgeDef.GetRecipes(Stage).ToList();
            if (recipes.Count == 0)
                return;

            if (isRemoved)
            {
                foreach (var baseRecipeDef in recipes)
                {
                    if (!KnownRecipes.ContainsKey(baseRecipeDef))
                    {
                        UI.Logger.IfError()?.Message($"{nameof(KnownRecipes)} don't contains {baseRecipeDef}").Write();
                        continue;
                    }

                    KnownRecipes.Remove(baseRecipeDef);
                    KnownRecipeRemove?.Invoke(baseRecipeDef);
                }
            }
            else
            {
                foreach (var baseRecipeDef in recipes)
                {
                    if (KnownRecipes.ContainsKey(baseRecipeDef))
                    {
                        UI.Logger.IfError()?.Message($"{nameof(KnownRecipes)} already contains {baseRecipeDef}").Write();
                        continue;
                    }

                    var recipeState =
                        new RecipeState(knowledgeDef) {IsAvailable = !KnowledgeLogic.IsKnowledgeBlocked(knowledgeDef, Stage)};
                    KnownRecipes[baseRecipeDef] = recipeState;
                    KnownRecipeAdd?.Invoke(baseRecipeDef, recipeState, isFirstTime);
                }
            }
        }

        private void OnTechnologyAddRemove(TechnologyDef technologyDef, bool isRemoved, bool isFirstTime)
        {
            if (technologyDef.AssertIfNull(nameof(technologyDef)))
                return;

            if (isRemoved)
                KnownTechnologies.Remove(technologyDef);
            else
                KnownTechnologies.Add(technologyDef);

            if (technologyDef.ActivateConditions.RewardKnowledges == null)
                return;

            foreach (var knowledgeDefRef in technologyDef.ActivateConditions.RewardKnowledges)
            {
                var knowledgeDef = knowledgeDefRef.Target;
                if (knowledgeDef.AssertIfNull(nameof(knowledgeDef)))
                    continue;

                OnKnowledgeWithRecipesAddRemove(knowledgeDef, isRemoved, isFirstTime);
            }
        }
    }
}