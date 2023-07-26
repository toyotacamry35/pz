using Assets.Src.ResourcesSystem.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Src.Character.Events
{
    public class FXEventsDef : BaseResource
    {
        public List<ResourceRef<TriggerFXRuleDef>> FXEvents { get; set; } = new List<ResourceRef<TriggerFXRuleDef>>();
    }
}
