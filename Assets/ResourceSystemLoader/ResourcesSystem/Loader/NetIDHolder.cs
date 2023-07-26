using System;
using System.Collections.Generic;
using System.Linq;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using Core.Environment.Logging.Extension;
using NLog;

namespace ResourcesSystem.Loader
{
    public class NetIDHolder
    {
        private readonly IReadOnlyDictionary<ulong, string> _netIDToFile;
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public NetIDHolder(ILoader loader)
        {
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();
            var loaderAllPossibleRoots = loader.AllPossibleRoots;
            HashSet<string> duplicatesChecker = new HashSet<string>();
            foreach(var possibleRoot in loaderAllPossibleRoots)
                if(!duplicatesChecker.Add(possibleRoot))
                    throw new Exception($"Error Duplicate Resources Found! {string.Join(",", possibleRoot)}");
            _netIDToFile = loaderAllPossibleRoots.ToDictionary(x => Crc64.Compute(x));
            w.Stop();
            Logger.IfInfo()?.Message($"NetIdHolder finished in {w.Elapsed.TotalMilliseconds}").Write();
        }

        public ResourceIDFull GetID(ulong rootID, int line, int col, int protoIndex = 0)
        {
            String rootPath = null;
            if (!_netIDToFile.TryGetValue(rootID, out rootPath))
                return default(ResourceIDFull);
            return new ResourceIDFull(rootPath, line, col, protoIndex);
        }
    }
}