using SharedCode.EntitySystem;

namespace SharedCode.Aspects.Science
{
    public interface ITechnology : IDeltaObject
    {
        /// <summary>
        /// Если находится в списке, значит уже активирована, а этот параметр - активность, 
        /// которая может быть блокирована отсутствием необходимого уровня наук
        /// </summary>
        bool IsAvailable { get; set; }

        TechnologyDef Def { get; set; }
    }
}