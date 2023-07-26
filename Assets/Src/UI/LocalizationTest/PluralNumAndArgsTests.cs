using JetBrains.Annotations;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class PluralNumAndArgsTests : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private string[] _userNames = new[] {"Василий", "James", "guest"};

        private int _paramsIndex;


        //=== Props ===========================================================

        [SerializeField, UsedImplicitly]
        private int _pluralNumber;

        [Binding]
        public int PluralNumber
        {
            get { return _pluralNumber; }
            set
            {
                if (_pluralNumber != value)
                {
                    _pluralNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _userName;

        [Binding]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            UserName = _userNames[GetNewIndex()];
        }

        //=== Public ==========================================================

        public void OnTextArgsChangedButton()
        {
            UserName = _userNames[GetNewIndex()];
        }

        public void OnPluralNumberChange(bool isIncrement)
        {
            PluralNumber += (isIncrement ? 1 : -1);
        }


        //=== Private =============================================================

        private int GetNewIndex()
        {
            _paramsIndex++;
            if (_paramsIndex >= _userNames.Length)
                _paramsIndex = 0;

            return _paramsIndex;
        }
    }
}