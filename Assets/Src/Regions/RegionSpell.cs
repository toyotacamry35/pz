using Assets.Src.ResourcesSystem.Base;
using Assets.Src.ResourceSystem;
using SharedCode.Aspects.Regions;
using SharedCode.Wizardry;
using System;
using UnityEngine;

namespace Assets.Src.Regions
{
    public class RegionSpell : MonoBehaviour
    {
        public JdbMetadata OnEnterSpell;
        public JdbMetadata OnExitSpell;
        public JdbMetadata WhileInsideSpell;

        internal SpellCastRegionDef GetSpellRegionDef()
        {
            bool added = false;
            SpellCastRegionDef spellCastRegionDef = new SpellCastRegionDef();
            if (OnEnterSpell)
            {
                var onEnterSpell = OnEnterSpell.Get<SpellDef>();
                if (onEnterSpell == null)
                    throw new Exception($"Wrong jdb type in {nameof(OnEnterSpell)} field of {nameof(RegionSpell)} component of {gameObject.name}");
                var onEnterDef = new ResourceRef<SpellDef>(onEnterSpell);
                spellCastRegionDef.OnEnterSpellDef = onEnterDef;
                added = true;
            }
            if (OnExitSpell)
            {
                var onExitSpell = OnExitSpell.Get<SpellDef>();
                if (onExitSpell == null)
                    throw new Exception($"Wrong jdb type in {nameof(OnExitSpell)} field of {nameof(RegionSpell)} component of {gameObject.name}");
                var onExitDef = new ResourceRef<SpellDef>(onExitSpell);
                spellCastRegionDef.OnExitSpellDef = onExitDef;
                added = true;
            }
            if (WhileInsideSpell)
            {
                var whileInsideSpell = WhileInsideSpell.Get<SpellDef>();
                if (whileInsideSpell == null)
                    throw new Exception($"Wrong jdb type in {nameof(whileInsideSpell)} field of {nameof(RegionSpell)} component of {gameObject.name}");
                var onStayDef = new ResourceRef<SpellDef>(whileInsideSpell);
                spellCastRegionDef.WhileInsideSpellDef = onStayDef;
                added = true;
            }

            return added ? spellCastRegionDef : default;
        }
    }
}
