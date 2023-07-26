using System;

namespace ResourcesSystem.Base
{
    public interface IHasRandomFill
    {
        void Fill(int depthCount, Random random, bool withReadonly = true);
    }

    public interface ICanRandomFill
    {
        void Fill(int depthCount, bool withReadonly = true);
    }
}
