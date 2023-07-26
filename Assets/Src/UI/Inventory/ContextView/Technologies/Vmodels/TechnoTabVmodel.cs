using System.Linq;
using EnumerableExtensions;
using ReactivePropsNs;
using SharedCode.Aspects.Science;

namespace Uins
{
    public class TechnoTabVmodel : BindingVmodel
    {
        public readonly TechnologyTabDef TechnologyTabDef;

        public readonly TechnoAtlasVmodel TechnoAtlasVmodel;

        public TechnoItemVmodel[] ItemVmodels;

        public TechnoLinkVmodel[] LinkVmodels;


        //=== Props ===========================================================

        public ReactiveProperty<bool> IsSelectedRp { get; } = new ReactiveProperty<bool>() {Value = false};

        public ReactiveProperty<TechnoItemVmodel> SelectedItemRp { get; } = new ReactiveProperty<TechnoItemVmodel>() {Value = null};


        //=== Ctor ============================================================

        public TechnoTabVmodel(TechnologyTabDef technologyTabDef, TechnoAtlasVmodel technoAtlasVmodel, CharacterStreamsData characterStreamsData)
        {
            TechnologyTabDef = technologyTabDef;
            TechnoAtlasVmodel = technoAtlasVmodel;
            if (TechnologyTabDef.AssertIfNull(nameof(TechnologyTabDef)) ||
                TechnoAtlasVmodel.AssertIfNull(nameof(TechnoAtlasVmodel)))
                return;

            if (TechnologyTabDef.Items != null)
            {
                ItemVmodels = TechnologyTabDef.Items
                    .Select(technologyItem => new TechnoItemVmodel(technologyItem, this, characterStreamsData))
                    .ToArray();

                LinkVmodels = TechnologyTabDef.Items
                    .Where(technologyItem => technologyItem.Technology.Target.ActivateConditions.Requirements.IsRequiredTechnologies)
                    .SelectMany(technologyItem => technologyItem.Technology.Target.ActivateConditions.Requirements.Technologies
                        .Select(tdRef => new TechnoLinkVmodel(technologyItem, tdRef.Target, ItemVmodels)))
                    .ToArray();
            }

            ItemVmodels.ForEach(vm => vm.AfterAllItemVmodelsCreated());
        }


        //=== Public ==========================================================

        public void SetSelectedItemVmodel(TechnoItemVmodel technoItemVmodel)
        {
            SelectedItemRp.Value = technoItemVmodel;
        }

        public override string ToString()
        {
            return $"{nameof(TechnoTabVmodel)} {TechnologyTabDef} {nameof(IsSelectedRp)}{IsSelectedRp.Value.AsSign()}";
        }

        public override void Dispose()
        {
            base.Dispose();
            if (LinkVmodels != null)
            {
                LinkVmodels.ForEach(vm => vm.Dispose());
                LinkVmodels = null;
            }

            if (ItemVmodels != null)
            {
                ItemVmodels.ForEach(vm => vm.Dispose());
                ItemVmodels = null;
            }

            IsSelectedRp.Dispose();
            SelectedItemRp.Dispose();
        }
    }
}