using System;
using SharedCode.Wizardry;

namespace ResourceSystem.Aspects.ManualDefsForSpells
{
    public class EffectDebugLagDef : SpellEffectDef
    {
        public bool OnFinish;
        public int Delay;
        public Side Where;

        [Flags]
        public enum Side
        {
            Client = 0x1,
            ClientWithAuthority = 0x2,
            ServerSlave = 0x4,
            ServerMaster = 0x8,
            All = 0xFFFFFFF
        };
    }
}