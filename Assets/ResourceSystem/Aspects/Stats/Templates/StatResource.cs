using Assets.Src.ResourcesSystem.Base;
using L10n;

namespace Assets.Src.Aspects.Impl.Stats
{
    [Localized]
    public class StatResource : SaveableBaseResource
    {
        public string DebugName => ____GetDebugRootName(); // for debug purpose only!
        
        public LocalizedString StatNameLs { get; set; }

        /// <summary>
        /// Формат отображения значения стата: Value.ToString(statResource.ValueFormat);
        /// </summary>
        public string ValueFormat { get; set; }

        /// <summary>
        /// Отображение локализованных единиц измерения после значения, если они нужны.
        /// Внимание! LocalizedString единиц измерения должна включать плейсхолдер значения, т.е. выглядеть примерно так "{0} м/сек"
        /// </summary>
        public LocalizedString[] ValueUnitsLs { get; set; }

        public float OptionalSummand { get; set; }

        public bool DontShow { get; set; }
    }
}