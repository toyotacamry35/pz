using Assets.Src.GameObjectAssembler.Res;
using Assets.Src.ResourcesSystem.Base;
using SharedCode.Aspects.Item.Templates;
using System.Collections.Generic;

namespace SharedCode.Entities
{
    public interface IHasDollDef
    {
        DefaultContainer DefaultDoll { get; set; }

        List<DefaultItemsStack> FirstRunDoll { get; set; }
    }
}

