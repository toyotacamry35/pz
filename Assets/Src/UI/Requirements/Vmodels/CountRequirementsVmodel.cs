using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ReactivePropsNs;

namespace Uins
{
    public class CountRequirementsVmodel<T> : BindingVmodel where T : BaseResource
    {
        public ListStream<RequirementVmodel<T>> RequirementVmodels { get; }

        public ReactiveProperty<bool> IsEnoughRp { get; } = new ReactiveProperty<bool>() {Value = true};


        //=== Ctor ============================================================

        /// <summary>
        /// Типовой контейнер Vmodels требований по числу ресурсов
        /// </summary>
        /// <param name="requirements">требования технологии</param>
        /// <param name="availResourcesStream">доступные игроку ресурсы</param>
        /// <param name="neededResources">ключи ресурсов, которые должны присутствовать в словаре требований</param>
        public CountRequirementsVmodel(
            IDictionary<T, int> requirements,
            DictionaryStream<T, int> availResourcesStream,
            Func<RequirementVmodel<T>, int> sortingFunc,
            ListStream<T> neededResources = null)
        {
            if (sortingFunc.AssertIfNull(nameof(sortingFunc)))
                return;

            var actualRequirementVmodels = new DictionaryStream<T, RequirementVmodel<T>> {AllowToFinalizeWithoutDispose = true};
            IStream<bool> isEnoughStream = null;
            if (requirements != null && requirements.Count > 0)
                foreach (var requirementKvp in requirements)
                {
                    if (actualRequirementVmodels.ContainsKey(requirementKvp.Key))
                    {
                        UI.Logger.IfError()?.Message($"Double key {requirementKvp.Key} in {nameof(requirements)}").Write();
                        continue;
                    }

                    var vmodel = new RequirementVmodel<T>(requirementKvp.Key, requirementKvp.Value, availResourcesStream);
                    actualRequirementVmodels.Add(requirementKvp.Key, vmodel);
                    isEnoughStream = isEnoughStream != null ? isEnoughStream.Zip(D, vmodel.IsEnoughRp).Func(D, (b1, b2) => b1 && b2) : vmodel.IsEnoughRp;
                }

            var finalRequirementVmodels = neededResources != null
                ? actualRequirementVmodels.Join(D, neededResources, key => new RequirementVmodel<T>(key, 0))
                : actualRequirementVmodels;

            RequirementVmodels = finalRequirementVmodels.ToSortedListStream(D, sortingFunc);

            isEnoughStream?.Bind(D, IsEnoughRp);
        }


        //=== Public ==========================================================

        public override string ToString()
        {
            return
                $"({GetType().NiceName()}: {nameof(IsEnoughRp)}{IsEnoughRp.Value.AsSign()}, {nameof(RequirementVmodels)}: " +
                $"{RequirementVmodels.ItemsToStringByLines()})";
        }

        public override void Dispose()
        {
            base.Dispose();
            IsEnoughRp.Dispose();
        }
    }
}