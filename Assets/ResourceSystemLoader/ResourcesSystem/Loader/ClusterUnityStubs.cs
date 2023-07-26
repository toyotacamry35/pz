#if !UNITY_5_3_OR_NEWER

using ResourcesSystem.Loader;

namespace UnityEngine
{
    public class Object
    {
    }

    public class Component : Object
    {
    }

    public class Behaviour : Component
    {
    }

    public class MonoBehaviour : Behaviour
    {

    }
    public struct Matrix4x4
    {
        public float this[int x, int y]
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }
    }

    public class GameObject : Object
    {
    }


    public class TextAsset : Object
    {
    }

    public class Sprite : Object
    {
    }

    public class Material : Object
    {
    }

    public class AudioClip : Object
    {
    }

    public class Texture2D : Object
    {
    }

    public class Texture : Object
    {
    }

    public class FxData : Object
    {
    }

    public class PositionAndRotationYContextValue : Object
    {
    }
}

namespace Assets.Src.Aspects.Impl
{
    [KnownToGameResources]
    public class SpellComponent : UnityEngine.Object
    {
    }

}

namespace Assets.Src.Inventory
{
    [KnownToGameResources]
    public class ItemResourceVisual : UnityEngine.Object
    {
    }
}
#endif
