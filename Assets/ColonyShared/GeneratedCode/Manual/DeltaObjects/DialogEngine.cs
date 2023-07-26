using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedCode.Repositories;
using SharedCode.Entities;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.Refs;
using System;
using System.Threading.Tasks;
using ResourceSystem.Aspects.Dialog;

namespace GeneratedCode.DeltaObjects
{
    public partial class DialogEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public async Task<DialogDef> NextImpl(DialogDef nextDialog)
        {
            return nextDialog;
        }

        public async Task<DialogDef> NextWithCheckImpl(DialogDef nextDialog)
        {
            return nextDialog;
        }
    }
}
