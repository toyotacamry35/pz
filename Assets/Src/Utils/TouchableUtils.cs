using ReactivePropsNs.Touchables;
using SharedCode.EntitySystem;

namespace Assets.Src.Utils
{
    public static class TouchableUtils
    {
        public static TouchableEgoProxy<T> CreateEgoProxy<T>() where T : class, IEntity
        {
            return new TouchableEgoProxy<T>(UnityEntityTouchContainerFactory<T>.Instance);
        }
    }
}