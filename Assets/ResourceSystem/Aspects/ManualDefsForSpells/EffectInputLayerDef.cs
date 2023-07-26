using System.Collections.Generic;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode.InputActions;
using SharedCode.Wizardry;

namespace ColonyShared.SharedCode.Aspects.ManualDefsForSpells
{
    public class EffectInputLayerDef : SpellEffectDef
    {
        public const string DefaultLayer = "/UtilPrefabs/Input/Layers/Spell";
        
        public ResourceRef<InputActionHandlersLayerDef> Layer = new ResourceRef<InputActionHandlersLayerDef>(DefaultLayer);
        public Dictionary<ResourceRef<InputActionDef>,ResourceRef<InputActionHandlerDef>> Handlers;
    }
}