using SharedCode.EntitySystem;

namespace SharedCode.Aspects.Science
{
    public interface IKnowledge : IDeltaObject
    {
        KnowledgeDef Def { get; set; }

        /// <summary>
        /// Если находится в списке, значит уже активировано, а этот параметр - 
        /// активность, которая может быть вызвана блокировкой связанной технологии
        /// </summary>
        bool IsAvailable { get; set; }
    }
}