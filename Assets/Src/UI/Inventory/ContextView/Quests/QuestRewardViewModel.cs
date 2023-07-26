using Assets.ColonyShared.SharedCode.Aspects.Craft;
using Core.Environment.Logging.Extension;
using JetBrains.Annotations;
using L10n;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class QuestRewardViewModel : SomeItemViewModel<QuestRewardData>
    {
        private QuestRewardData _questRewardData;

        [SerializeField, UsedImplicitly]
        private ScienceViewModel _scienceViewModel;

        [SerializeField, UsedImplicitly]
        private TechPointViewModel _techPointViewModel;


        //=== Props ===========================================================

        [Binding]
        public bool HasItemResource => _questRewardData != null && _questRewardData.RewardType == QuestRewardType.Item;

        [Binding]
        public bool HasScience => _questRewardData != null && _questRewardData.RewardType == QuestRewardType.Science;

        [Binding]
        public bool HasRecipe => _questRewardData != null && _questRewardData.RewardType == QuestRewardType.Recipe;

        [Binding]
        public bool HasTechPoint => _questRewardData != null && _questRewardData.RewardType == QuestRewardType.TechPoint;

        [Binding]
        public string ItemResourceName => _questRewardData?.Item?.ItemNameLs.GetText();

        [Binding]
        public Sprite ItemResourceSprite => _questRewardData?.Item?.Icon?.Target;

        [Binding]
        public Sprite RecipeBlueprintSprite => _questRewardData?.Recipe?.BlueprintIcon?.Target;

        [Binding]
        public string RecipeName
        {
            get
            {
                if (_questRewardData == null)
                    return "";

                var baseRecipeDef = _questRewardData.Recipe;
                if (baseRecipeDef is RepairRecipeDef repairRecipeDef)
                    return repairRecipeDef.GetRecipeOrProductNameLs().GetText();

                return baseRecipeDef.NameLs.GetText();
            }
        }

        [Binding]
        public int Count => _questRewardData?.Count ?? 0;

        [Binding]
        public bool IsCountMoreThanOne => Count > 1;


        //=== Unity ===========================================================

        private void Awake()
        {
            _scienceViewModel.AssertIfNull(nameof(_scienceViewModel));
            _techPointViewModel.AssertIfNull(nameof(_techPointViewModel));
        }


        //=== Public ==========================================================

        public override void Fill(QuestRewardData questRewardData)
        {
            if (questRewardData.AssertIfNull(nameof(questRewardData)))
                return;

            _questRewardData = questRewardData;
            NotifyPropertyChanged(nameof(HasItemResource));
            NotifyPropertyChanged(nameof(HasRecipe));
            NotifyPropertyChanged(nameof(HasScience));
            NotifyPropertyChanged(nameof(HasTechPoint));

            switch (_questRewardData.RewardType)
            {
                case QuestRewardType.Item:
                    NotifyPropertyChanged(nameof(ItemResourceSprite));
                    NotifyPropertyChanged(nameof(ItemResourceName));
                    NotifyPropertyChanged(nameof(Count));
                    NotifyPropertyChanged(nameof(IsCountMoreThanOne));
                    break;

                case QuestRewardType.Recipe:
                    NotifyPropertyChanged(nameof(RecipeBlueprintSprite));
                    NotifyPropertyChanged(nameof(RecipeName));
                    break;

                case QuestRewardType.Science:
                    NotifyPropertyChanged(nameof(Count));
                    _scienceViewModel.Set(_questRewardData.Science, _questRewardData.Count, true);
                    break;

                case QuestRewardType.TechPoint:
                    NotifyPropertyChanged(nameof(Count));
                    _techPointViewModel.Set(_questRewardData.TechPoint, Count, true);
                    break;

                default:
                    UI.Logger.IfError()?.Message($"Unhandled QuestRewardType: {_questRewardData.RewardType}").Write();
                    break;
            }
        }
    }
}