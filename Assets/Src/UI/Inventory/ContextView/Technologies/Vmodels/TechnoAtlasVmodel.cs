using System.Linq;
using Core.Environment.Logging.Extension;
using EnumerableExtensions;
using ReactivePropsNs;
using SharedCode.Aspects.Science;
using Utilities;

namespace Uins
{
    public class TechnoAtlasVmodel : BindingVmodel
    {
        public readonly TechnologyAtlasDef TechnologyAtlasDef;

        public IStream<TechnoTabVmodel> SelectedTabVmodelStream;

        public TechnoTabVmodel[] TechnoTabVmodels;


        //=== Props ===========================================================

        public ReactiveProperty<int> SelectedTabIndexRp { get; } = new ReactiveProperty<int>() {Value = 0};

        public CharacterStreamsData CharacterStreamsData { get; private set; }


        //=== Ctor ============================================================

        public TechnoAtlasVmodel(TechnologyAtlasDef technologyAtlasDef, CharacterStreamsData characterStreamsData)
        {
            TechnologyAtlasDef = technologyAtlasDef;
            CharacterStreamsData = characterStreamsData;

            if (TechnologyAtlasDef.AssertIfNull(nameof(TechnologyAtlasDef)))
                return;

            if (TechnologyAtlasDef.Tabs == null)
            {
                UI.Logger.IfError()?.Message($"{TechnologyAtlasDef.____GetDebugRootName()} Hasn't tabs").Write();
                return;
            }

            TechnoTabVmodels = TechnologyAtlasDef.Tabs
                .Select(technologyTabDef => new TechnoTabVmodel(technologyTabDef, this, characterStreamsData))
                .ToArray();

            SelectedTabVmodelStream = SelectedTabIndexRp.Func(D, idx => idx < 0 || idx >= TechnoTabVmodels.Length ? null : TechnoTabVmodels[idx]);
        }


        //=== Public ==========================================================

        /// <summary>
        /// Запрос на установку выбранной TechnoTabVmodel
        /// </summary>
        public void SetSelectedTabVmodel(TechnoTabVmodel technoTabVmodel)
        {
            if (TechnoTabVmodels == null)
                return;

            var selectedIndex = technoTabVmodel == null ? -1 : TechnoTabVmodels.IndexOf(vm => vm == technoTabVmodel);
            if (selectedIndex < 0)
            {
                UI.Logger.IfError()?.Message($"Not registered {nameof(TechnoTabVmodel)} {technoTabVmodel}").Write();
                return;
            }

            SelectedTabIndexRp.Value = selectedIndex;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (TechnoTabVmodels != null)
            {
                TechnoTabVmodels.ForEach(vm => vm.Dispose());
                TechnoTabVmodels = null;
            }

            CharacterStreamsData = new CharacterStreamsData();
            SelectedTabIndexRp.Dispose();
        }
    }
}