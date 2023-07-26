using System.IO;

namespace Assets.Src.ResourcesSystem.Base
{
    public interface IBinarySerializable
    {
        void WriteToStream(Stream stream);

        void ReadFromStream(Stream stream);
    }
}