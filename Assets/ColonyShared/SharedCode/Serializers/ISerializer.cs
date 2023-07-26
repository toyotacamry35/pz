using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.EntitySystem;

namespace SharedCode.Serializers
{
    public interface ISerializer
    {
        T Deserialize<T>(byte[] data);
        T Deserialize<T>(byte[] data, ref int offset);
        byte[] Serialize(byte[] buffer, ref int offset, object obj);
        byte[] Serialize<T>(T obj);
        T Merge<T>(T obj, byte[] data);
    }
}
