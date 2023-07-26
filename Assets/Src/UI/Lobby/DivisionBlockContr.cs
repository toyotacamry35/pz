using System.Linq;
using JetBrains.Annotations;
using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class DivisionBlockContr : CreditsBlockContr<DivisionBlock>
    {
        [SerializeField, UsedImplicitly]
        private Transform _linesTransform;

        [SerializeField, UsedImplicitly]
        private CreditsHeadedLineContr _headedLineContrPrefab;

        [SerializeField, UsedImplicitly]
        private CreditsLineContr _lineContrPrefab;

        private BindingControllersPool<CreditsHeadedLine> _headedLinesPool;
        private BindingControllersPool<CreditsLine> _linesPool;


        //=== Unity ===========================================================

        protected override void Awake()
        {
            base.Awake();
            if (_linesTransform.AssertIfNull(nameof(_linesTransform)) ||
                _headedLineContrPrefab.AssertIfNull(nameof(_headedLineContrPrefab)) ||
                _lineContrPrefab.AssertIfNull(nameof(_lineContrPrefab)))
                return;

            var headedLinesList = Vmodel.NonMutableEnumerableAsSubListStream(D, block => block?.HeadedLines ?? Enumerable.Empty<CreditsHeadedLine>());
            _headedLinesPool = new BindingControllersPool<CreditsHeadedLine>(_linesTransform, _headedLineContrPrefab);
            _headedLinesPool.Connect(headedLinesList);

            var linesList = Vmodel.NonMutableEnumerableAsSubListStream(D, block => block?.Lines ?? Enumerable.Empty<CreditsLine>());
            _linesPool = new BindingControllersPool<CreditsLine>(_linesTransform, _lineContrPrefab);
            _linesPool.Connect(linesList);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _headedLinesPool.Disconnect();
            _linesPool.Disconnect();
        }
    }
}