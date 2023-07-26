using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using L10n;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class BaseStartGameContentCtrl : BindingController<StartGameWindowVM>
    {
        private readonly HashSet<string> _busyCommands = new HashSet<string>();
        private readonly ReactiveProperty<LocalizedString> _title = new ReactiveProperty<LocalizedString>();
        private readonly ReactiveProperty<SemanticContext> _titleSemanticContext = new ReactiveProperty<SemanticContext>();

        [SerializeField]
        private StartGameWindowStateEnum RepresentedState;

        [SerializeField]
        protected LocalizationKeyProp TitleHolder;

        [Binding, UsedImplicitly]
        public bool IsVisible { get; set; }

        public IStream<LocalizedString> Title => _title;

        public IStream<SemanticContext> TitleSemanticContext => _titleSemanticContext;
        public StartGameWindowStateEnum RepresentedStateProp => RepresentedState;

        protected void RunSingleCommandAsync(Task task, [CallerMemberName] string callerName = "")
        {
            if (_busyCommands.Add(callerName))
                UnityQueueHelper.RunInUnityThread(
                    async () =>
                    {
                        try
                        {
                            await task;
                        }
                        finally
                        {
                            _busyCommands.Remove(callerName);
                        }
                    }
                );
        }

        protected virtual void Start()
        {
            SetTitle(TitleHolder, SemanticContext.Primary);

            Bind(Vmodel.Func(D, vm => vm != null), () => IsVisible);
        }

        protected void SetTitle(LocalizationKeyProp localizationKeyProp, SemanticContext semanticContext)
        {
            _title.Value = localizationKeyProp.LocalizedString;
            _titleSemanticContext.Value = semanticContext;
        }
    }
}