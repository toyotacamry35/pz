using L10n;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class EndGameScreenNode : DependencyEndNode
    {
        private readonly ListStream<object> _causers = new ListStream<object>();


        //=== Props ===========================================================

        public static EndGameScreenNode Instance { get; private set; }

        [Binding]
        public bool IsVisible { get; private set; }

        [Binding]
        public LocalizedString Text { get; private set; }

        private ReactiveProperty<LocalizedString> TextRp { get; } = new ReactiveProperty<LocalizedString>();

        [Binding]
        public LocalizedString Title { get; private set; }

        private ReactiveProperty<LocalizedString> TitleRp { get; } = new ReactiveProperty<LocalizedString>();

        [Binding]
        public Sprite Sprite { get; private set; }

        private ReactiveProperty<Sprite> SpriteRp { get; } = new ReactiveProperty<Sprite>();


        //=== Unity ===========================================================

        private void Awake()
        {
            Instance = SingletonOps.TrySetInstance(this, Instance);

            Bind(TitleRp, () => Title);
            Bind(TextRp, () => Text);
            Bind(SpriteRp, () => Sprite);
            Bind(_causers.CountStream.Func(D, count => count > 0), () => IsVisible);
        }

        protected override void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            base.OnDestroy();
        }


        //=== Public ==========================================================

        public void Show(object causer, LocalizedString title, LocalizedString text, Sprite sprite)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(
                () =>
                {
                    _causers.Add(causer);
                    TitleRp.Value = title;
                    TextRp.Value = text;
                    SpriteRp.Value = sprite;
                });
        }

        public void Hide(object causer)
        {
            UnityQueueHelper.RunInUnityThreadNoWait(() => { _causers.Remove(causer); });
        }
    }
}