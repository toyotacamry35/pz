using ResourcesSystem.Loader;
using System;

namespace Assets.Src.ResourceSystem
{
    public static class EditorGameResourcesForMonoBehaviours
    {
        public static Func<GameResources> NewGR;
        public static GameResources GetNew()
        {
            return NewGR();
        }
    }
}
