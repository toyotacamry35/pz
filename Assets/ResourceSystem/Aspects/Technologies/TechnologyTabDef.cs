using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace SharedCode.Aspects.Science
{
    [Localized]
    public class TechnologyTabDef : BaseResource
    {
        /// <summary>
        /// Название таба
        /// </summary>
        public LocalizedString TitleLs { get; set; }

        /// <summary>
        /// Список айтемов
        /// </summary>
        public TechnologyItem[] Items { get; set; }

        /// <summary>
        /// Шаг сетки X/Y, px
        /// </summary>
        public float GridPitch { get; set; }

        /// <summary>
        /// Расстояние от центра самого крайнего айтема технологии до края листа контента- X, px
        /// </summary>
        public float MarginX { get; set; }

        /// <summary>
        /// Расстояние от центра самого крайнего айтема технологии до края листа контента- Y, px
        /// </summary>
        public float MarginY { get; set; }
    }

    public struct TechnologyItem
    {
        public ResourceRef<TechnologyDef> Technology { get; set; }

        public bool IsDebug { get; set; }

        /// <summary>
        /// Координаты айтема технологии на листе таба, в относительных координатах шага сетки GridPitch.
        /// Точка отсчета - центр листа минус офсет листа (OffsetX, OffsetY)
        /// </summary>
        public float X { get; set; }

        public float Y { get; set; }

        public override string ToString()
        {
            return $"{nameof(TechnologyItem)}: ({X}, {Y}) {Technology.Target?.ToStringShort()}";
        }
    }
}