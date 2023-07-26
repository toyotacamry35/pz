using System.Linq;
using System.Text;
using Assets.Src.ResourceSystem.L10n;
using EnumerableExtensions;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class AchievedPointsNotificationViewModel : NotificationViewModel
    {
        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _textBeginsKeyPairsDefRef;

        ListStream<TechPointVmodel> _techPointListStream = new ListStream<TechPointVmodel>();
        ListStream<ScienceVmodel> _scienceListStream = new ListStream<ScienceVmodel>();

        //TODO Переделать на Vmodel/Contr-схему
        [Binding]
        public ObservableList<TechPointVmodel> TechPointVmodels { get; } = new ObservableList<TechPointVmodel>();

        [Binding]
        public ObservableList<ScienceVmodel> ScienseVmodels { get; } = new ObservableList<ScienceVmodel>();

        [SerializeField, UsedImplicitly]
        private Color _resourcesconstantPartTextColor = Color.blue;

        ReactiveProperty<string> _sciencesTextRp = new ReactiveProperty<string>();
        ReactiveProperty<string> _techPointsTextRp = new ReactiveProperty<string>();


        //=== Props ===========================================================

        [Binding]
        public string SciencesText { get; private set; }

        [Binding]
        public string TechPointsText { get; private set; }

        [Binding]
        public bool HasManyResources { get; private set; }

        [Binding]
        public bool HasSciences { get; private set; }

        [Binding]
        public bool HasTechPoins { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _textBeginsKeyPairsDefRef.Target.AssertIfNull(nameof(_textBeginsKeyPairsDefRef));

            Bind(_techPointListStream, TechPointVmodels);
            Bind(_scienceListStream, ScienseVmodels);

            Bind(_techPointListStream.CountStream.Func(D, c => c > 0), () => HasTechPoins);
            Bind(_scienceListStream.CountStream.Func(D, c => c > 0), () => HasSciences);

            var hasManyResourcesStream = _techPointListStream.CountStream
                .Zip(D, _scienceListStream.CountStream)
                .Func(D, (c1, c2) => c1 + c2 > 1);
            Bind(hasManyResourcesStream, () => HasManyResources);

            Bind(_sciencesTextRp, () => SciencesText);
            Bind(_techPointsTextRp, () => TechPointsText);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _sciencesTextRp.Dispose();
            _techPointsTextRp.Dispose();
        }


        //=== Public ==========================================================

        public override void Show(HudNotificationInfo info)
        {
            base.Show(info);
            var achievedPointsNotificationInfo = info as AchievedPointsNotificationInfo;
            if (achievedPointsNotificationInfo.AssertIfNull(nameof(achievedPointsNotificationInfo)))
                return;

            //SoundControl.Instance.KnowledgeBaseNewNote.Post(transform.root.gameObject);

            _techPointListStream.Clear();
            _scienceListStream.Clear();

            achievedPointsNotificationInfo.TechPointCounts?
                .Where(tpc => tpc.Count > 0 && tpc.TechPoint != null)
                .ForEach(tpc => _techPointListStream.Add(new TechPointVmodel(tpc)));

            achievedPointsNotificationInfo.ScienceCounts?
                .Where(sc => sc.Count > 0 && sc.Science != null)
                .ForEach(sc => _scienceListStream.Add(new ScienceVmodel(sc)));

            _sciencesTextRp.Value = GetSciencesText(_scienceListStream);
            _techPointsTextRp.Value = GetTechPointsText(_techPointListStream);
            AkSoundEngine.PostEvent("UI_Notif_ResourceObtain", transform.root.gameObject);
        }


        //=== Private =========================================================

        private string GetTechPointsText(ListStream<TechPointVmodel> techPointListStream)
        {
            var list = techPointListStream.ToList();
            if (list.Count == 0)
                return "";

            var sb = new StringBuilder(
                $"<color=#{ColorUtility.ToHtmlStringRGBA(_resourcesconstantPartTextColor)}>{_textBeginsKeyPairsDefRef.Target.Ls1.GetText()}</color> ");
            sb.Append(list.Select(e => $"{e.NameLs.GetText()} ({e.ShortNameLs.GetText()}) +{e.Count}").ItemsToStringSimple());
            return sb.ToString();
        }

        private string GetSciencesText(ListStream<ScienceVmodel> scienceListStream)
        {
            var list = scienceListStream.ToList();
            if (list.Count == 0)
                return "";

            var sb = new StringBuilder(
                $"<color=#{ColorUtility.ToHtmlStringRGBA(_resourcesconstantPartTextColor)}>{_textBeginsKeyPairsDefRef.Target.Ls2.GetText()}</color> ");
            sb.Append(list.Select(e => $"{e.NameLs.GetText()} +{e.Count}").ItemsToStringSimple());
            return sb.ToString();
        }
    }
}