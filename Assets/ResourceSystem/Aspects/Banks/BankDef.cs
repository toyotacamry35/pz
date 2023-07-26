using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.ResourceSystem.Aspects.Banks
{
    public class BankDef : SaveableBaseResource
    {
        public int InitialSize { get; set; }
        public int MaximumSize { get; set; }
    }
}
