using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class SciencesCollection : ItemsCollection<ScienceViewModel, ScienceViewModelData>
    {
        protected override bool CanReuseItems => true;
    }
}