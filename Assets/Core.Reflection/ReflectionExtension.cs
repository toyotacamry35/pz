using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Reflection
{
    public static class ReflectionExtension
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static Assembly[] GetAssembliesSafe(this AppDomain appDomain)
        {
            try
            {
                return appDomain.GetAssemblies();
            }
            catch (Exception) // AppDomainUnloadedException
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetAssemblies() return {ex.GetType().Name}, domain: {appDomain.FriendlyName}, messge: {ex.Message}").Write();
            }
            return Array.Empty<Assembly>();
        }

        public static Assembly[] GetAssembliesSafeNonStandard(this AppDomain appDomain)
        {
            try
            {
                return appDomain.GetAssemblies()
                    .Where(x=>!x.IsDynamic && 
                    !x.FullName.ToLower().StartsWith("mscorlib") && 
                    !x.FullName.ToLower().StartsWith("system") && 
                    !x.FullName.ToLower().StartsWith("nunit") && 
                    !x.FullName.ToLower().StartsWith("epplus") && 
                    !x.FullName.ToLower().StartsWith("unityengine") && 
                    !x.FullName.ToLower().StartsWith("unityeditor")).ToArray();
            }
            catch (Exception) // AppDomainUnloadedException
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetAssembliesSafeNonStandard() return {ex.GetType().Name}, domain: {appDomain.FriendlyName}, messge: {ex.Message}").Write();
            }
            return Array.Empty<Assembly>();
        }

        public static Type GetTypeSafe(this Assembly assembly, string name)
        {
            try
            {
                return assembly.GetType(name);
            }
            catch (Exception) // ArgumentException, ArgumentNullException, FileNotFoundException, FileLoadException, BadImageFormatException
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetType() return {ex.GetType().Name}, assembly: {assembly.FullName}, messge: {ex.Message}").Write();
            }
            return null;
        }

        public static Type[] GetTypesSafe(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (Exception) // ReflectionTypeLoadException, SecurityException
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetTypes() return {ex.GetType().Name}, assembly: {assembly.FullName}, messge: {ex.Message}").Write();
            }
            return Array.Empty<Type>();
        }

        public static MethodInfo[] GetMethodsSafe(this Type type, BindingFlags flags)
        {
            try
            {
                return type.GetMethods(flags);
            }
            catch (Exception) // no any documented exceptions
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetMethods() return {ex.GetType().Name}, type: {type.Name}, messge: {ex.Message}").Write();
            }
            return Array.Empty<MethodInfo>();
        }

        public static IEnumerable<T> GetCustomAttributesSafe<T>(this MemberInfo element, bool inherit) where T : Attribute
        {
            try
            {
                return element.GetCustomAttributes<T>(inherit);
            }
            catch (Exception) //ArgumentNullException, ArgumentException, NotSupportedException, TypeLoadException
            {
                //Logger.IfError()?.Message($"ReflectionExtension, GetCustomAttributes() return {ex.GetType().Name}, type: {element.DeclaringType.Name}, member: {element.Name}, inherit: {inherit}, messge: {ex.Message}").Write();
            }
            return Array.Empty<T>();
        }
    }
}
