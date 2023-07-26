using System;

namespace Assets.Test.Src.Editor
{
    public interface IBuilder
    {
        event Action BeforeBuild;
        event Action AfterBuild;
        
        int Build();
    }
}