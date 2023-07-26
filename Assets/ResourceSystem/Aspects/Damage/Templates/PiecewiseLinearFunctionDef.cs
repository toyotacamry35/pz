using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using static Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Assets.ColonyShared.SharedCode.Aspects.Damage.Templates
{
    public class PiecewiseLinearFunctionDef : BaseResource
    {
        [UsedImplicitly] public float[] Keys;
        [UsedImplicitly] public float[] Values;

        private ValueTuple<float,float>[] _table;
 
        public float Evaluate(float val)
        {
            if (_table == null)
            {
                if(Keys.Length != Values.Length) throw new Exception($"Keys ({Keys.Length}) and values ({Values.Length}) count mismatch at {____GetDebugAddress()}");
                _table = Keys.Select((x,i) => ValueTuple.Create(x, Values[i])).ToArray();
                Array.Sort(_table, (x,y) => x.Item1 < y.Item1 ? -1 : x.Item1 > y.Item1 ? 1 : 0);
                if(_table.Length == 0) throw new Exception($"Empty definition at {____GetDebugAddress()}");
            }

            if (_table.Length == 0)
                return 0;
            
            for (int i = 0; i < _table.Length; ++i)
                if (val <= _table[i].Item1)
                    return i > 0 ? Lerp(_table[i - 1].Item2, _table[i].Item2,InverseLerp(_table[i - 1].Item1, _table[i].Item1, val)) : _table[i].Item2;
            
            return _table[_table.Length - 1].Item2;
        }
    }
}