using SharedCode.Utils;
using Mathf = Assets.ColonyShared.SharedCode.Utils.SharedHelpers;

namespace Src.Locomotion
{
    public interface ILocomotionInputProvider
    {
        /// <summary>
        /// Текущее значение оси
        /// </summary>
        float this[InputAxis it] { get; }

        /// <summary>
        /// Текущее значение пары осей
        /// </summary>
        Vector2 this[InputAxes it] { get; }
        
        /// <summary>
        /// Текущее значение триггера
        /// </summary>
        bool this[InputTrigger it] { get; }

        int HistoryCount { get; }
        
        InputFrame History(int idx);
    }

    public static class LocomotionInputProviderExtensions
    {

        /// <summary>
        /// Максимальное по модулю значение оси за последние time секунд
        /// </summary>
        public static float Max(this ILocomotionInputProvider input, InputAxis it, float time)
        {
            float max = 0, maxAbs = 0;
            var itr = new InputAxisEnumerator(input, it, time, 0);
            while(itr.MoveNext())
                if (Mathf.Abs(itr.Current) > maxAbs)
                    maxAbs = Mathf.Abs(max = itr.Current);
            return max;
        }

        /// <summary>
        /// Сркднее значение оси за последние time секунд
        /// </summary>
        public static float Avg(this ILocomotionInputProvider input, InputAxis it, float time)
        {
            int i = 0;
            float value = 0;
            var itr = new InputAxisEnumerator(input, it, time, 0);
            while (itr.MoveNext())
            {
                value += itr.Current;
                ++i;
            }
            return i > 0 ? value / i : 0;
        }

        /// <summary>
        /// Последнее по времени значение осей с максимальным модулем 
        /// </summary>
        public static Vector2 Max(this ILocomotionInputProvider input, InputAxes it, float time)
        {
            float maxMag = 0;
            Vector2 max = Vector2.zero;
            var itr = new InputAxesEnumerator(input, it, time, 0);
            while(itr.MoveNext())
                if (itr.Current.sqrMagnitude > maxMag)
                    maxMag = (max = itr.Current).sqrMagnitude;
            return max;
        }

        /// <summary>
        /// Среднее значение оси за последние time секунд
        /// </summary>
        public static Vector2 Avg(this ILocomotionInputProvider input, InputAxes it, float time)
        {
            int i = 0;
            var value = Vector2.zero;
            var itr = new InputAxesEnumerator(input, it, time, 0);
            while(itr.MoveNext())
            {
                value += itr.Current;
                ++i;
            }
            return i > 0 ? value / i : value;
        }
    }
}
