using System.Collections.Generic;
using ColonyShared.SharedCode.InputActions;

namespace Assets.Src.InteractionSystem
{
    //Временный компонент показа, что должно подбирается

    public class SpellDescriptions
    {
        public Dictionary<InputActionDef, SpellDescription> Descriptions = new Dictionary<InputActionDef, SpellDescription>();
    }
}