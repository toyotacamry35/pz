using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.Wizardry;

namespace GeneratedCode.Network.Statistic
{
    public class SpellCastStat
    {
        public SpellDef SpellIdField;
        public long UsedField;
        public long CastedField;

        public string SpellId => SpellIdField.ToString();
        public long Used => UsedField;
        public long Casted => CastedField;
    }
}
