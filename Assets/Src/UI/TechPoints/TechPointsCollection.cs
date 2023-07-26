using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TechPointsCollection : ItemsCollection<TechPointViewModel, TechPointViewModelData>
    {
        protected override bool CanReuseItems => true;
    }
}