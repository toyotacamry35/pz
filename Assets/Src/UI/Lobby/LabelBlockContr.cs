using System.Linq;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class LabelBlockContr : CreditsBlockContr<LabelBlock>
    {
        [SerializeField, UsedImplicitly]
        private Transform _textsTransform;

        [SerializeField, UsedImplicitly]
        private TextContr _textContrPrefab;

        private BindingControllersPool<string> _pool;


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            if (_textsTransform.AssertIfNull(nameof(_textsTransform)) ||
                _textContrPrefab.AssertIfNull(nameof(_textContrPrefab)))
                return;

            var textsList = Vmodel.NonMutableEnumerableAsSubListStream(D, block => block?.Names ?? Enumerable.Empty<string>());

            _pool = new BindingControllersPool<string>(_textsTransform, _textContrPrefab);
            _pool.Connect(textsList);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _pool.Disconnect();
        }
    }
}