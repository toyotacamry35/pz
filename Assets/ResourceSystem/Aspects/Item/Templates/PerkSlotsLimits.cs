using System;
using Assets.Src.ResourcesSystem.Base;

namespace SharedCode.Aspects.Item.Templates
{
    /// <summary>
    /// Ограничение на число слотов (по типам) в коллекции перманентных перков
    /// </summary>
    public class PerkSlotsLimits : BaseResource
    {
        public int[] Limits;
    }
}