namespace ResourceSystem.Entities
{
    /// <summary>
    /// Bound-размеры объекта. Используется:
    /// * Для валидации (но НЕ для определения) попадания по объекту в <see cref="GeneratedCode.DeltaObjects.AttackEngine"/>
    /// * ...
    /// Объект представлен вертикальной капсулой с заданным радиусом и высотой (расстояние от нижней точки нижней полусферы до верхней точки верхней полусферы).
    /// Центр капсулы смещён относительно позиции объекта вверх на половину высоты (т.е. нижняя точка капсулы совпадает с позицией объекта).
    /// </summary>
    public interface IHasBoundsDef
    {
        CapsuleDef Bounds { get; }
    }
}