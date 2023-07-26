using Assets.ResourceSystem.Aspects.Banks;
using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using System.Collections.Generic;

namespace SharedCode.Entities
{
    public interface IBankDef
    {
        ResourceRef<BankDef> BankDef { get; set; }
    }
}

