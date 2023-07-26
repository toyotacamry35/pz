using Assets.Src.ResourcesSystem.Base;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System;
using System.CommandLine.Parsing;
using System.Linq;

namespace Core.Environment.CommandLine
{
    public static class NodeBootstrapper
    {
        private static bool ConvertResourceRef(ArgumentResult symbol, out ResourceIDFull resId)
        {
            resId = ResourceIDFull.Parse(symbol.Tokens.Single().Value);
            return true;
        }

        public static Task<int> Run(string[] args, Func<ResourceIDFull, ResourceIDFull, int, ResourceIDFull, bool, DirectoryInfo, Task> action)
        {
            var command = new RootCommand
            {
                new Option(new[]{ "--container-config" }) { Required = true, Argument = new Argument<ResourceIDFull>(ConvertResourceRef){ Arity = ArgumentArity.ExactlyOne} },
                new Option(new[]{ "--shared-config" }) { Required = true, Argument = new Argument<ResourceIDFull>(ConvertResourceRef){ Arity = ArgumentArity.ExactlyOne} },
                new Option(new[]{ "-rr", "--resource-system-root"}) { Required = false, Argument = new Argument<DirectoryInfo>(){ Arity = ArgumentArity.ExactlyOne }.ExistingOnly() },
                new Option(new[]{ "--watch-pid" }) { Required = false, Argument = new Argument<int>(){ Arity = ArgumentArity.ExactlyOne } },
                new Option(new[]{ "--realms-list-override" }) { Required = false, Argument = new Argument<ResourceIDFull>(ConvertResourceRef){ Arity = ArgumentArity.ExactlyOne } },
                new Option(new[]{ "--call-platform" }) { Required = false, Argument = new Argument<bool>(){ Arity = ArgumentArity.ExactlyOne } },

                new Option(new[]{ "--cluster" }, "Cluster node mode, as opposite to client") { Required = false, Argument = new Argument<bool>(){ Arity = ArgumentArity.ZeroOrOne } },
                new Option(new[]{ "-logFile" }, "Unused, for unity compatibility") { Required = false, Argument = new Argument<string>(){ Arity = ArgumentArity.ExactlyOne } },
            };
            command.TreatUnmatchedTokensAsErrors = false;

            command.Handler = CommandHandler.Create(action);
            return command.InvokeAsync(args);
        }
    }
}
