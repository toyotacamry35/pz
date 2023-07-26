using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PaginationContentCtrl : BindingController<IPaginationVM>
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField, UsedImplicitly]
        private Transform Container;

        [SerializeField, UsedImplicitly]
        private PaginationElementCtrl PaginationElementPrefab;
        
        private void Awake()
        {
            foreach (Transform child in Container.transform)
                Destroy(child.gameObject);

            var realmRulesQueryVms = Vmodel.SubListStream(D, vm => vm.PaginationElementVMs);
            var gameModeCtrlsPool = new BindingControllersPool<IPaginationElementVM>(Container, PaginationElementPrefab);
            gameModeCtrlsPool.Connect(realmRulesQueryVms);
        }
    }
}