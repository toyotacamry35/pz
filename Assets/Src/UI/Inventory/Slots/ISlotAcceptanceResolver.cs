using UnityEngine;

namespace Uins.Slots
{
    public interface ISlotAcceptanceResolver
    {
        /// <summary>
        /// Возвращает число айтемов, которое можно перенести из fromSvm в toSvm
        /// </summary>
        /// <param name="doMove">Следует ли провести операцию или только посчитать</param>
        /// <param name="isCounterSwapCheck">Флаг дополнительной обратной проверки: сколько айтемов ячейки B (здесь это все равно fromSvm) 
        /// можно положить в ячейку A (toSvm). В отличие от прямой проверки: сколько айтемов ячейки A (fromSvm) можно положить в ячейку B (toSvm).
        /// При положительном результате прямой проверки, если toSvm не пуста, делаем обратную проверку:
        ///  fromSvm.SlotAcceptanceResolver.TryMoveTo(toSvm, fromSvm, false, true)</param>
        int TryMoveTo(SlotViewModel fromSvm, SlotViewModel toSvm, bool doMove = false, bool isCounterSwapCheck = false);

        /// <summary>
        /// Пытается выбросить на землю содержимое слота fromSvm
        /// </summary>
        bool TryToDropFrom(SlotViewModel fromSvm);
    }
}