using JetBrains.Annotations;
using Uins;
using UnityEngine;
using UnityWeld.Binding;

namespace WeldAdds
{
    public enum BoolOperation
    {
        Or,
        And,
        Xor
    }

    [Binding]
    public class BoolAggregator : BindingViewModel
    {
        [SerializeField, UsedImplicitly]
        private string _title;

        [SerializeField, UsedImplicitly]
        private BoolOperation _operation;


        //=== Props ===========================================================

        private bool _flag1;

        public bool Flag1
        {
            get => _flag1;
            set
            {
                if (_flag1 != value)
                {
                    _flag1 = value;
                    Result = GetResult();
                }
            }
        }

        private bool _flag2;

        public bool Flag2
        {
            get => _flag2;
            set
            {
                if (_flag2 != value)
                {
                    _flag2 = value;
                    Result = GetResult();
                }
            }
        }

        private bool _result;

        [Binding]
        public bool Result
        {
            get => _result;
            set
            {
                if (_result != value)
                {
                    _result = value;
                    NotifyPropertyChanged();
                }
            }
        }


        //=== Unity ===========================================================

        private void Awake()
        {
            Result = GetResult();
        }


        //=== Private =========================================================

        private bool GetResult()
        {
            switch (_operation)
            {
                case BoolOperation.And:
                    return Flag1 && Flag2;
                case BoolOperation.Xor:
                    return Flag1 ^ Flag2;
            }

            return Flag1 || Flag2;
        }
    }
}