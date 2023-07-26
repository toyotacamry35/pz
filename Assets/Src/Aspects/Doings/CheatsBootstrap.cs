using Assets.Src.Lib.Cheats;
using Assets.Src.SpawnSystem;
using Core.Cheats;
using System;
using Utilities;

namespace Assets.Src.Aspects.Doings
{
    class CheatsBootstrap : EntityGameObjectComponent
    {
        protected override void GotClient()
        {
            base.GotClient();
            var cheatIndex = Environment.GetCommandLineArgs().IndexOf(x => x.StartsWith("-cheat"));
            if (cheatIndex != -1)
            {
                var cheatArg = Environment.GetCommandLineArgs()[cheatIndex + 1];
                CheatsManager.ExecuteCommand(cheatArg).AsTask().WrapErrors();
            }
        }
    }
}
