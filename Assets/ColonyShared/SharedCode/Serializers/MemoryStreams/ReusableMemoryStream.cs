using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Serializers.MemoryStreams
{
    //только для внутреннего использования в сериализации
    public class ReusableMemoryStream: MemoryStream
    {
        //private static TypeAccessor _typeAccessor;
        private static Action<MemoryStream, bool> _exposableSet;
        private static Action<MemoryStream, bool> _expandableSet;
        private static Action<MemoryStream, bool> _isOpenSet;
        private static Action<MemoryStream, bool> _writableSet;
        private static Action<MemoryStream, byte[]> _bufferSet;
        private static Action<MemoryStream, int> _originSet;
        private static Action<MemoryStream, int> _positionSet;
        private static Action<MemoryStream, int> _lengthSet;
        private static Action<MemoryStream, int> _capacitySet;
        private static Action<MemoryStream, Task> _lastReadTaskSet;

        public static void Register()
        {
            _exposableSet = createSetter<MemoryStream, bool>(typeof(MemoryStream).GetField("_exposable", BindingFlags.NonPublic | BindingFlags.Instance));
            _expandableSet = createSetter<MemoryStream, bool>(typeof(MemoryStream).GetField("_expandable", BindingFlags.NonPublic | BindingFlags.Instance));
            _isOpenSet = createSetter<MemoryStream, bool>(typeof(MemoryStream).GetField("_isOpen", BindingFlags.NonPublic | BindingFlags.Instance));
            _writableSet = createSetter<MemoryStream, bool>(typeof(MemoryStream).GetField("_writable", BindingFlags.NonPublic | BindingFlags.Instance));
            _bufferSet = createSetter<MemoryStream, byte[]>(typeof(MemoryStream).GetField("_buffer", BindingFlags.NonPublic | BindingFlags.Instance));
            _originSet = createSetter<MemoryStream, int>(typeof(MemoryStream).GetField("_origin", BindingFlags.NonPublic | BindingFlags.Instance));
            _positionSet = createSetter<MemoryStream, int>(typeof(MemoryStream).GetField("_position", BindingFlags.NonPublic | BindingFlags.Instance));
            _lengthSet = createSetter<MemoryStream, int>(typeof(MemoryStream).GetField("_length", BindingFlags.NonPublic | BindingFlags.Instance));
            _capacitySet = createSetter<MemoryStream, int>(typeof(MemoryStream).GetField("_capacity", BindingFlags.NonPublic | BindingFlags.Instance));
            _lastReadTaskSet = createSetter<MemoryStream, Task>(typeof(MemoryStream).GetField("_lastReadTask", BindingFlags.NonPublic | BindingFlags.Instance));
        }
        static Action<S, T> createSetter<S, T>(FieldInfo field)
        {
            string methodName = field.ReflectedType.FullName + ".set_" + field.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(S), typeof(T) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, field);
            gen.Emit(OpCodes.Ret);
            return (Action<S, T>)setterMethod.CreateDelegate(typeof(Action<S, T>));
        }

        public ReusableMemoryStream()
            : base()
        {
        }

        public ReusableMemoryStream(byte[] buffer, int index, int count, bool writable) 
            : base(buffer, index, count, writable)
        {
        }

        public void SetBuffer(byte[] buffer, int index, int count, bool writable)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid offset");

            _isOpenSet(this, true);
            _exposableSet(this, false);
            _expandableSet(this, false);
            _bufferSet(this, buffer);
            _originSet(this, index);
            _positionSet(this, index);
            _lengthSet(this, index + count);
            _capacitySet(this, index + count);
            _writableSet(this, writable);
        }

        public void ReleaseBuffer()
        {
            _bufferSet(this, null);
            _writableSet(this, false);
            _isOpenSet(this, false);
            _lastReadTaskSet(this, null);
        }
    }
}
