using System.Linq;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class CreditsHeadedLineContr : BindingController<CreditsHeadedLine>
    {
        [SerializeField, UsedImplicitly]
        private Transform _namesTransform;

        [SerializeField, UsedImplicitly]
        private TextContr _nameContrPrefab;

        private BindingControllersPool<string> _namesPool;


        //=== Props ===========================================================

        [Binding]
        public string HeadText { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            if (_namesTransform.AssertIfNull(nameof(_namesTransform)) ||
                _nameContrPrefab.AssertIfNull(nameof(_nameContrPrefab)))
                return;

            Bind(Vmodel.Func(D, line => line.Head), () => HeadText);
            var namesListStream = Vmodel.NonMutableEnumerableAsSubListStream(D, line => line.Names != null ? line.Names : Enumerable.Empty<string>());

            _namesPool = new BindingControllersPool<string>(_namesTransform, _nameContrPrefab);
            _namesPool.Connect(namesListStream);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _namesPool.Disconnect();
        }
    }
}