using System.Linq;
using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Assets.Src.Aspects;
using Assets.Src.Aspects.Impl;
using Assets.Src.ContainerApis;
using Assets.Src.ResourceSystem.L10n;
using Assets.Src.SpawnSystem;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using UnityEngine;
using UnityEngine.Serialization;

namespace Uins.Knowledge
{
    public class KnowledgeMessage : HasDisposablesMonoBehaviour
    {
        [SerializeField, UsedImplicitly]
        private WarningMessager _warningMessager;

        [SerializeField, UsedImplicitly]
        [FormerlySerializedAs("Color")]
        private Color _color;

        public Sprite KnowledgeSprite;
        public Sprite RecipeSprite;

        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _learnMessagesDefRef;

        private EntityApiWrapper<KnowledgeEngineFullApi> _knowledgeEngineFullApiWrapper;


        //=== Props ===========================================================

        private LocalizationKeyPairsDef LearnMessagesDef => _learnMessagesDefRef.Target;


        //=== Unity ===========================================================

        private void Awake()
        {
            _warningMessager.AssertIfNull(nameof(_warningMessager));
            KnowledgeSprite.AssertIfNull(nameof(KnowledgeSprite));
            RecipeSprite.AssertIfNull(nameof(RecipeSprite));
            _learnMessagesDefRef.Target.AssertIfNull(nameof(_learnMessagesDefRef));
        }


        //=== Public ==========================================================

        public void Init(IPawnSource pawnSource)
        {
            if (!pawnSource.AssertIfNull(nameof(pawnSource)))
                pawnSource.PawnChangesStream.Action(D, OnOurPawnChanged);
        }


        //=== Private =========================================================

        private void OnOurPawnChanged(EntityGameObject prevEgo, EntityGameObject newEgo)
        {
            if (prevEgo != null)
            {
                _knowledgeEngineFullApiWrapper.EntityApi.UnsubscribeFromKnowledge(OnKnowledgeAddRemove);
                _knowledgeEngineFullApiWrapper.Dispose();
                _knowledgeEngineFullApiWrapper = null;
            }

            if (newEgo != null)
            {
                _knowledgeEngineFullApiWrapper = EntityApi.GetWrapper<KnowledgeEngineFullApi>(newEgo.OuterRef);
                _knowledgeEngineFullApiWrapper.EntityApi.SubscribeToKnowledge(OnKnowledgeAddRemove);
            }
        }

        private void OnKnowledgeAddRemove(KnowledgeDef knowledgeDef, bool isRemoved, bool atFirstTime)
        {
            if (atFirstTime || isRemoved)
                return;

            if (knowledgeDef.Recipes != null && knowledgeDef.Recipes.Length > 0)
            {
                var baseRecipeDef = knowledgeDef.Recipes.FirstOrDefault().Target;
                var recipeNameLs = baseRecipeDef is RepairRecipeDef repairRecipeDef
                    ? repairRecipeDef.GetRecipeOrProductNameLs()
                    : baseRecipeDef.NameLs;

                var msg = knowledgeDef.Recipes.Length == 1
                    ? LearnMessagesDef.Ls2.GetText(0, recipeNameLs.GetText()) //1 рецепт
                    : LearnMessagesDef.AltLs2.GetText(0, recipeNameLs.GetText(), knowledgeDef.Recipes.Length - 1); //несколько

                _warningMessager.ShowWarningMessage(msg, _color, RecipeSprite);
            }
        }
    }
}