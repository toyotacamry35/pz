using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Src.RubiconAI
{
    public class LegionDef : BaseResource
    {
        HashSet<LegionDef> _kin;
        public ResourceRef<LegionDef>[] Kin { get; set; } = Array.Empty<ResourceRef<LegionDef>>();
        // Is not using now (mostly 'cos of AI depends on WHO it controls)
        public ResourceRef<LegionaryDef> LegionaryDefOverride { get; set; }
        //This method is allowed in a Def class as it only uses other defs and not any gameplay code
        public bool IsOfMyKin(LegionDef def)
        {
            if (def == null)
                return false;
            if (def == this)
                return true;
            if (_kin == null && Kin.Length == 0)
                return false;
            if (_kin == null)
                _kin = new HashSet<LegionDef>(Kin.SelectMany(x => x.Target.Kin.Select(y => y.Target)));
            if (_kin.Contains(def))
                return true;
            return false;
        }
    }
}
