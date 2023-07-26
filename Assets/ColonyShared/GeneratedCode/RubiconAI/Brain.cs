using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.RubiconAI
{
    public abstract class Brain
    {
        public abstract ValueTask Init(MobLegionary hostLegionary);
        public abstract ValueTask Think();

        public virtual void OnIMGUI()
        {
        }

        public virtual bool DemandsUpdate => false;
    }
}
