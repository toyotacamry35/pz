using Assets.Src.Wizardry;
using GeneratedCode.DeltaObjects;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using System.Threading.Tasks;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using JetBrains.Annotations;
using SharedCode.Wizardry;
using Uins;
using UnityEngine;

namespace Assets.Src.Effects.UIEffects
{
    [UsedImplicitly]
    public class EffectOpenUIDialog : IEffectBinding<EffectOpenUIDialogDef>
    {
        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUIDialogDef def)
        {
            if (!cast.IsSlave || !cast.SlaveMark.OnClient)
                return;

            var casterRef = await def.Caster.Target.GetOuterRef(cast, repo);
            var characterID = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (casterRef.Guid != characterID)
                return;

            ISpellDoer spellDoer;
            using (var cnt = await repo.Get(casterRef))
            {
                var hasWizard = cnt.Get<IHasWizardEntityClientFull>(casterRef, ReplicationLevel.ClientFull);
                spellDoer = hasWizard.SlaveWizardHolder.SpellDoer;
            }
            
            GameObject targetGameObject = def.Target.Target.GetGo(cast);
            GameObject casterGameObject = def.Caster.Target.GetGo(cast);

            UnityQueueHelper.RunInUnityThreadNoWait(async () =>
            {
                var dialogOpponentBadgePoint = targetGameObject.GetComponentInChildren<DialogOpponentBadgePoint>();
                var spellId = cast.SpellId;

                var dialogSbp = casterGameObject.GetComponentInChildren<DialogSelfBadgePoint>();
                if (spellDoer.AssertIfNull(nameof(spellDoer)) ||
                    dialogSbp.AssertIfNull(nameof(dialogSbp)) ||
                    dialogOpponentBadgePoint.AssertIfNull(nameof(dialogOpponentBadgePoint)))
                {
                    spellDoer.StopCast(spellId, FinishReasonType.None);
                    return;
                }
                dialogSbp.Attach(spellDoer, await def.Target.Target.GetOuterRef(cast, repo), dialogOpponentBadgePoint, spellId);
                dialogSbp.SetDialog(def.Dialog, default);
            });

            return;
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, EffectOpenUIDialogDef def)
        {
            if (!cast.IsSlave || !cast.SlaveMark.OnClient)
                return;

            var casterID = (await def.Caster.Target.GetOuterRef(cast, repo)).Guid;
            var characterID = GameState.Instance.CharacterRuntimeData.CharacterId;
            if (casterID != characterID)
                return;

            GameObject casterGameObject = def.Caster.Target.GetGo(cast);

            UnityQueueHelper.RunInUnityThreadNoWait(() =>
            {
                var dialogSbp = casterGameObject.GetComponentInChildren<DialogSelfBadgePoint>();
                dialogSbp.Detach();
            });
        }
    }
}