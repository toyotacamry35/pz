using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Reflection;
using JetBrains.Annotations;
using NLog;
using NLog.Fluent;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;
using Core.Environment.Logging.Extension;

namespace Core.Cheats
{
    public static class CheatsManager
    {
        [NotNull] private static readonly NLog.Logger Logger = LogManager.GetLogger("Cheats");

        private static readonly IReadOnlyDictionary<string, MethodInfo> _commandsByName;

        static CheatsManager()
        {
            try
            {
                var cheatAttrType = typeof(CheatAttribute);
                var cheatMethods = (from assembly in AppDomain.CurrentDomain.GetAssembliesSafe()
                                   from type in assembly.GetTypesSafe()
                                   let methods = type.GetMethodsSafe(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
                                   from method in methods
                                   where method.IsDefined(cheatAttrType)
                                   select method).ToArray();

                var cheatMethodGroups = (from method in cheatMethods
                                        group method by method.Name.ToLower() into grp
                                        select grp).ToArray();

                var validCommands = from grp in cheatMethodGroups
                                    where !grp.Skip(1).Any()
                                    select grp.Single();

                var invalidCommands = from grp in cheatMethodGroups
                                      where grp.Skip(1).Any()
                                      select grp;

                _commandsByName = validCommands.ToImmutableDictionary(v => v.Name.ToLower());

                foreach (var grp in invalidCommands)
                {
                    var cmdList = grp.Select(v => v.ToString());
                    var str = string.Join(", ", cmdList);
                    Logger.IfError()?.Message("Command name collision: {0}, multiple variants: {1}", grp.Key, str).Write();
                }
            }
            catch (Exception e)
            {
                Logger.IfError()?.Message(e, "CheatsManager exception").Write();;
                _commandsByName = ImmutableDictionary.Create<string, MethodInfo>();
            }
        }



        public static IEnumerable<MethodInfo> TryAutocompleteCommand(string pieceOfCommand)
        {
            if (string.IsNullOrEmpty(pieceOfCommand))
                return Enumerable.Empty<MethodInfo>();

            string lowerPiece = pieceOfCommand.Trim().ToLower();

            if (string.IsNullOrEmpty(lowerPiece))
                return Enumerable.Empty<MethodInfo>();


            return _commandsByName.Where(v => v.Key.Contains(lowerPiece)).Select(v => v.Value);
        }

        public static MethodInfo MatchCommand(string command)
        {
            _commandsByName.TryGetValue(command.ToLower(), out var methodInfo);
            return methodInfo;
        }

        private static Regex SplitterRegex = new Regex("\"(.+?)\"|(\\S+)");

        public static (ExecuteCheatResult result, MethodInfo command, string[] splitCommand) ParseCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return (ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatParseError, "Command is null"), null, Array.Empty<string>());

            var splitCommand = SplitterRegex.Matches(command);

            if (splitCommand.Count == 0)
                return (ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatParseError, "Failed to parse command"), null, Array.Empty<string>());

            var lowerCmd = string.IsNullOrWhiteSpace(splitCommand[0].Groups[1].Value) ? splitCommand[0].Groups[2].Value : splitCommand[0].Groups[1].Value;

            if (!_commandsByName.TryGetValue(lowerCmd.ToLower(), out var methodInfo))
                return (ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.CheatNotFound, $"Cant find command {lowerCmd}"), null, Array.Empty<string>());

            var args = splitCommand.Cast<Match>().Skip(1).Select(v => string.IsNullOrEmpty(v.Groups[1].Value) ? v.Groups[2].Value : v.Groups[1].Value).ToArray();

            return (ExecuteCheatResult.Success(""), methodInfo, args);
        }

        public static ValueTask<ExecuteCheatResult> ExecuteCommand(string command, object instance = null)
        {
            (var parseResult, var methodInfo, var args) = ParseCommand(command);

            if (!parseResult.IsSuccess())
                return new ValueTask<ExecuteCheatResult>(parseResult);

            return ExecuteParsed(methodInfo, args, instance);
        }

        public static ValueTask<ExecuteCheatResult> ExecuteParsed(MethodInfo command, string[] args, object instance = null)
        {
            ParameterInfo[] parametersInfo = command.GetParameters();
            var firstParamWithDefault = parametersInfo.Select((param, i) => (param, i)).FirstOrDefault(v => v.param.HasDefaultValue);

            var minParams = firstParamWithDefault.i;
            var maxParams = parametersInfo.Length;

            if (args.Length < minParams || args.Length > maxParams)
                return new ValueTask<ExecuteCheatResult>(ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.ArgumentsCountError, $"Wroung count of arguments. Got {args.Length}, Expected {minParams}-{maxParams}"));

            try
            {
                var converted = parametersInfo.Select((v, i) =>
                {
                    if (i < args.Length)
                    {
                        var strVal = args[i];
                        return ArgumentConverter.Convert(strVal, v);
                    }

                    return v.DefaultValue;
                }).ToArray();

                var retVal = command.Invoke(instance, converted.ToArray());
                return TryWait(retVal, command);
            }
            catch (Exception e)
            {
                return new ValueTask<ExecuteCheatResult>(ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.ExecutionError, $"Cheat exception: {e}"));
            }
        }

        private static class TaskConverter<T>
        {
            public static async ValueTask<ExecuteCheatResult> WaitForValueTask(ValueTask<T> task)
            {
                try
                {
                    var res = await task;
                    return ExecuteCheatResult.Success(res);
                }
                catch (Exception e)
                {
                    return ExecuteCheatResult.Fail(ExecuteCheatResult.StatusCode.ExecutionError, $"Cheat exception: {e}");
                }
            }

            public static ValueTask<ExecuteCheatResult> WaitForTask(Task<T> task) => WaitForValueTask(new ValueTask<T>(task));
        }

        private static async ValueTask<ExecuteCheatResult> TryWait(object obj, MethodInfo method)
        {
            if(method.ReturnType.IsGenericType)
            {
                if(method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    var type = typeof(TaskConverter<>).MakeGenericType(method.ReturnType.GetGenericArguments().Single());
                    var waitMethod = type.GetMethod(nameof(TaskConverter<object>.WaitForTask), BindingFlags.Public | BindingFlags.Static);
                    var res = (ValueTask<ExecuteCheatResult>)waitMethod.Invoke(null, new[] { obj });
                    return await res;
                }
                
                if (method.ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    var type = typeof(TaskConverter<>).MakeGenericType(method.ReturnType.GetGenericArguments().Single());
                    var waitMethod = type.GetMethod(nameof(TaskConverter<object>.WaitForValueTask), BindingFlags.Public | BindingFlags.Static);
                    var res = (ValueTask<ExecuteCheatResult>)waitMethod.Invoke(null, new[] { obj });
                    return await res;
                }
            }

            if (method.ReturnType == typeof(Task))
            {
                await (Task)obj;
                return ExecuteCheatResult.Success("");
            }
            if (method.ReturnType == typeof(ValueTask))
            {
                await (ValueTask)obj;
                return ExecuteCheatResult.Success("");
            }
            return ExecuteCheatResult.Success(obj);
        }

        [Cheat]
        public static void Help()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var methodInfo in _commandsByName)
            {
                builder.Append(methodInfo.Key);
                foreach (var parameterInfo in methodInfo.Value.GetParameters())
                {
                    builder.Append(" ").Append(parameterInfo.ParameterType.Name).Append(" ").Append(parameterInfo.Name);
                }
                builder.AppendLine();
            }
            Logger.IfInfo()?.Message(builder.ToString()).Write();
        }
    }

    public interface ICheatArgConverter
    {
        object Convert(string value, ParameterInfo parameter);
        bool CanConvert(Type targetType);
    }
    
    internal class EnumStringConverter : ICheatArgConverter
    {
        public object Convert(string value, ParameterInfo parameter)
        {
            var names= parameter.ParameterType.GetEnumNames();
            var values= parameter.ParameterType.GetEnumValues();
            for (int i = 0; i < names.Length; i++)
            {
                if (string.Equals(names[i], value, StringComparison.OrdinalIgnoreCase))
                {
                    return values.GetValue(i);
                }
            }
            
            throw new Exception($"Couldn't cast value={value} to enum {parameter.ParameterType}");
        }

        public bool CanConvert(Type targetType) => targetType.IsEnum;
        
    }

    internal class BoolConverter : ICheatArgConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(bool);

        public object Convert(string value, ParameterInfo parameter)
        {
            if (string.Compare(value, "0", true) == 0 || string.Compare(value, "false", true) == 0)
                return false;
            if (string.Compare(value, "1", true) == 0 || string.Compare(value, "true", true) == 0)
                return true;

            throw new ArgumentException($"Cant convert string '{value}' to {parameter.ParameterType} {parameter.Name}", parameter.Name);
        }
    }

    internal class GuidConverter : ICheatArgConverter
    {
        public bool CanConvert(Type targetType) => targetType == typeof(Guid);

        public object Convert(string value, ParameterInfo parameter)
        {
            if(Guid.TryParse(value, out var result))
                return result;
            throw new ArgumentException($"Cant convert string '{value}' to {parameter.ParameterType} {parameter.Name}", parameter.Name);
        }
    }

    internal static class ArgumentConverter
    {
        private static ICheatArgConverter[] _converters { get; }

        static ArgumentConverter()
        {
            var converters = from assembly in AppDomain.CurrentDomain.GetAssembliesSafe()
                             from type in assembly.GetTypesSafe()
                             where typeof(ICheatArgConverter).IsAssignableFrom(type)
                             where !type.IsAbstract
                             let ctor = type.GetConstructor(Array.Empty<Type>())
                             where ctor != null
                             let instance = Activator.CreateInstance(type) as ICheatArgConverter
                             where instance != null
                             select instance;

            _converters = converters.ToArray();
        }

        public static object Convert(string parameter, ParameterInfo param)
        {
            var converter = _converters.FirstOrDefault(v => v.CanConvert(param.ParameterType));
            if (converter != null)
                return converter.Convert(parameter, param);
            try
            {
                return System.Convert.ChangeType(parameter, param.ParameterType, CultureInfo.InvariantCulture);
            }
            catch(Exception e)
            {
                throw new ArgumentException($"Cant convert string '{parameter}' to {param.ParameterType} {param.Name}", param.Name, e);
            }
        }

    }//class ArgumentConverter

}//ns