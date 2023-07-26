using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCode.ActorServices;
using SharedCode.Serializers;
using System.Reflection;
using SharedCode.Serializers.Protobuf;
using Core.Reflection;

namespace SharedCode.OurSimpleIoC
{
    //TODO: actually collect services instead of this hacky bullshit
    class ServicesProvider : IServices
    {
        public ServicesProvider()
        {
            _services = new Dictionary<Type, object>()
            {
            {typeof(ISerializer), new ProtoBufSerializer() },
            };

            foreach(var type in Assembly.GetExecutingAssembly().GetTypesSafe())
            {
                var ser = type.GetCustomAttribute<ServiceAttribute>();

                if (ser != null)
                {
                    if (_services.ContainsKey(ser.InterType))
                        throw  new Exception(string.Format("ServiceAttribute: Already contains {0}", ser.InterType));

                    _services.Add(ser.InterType, Activator.CreateInstance(type));
                }
            }
            foreach (var service in _services)
                Inject(service.Value);

        }
        Dictionary<Type, object> _services;

        object _locker = new object();

        public T Get<T>(object context = null)
        {
            object result;
            if (_services.TryGetValue(typeof(T), out result))
                return (T)result;
            return default(T);
        }

        public void Register<T>(object service)
        {
            lock (_locker)
                _services.Add(typeof(T), service);
        }

        public void Inject(object target)
        {
            foreach(var field in target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic ))
            {
                if (_services.ContainsKey(field.FieldType))
                    field.SetValue(target, _services[field.FieldType]);
            }
            foreach (var field in target.GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (_services.ContainsKey(field.FieldType))
                    field.SetValue(null, _services[field.FieldType]);
            }
        }
    }
    public class ServiceAttribute : Attribute
    {
        public Type InterType { get; }
        public ServiceAttribute(Type interfaceType)
        {
            InterType = interfaceType;
        }
    }
}
