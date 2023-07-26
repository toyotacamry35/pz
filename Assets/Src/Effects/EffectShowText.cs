using Assets.ColonyShared.SharedCode.GeneratedDefsForSpells;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using System.Threading.Tasks;
using JetBrains.Annotations;
using L10n;
using Uins;

namespace Assets.Src.Effects
{
    /// <summary>
    /// Фразы Гордона, сообщения о травмах
    /// </summary>
    [UsedImplicitly]
    class EffectShowText : IClientOnlyEffectBinding<EffectShowTextDef>
    {
        public ValueTask AttachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowTextDef def)
        {
            return ShowText(cast, repo, def, false);
        }

        public static async ValueTask ShowText(SpellWordCastData cast, IEntitiesRepository repo, EffectShowTextDef def, bool end)
        {
            var defClass = def;
            if (defClass == null)
                return;
            var entity = cast.Caster;
            using (var wrapper = await repo.Get(entity.TypeId, entity.Guid))
            {
                var rlevel = defClass.ShowForEveryone ? ReplicationLevel.ClientBroadcast : ReplicationLevel.ClientFull;
                var e = wrapper.Get<IEntity>(entity.TypeId, entity.Guid, rlevel);
                if (!cast.IsSlave || !cast.SlaveMark.OnClient || (e == null))
                    return;
            }
            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                if (!end || defClass.ShouldShowEnd)
                {
                    var ls = end ? defClass.TextEndLs : defClass.TextLs;
                    var message = ls.IsEmpty()
                        ? $"Empty {(end ? nameof(defClass.TextEndLs) : nameof(defClass.TextLs))} in {defClass}"
                        : ls.GetText();

                    if (def.IsError)
                        WarningMessager.Instance?.ShowErrorMessage(message);
                    else
                        WarningMessager.Instance?.ShowWarningMessage(message, defClass.Color, null, null, defClass.Color, false);
                }
            });
        }

        public ValueTask DetachOnClient(SpellWordCastData cast, IEntitiesRepository repo, EffectShowTextDef def)
        {
            return ShowText(cast, repo, def, true);
        }
    }
}