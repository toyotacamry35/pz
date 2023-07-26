using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

namespace SharedCode.Entities.GameObjectEntities
{
    //Очень сумрачно, но иначе оно безумно медленно, а какого-то дизайна как у нас делать бинарные ресурсы сейчас нет
    public class BinaryConverter<T> : JsonConverter where T : struct
    {
        public BinaryConverter()
        {
            size = Marshal.SizeOf(default(T));
            arr = new byte[size];
            ptr = Marshal.AllocHGlobal(size);
        }

        ~BinaryConverter()
        {
            Marshal.FreeHGlobal(ptr);
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
        IntPtr ptr;
        int size;
        byte[] arr;
        byte[] GetBytes(T str)
        {
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            return arr;
        }
        T FromBytes(byte[] array)
        {
            T str = new T();
            Marshal.Copy(array, 0, ptr, size);
            str = (T)Marshal.PtrToStructure(ptr, typeof(T));
            return str;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = (string)reader.Value;
            var bytes = Convert.FromBase64String(str);
            return FromBytes(bytes);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bytes = GetBytes((T)value);
            var str = Convert.ToBase64String(bytes);
            writer.WriteValue(str);
        }
    }
}
