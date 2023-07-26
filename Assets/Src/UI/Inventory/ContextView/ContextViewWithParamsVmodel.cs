using System.Collections.Generic;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ReactivePropsNs;
using Uins.Inventory;

namespace Uins
{
    public class ContextViewWithParamsVmodel : BindingVmodel, IContextViewWithParams
    {
        //=== Props ==============================================================

        public DictionaryStream<InventoryTabType, InventoryTabVmodel> TabVmodels { get; } = new DictionaryStream<InventoryTabType, InventoryTabVmodel>();

        public ReactiveProperty<(IContextViewTarget, ContextViewParams)> CurrentContextWithParamsRp { get; }
            = new ReactiveProperty<(IContextViewTarget, ContextViewParams)>();

        public ReactiveProperty<InventoryTabVmodel> CurrentTabRp { get; } = new ReactiveProperty<InventoryTabVmodel>() {Value = null};

        private IStream<IContextViewTarget> _currentContextStream;

        public IStream<IContextViewTarget> CurrentContext
        {
            get
            {
                if (_currentContextStream == null)
                    _currentContextStream = CurrentContextWithParamsRp.Func(D, (target, cvParams) => target);

                return _currentContextStream;
            }
        }

        public IContextViewTarget ContextValue => CurrentContextWithParamsRp.HasValue ? CurrentContextWithParamsRp.Value.Item1 : null;

        public IStream<ContextViewParams> ContextParamsStream { get; }


        //=== Ctor ============================================================

        public ContextViewWithParamsVmodel(Dictionary<InventoryTabType, InventoryTabVmodel> tabVmodels)
        {
            CurrentContextWithParamsRp.Value = (null, new ContextViewParams());
            ContextParamsStream = CurrentContextWithParamsRp.Func(D, (target, cvParams) => cvParams);
            if ((tabVmodels?.Count ?? 0) > 0)
            {
                tabVmodels.ForEach(kvp =>
                {
                    TabVmodels.Add(kvp.Key, kvp.Value);
                    kvp.Value.Init(this);
                });
            }
        }


        //=== Public ==========================================================

        public void SetTabsContext(InventoryTabVmodel tabVmodel)
        {
            //UI.CallerLog($"---- tabVm={tabVmodel}"); //DEBUG
            if (tabVmodel == CurrentTabRp.Value || (tabVmodel != null && !tabVmodel.IsOpenableRp.Value))
                return;

            CurrentTabRp.Value = tabVmodel;
            if (tabVmodel != null)
            {
                //Если при смене таба у нового таба есть единственный таргет, то выставляем его в главном контексте
                if (tabVmodel.ContextSingleTarget != null &&
                    tabVmodel.ContextSingleTarget != CurrentContextWithParamsRp.Value.Item1)
                {
                    CurrentContextWithParamsRp.Value =
                        (tabVmodel.ContextSingleTarget, tabVmodel.ContextSingleTarget.GetContextViewParamsForOpening());
                }
            }
            else
            {
                var currContextViewTargetWithParams = CurrentContextWithParamsRp.Value.Item1 as IContextViewTargetWithParams;
                if (currContextViewTargetWithParams?.TabType != null)
                {
                    //Если на момент обнуления таба в главном контексте есть таргет, принадлежащий (этому) табу, то обнуляем главный контекст
                    CurrentContextWithParamsRp.Value = (null, new ContextViewParams());
                }
            }
        }

        public void SetContext(IContextViewTargetWithParams target)
        {
            //UI.CallerLog($"---- target={target}"); //DEBUG
            //Ограничиваем по уникальности target, чтобы не прошло два события с разными contextViewParams и одинаковыми target
            if (target == CurrentContextWithParamsRp.Value.Item1)
                return;

            var contextViewParams = target != null ? target.GetContextViewParamsForOpening() : new ContextViewParams();

            var newTabType = target?.TabType;
            if (newTabType != null && !IsTabOpenable(newTabType.Value)) //проверка на допустимость смены контекста по табу
            {
                UI.Logger.IfWarn()?.Message($"Attempt to set context {target} with unopenable tab {newTabType.Value}").Write();
                return;
            }

            CurrentContextWithParamsRp.Value = (target, contextViewParams);

            if (newTabType != null)
            {
                //Если target имеет TabType, то надо переключить на таб соответствующего типа
                if (CurrentTabRp.Value?.TabType != newTabType)
                    CurrentTabRp.Value = GetTabVmodel(newTabType.Value);
            }
            else
            {
                //Если нет target или он без TabType, а текущий таб имеет единственный таргет, то очищаем таб
                if (CurrentTabRp.Value?.ContextSingleTarget != null)
                    CurrentTabRp.Value = null;
            }
        }

        public InventoryTabVmodel GetTabVmodel(InventoryTabType tabType)
        {
            if (!TabVmodels.TryGetValue(tabType, out var tabVmodel))
            {
                UI.Logger.IfError()?.Message($"Unable to get {nameof(InventoryTabVmodel)} by type {tabType}").Write();
                return null;
            }

            return tabVmodel;
        }


        //=== Private =========================================================

        private bool IsTabOpenable(InventoryTabType inventoryTabType)
        {
            if (!TabVmodels.TryGetValue(inventoryTabType, out var tabVmodel))
                return false;

            return tabVmodel.IsOpenableRp.Value;
        }
    }
}