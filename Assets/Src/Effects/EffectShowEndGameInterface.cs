using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using JetBrains.Annotations;
using ReactivePropsNs;
using ResourceSystem.GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Logging;
using Uins;

namespace Src.Effects
{
    [UsedImplicitly]
    public class EffectShowEndGameInterface : IClientOnlyEffectBinding<EffectShowEndGameInterfaceDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowEndGameInterfaceDef def)
        {
            if (cast.OnClientWithAuthority())
            {
                var causer = cast.WordGlobalCastId(def);
                UnityQueueHelper.RunInUnityThreadNoWait(() =>
                    {
                        EndGameScreenNode.Instance.Show(causer, def.TitleLs, def.TextLs, def.Image?.Target);
                        var disposables = new DisposableComposite();
                        GameState.Instance.IsInGameRp.Action(disposables,
                            (ingame) =>
                            {
                                Log.Logger.IfDebug()?.Message($"InGame:{ingame}").Write();                                
                                if (!ingame)
                                {
                                    EndGameScreenNode.Instance?.Hide(cast.WordGlobalCastId(def));
                                    disposables.Dispose();
                                }
                            });
                    });
            }
            return new ValueTask();
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowEndGameInterfaceDef def)
        {
            // if (cast.OnClientWithAuthority())
            //     EndGameScreenNode.Instance?.Hide(cast.WordGlobalCastId(def));
            return new ValueTask();
        }
    }
}