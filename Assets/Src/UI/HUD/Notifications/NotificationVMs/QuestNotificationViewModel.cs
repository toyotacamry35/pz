using Assets.Src.ResourceSystem.L10n;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;
using Uins.Sound;

namespace Uins
{
    [Binding]
    public class QuestNotificationViewModel : NotificationViewModel
    {
        [SerializeField, UsedImplicitly]
        private LocalizationKeyPairsDefRef _questTitlesPairsDefRef;

        ReactiveProperty<LocalizedString> _questNameRp = new ReactiveProperty<LocalizedString>();
        ReactiveProperty<bool> _isNewNorDoneRp = new ReactiveProperty<bool>();
        ReactiveProperty<LocalizedString> _titleRp = new ReactiveProperty<LocalizedString>();


        //=== Props ===========================================================

        [Binding]
        public LocalizedString QuestName { get; private set; }

        [Binding]
        public LocalizedString Title { get; private set; }

        [Binding]
        public bool IsNewNorDone { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _questTitlesPairsDefRef.Target.AssertIfNull(nameof(_questTitlesPairsDefRef));
            Bind(_titleRp, () => Title);
            Bind(_questNameRp, () => QuestName);
            Bind(_isNewNorDoneRp, () => IsNewNorDone);
        }


        //=== Public ==========================================================

        public override void Show(HudNotificationInfo info)
        {
            base.Show(info);
            var questNotificationInfo = info as QuestNotificationInfo;
            if (questNotificationInfo.AssertIfNull(nameof(questNotificationInfo)))
                return;

            var soundEvent = questNotificationInfo.IsNewNorDone ? SoundControl.Instance.QuestObtain : SoundControl.Instance.QuestComplete;
            soundEvent.Post(transform.root.gameObject);
            _questNameRp.Value = questNotificationInfo.QuestName;
            _isNewNorDoneRp.Value = questNotificationInfo.IsNewNorDone;
            _titleRp.Value = questNotificationInfo.IsNewNorDone ? _questTitlesPairsDefRef.Target.Ls1 : _questTitlesPairsDefRef.Target.Ls2;
        }
    }
}