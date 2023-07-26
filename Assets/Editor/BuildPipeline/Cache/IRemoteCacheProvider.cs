namespace Assets.Test.Src.Editor
{
    public interface IRemoteCacheProvider
    {
        bool Check(string name);
        void Clean();
        string Download(string name);
        void Upload(string name, string serialized);
    }
}