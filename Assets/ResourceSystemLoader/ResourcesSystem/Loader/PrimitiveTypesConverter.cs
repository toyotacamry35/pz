using System;

namespace ResourcesSystem.Loader
{
    internal static class PrimitiveTypesConverter
    {
        internal static object Convert(object v, Type objectType)
        {

            if (v == null)
            {
                if (objectType == typeof(string))
                    return null;
                if (objectType == typeof(object))
                    return null;
                return Activator.CreateInstance(objectType);
            }
            if (objectType == typeof(object))
                return v;
            if (v is double && objectType == typeof(float))
                return (float)(double)v;
            if (v is int && objectType == typeof(float))
                return (float)(int)v;
            if (v is float && objectType == typeof(double))
                return (double)(float)v;
            if (v is float && objectType == typeof(int))
                return (int)(float)v;
            if (v is float && objectType == typeof(Int64))
                return (Int64)(float)v;
            if (v is int && objectType == typeof(Int64))
                return (Int64)(int)v;
            if (v is long && objectType == typeof(int))
                return (int)(Int64)v;
            if (v is long && objectType == typeof(float))
                return (float)(Int64)v;
            return v;
        }
        internal static bool CanConvert(Type fromType, Type objectType)
        {
            if (fromType == typeof(double) && objectType == typeof(float))
                return true;
            if (fromType == typeof(int) && objectType == typeof(float))
                return true;
            if (fromType == typeof(float) && objectType == typeof(double))
                return true;
            if (fromType == typeof(float) && objectType == typeof(int))
                return true;
            if (fromType == typeof(float) && objectType == typeof(Int64))
                return true;
            if (fromType == typeof(int) && objectType == typeof(Int64))
                return true;
            if (fromType == typeof(Int64) && objectType == typeof(int))
                return true;
            if (fromType == typeof(Int64) && objectType == typeof(float))
                return true;
            return false;
        }

    }

}
