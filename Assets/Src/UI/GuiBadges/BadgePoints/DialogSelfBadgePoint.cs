using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.Shared;
using Assets.Src.Wizardry;
using ColonyShared.SharedCode.Wizardry;
using Core.Environment.Logging.Extension;
using ResourceSystem.Aspects.Dialog;
using GeneratedDefsForSpells;
using SharedCode.EntitySystem;
using SharedCode.Wizardry;
using L10n;
using SharedCode.Serializers;

namespace Uins
{
    public class DialogSelfBadgePoint : BadgePoint
    {
        private ISpellDoer _spellDoer;
        private OuterRef<IEntity> _target;
        private DialogOpponentBadgePoint _dialogOpponentBadgePoint;
        private SpellId _spellId;


        //=== Props ===========================================================

        protected DialogSelfGuiBadge DialogSelfGuiBadge => (DialogSelfGuiBadge) ConnectedGuiBadge;


        //=== Public ==========================================================

        public void Attach(ISpellDoer spellDoer, OuterRef<IEntity> target, DialogOpponentBadgePoint dialogOpponentBp, SpellId spellId)
        {
            _spellDoer = spellDoer;
            _target = target;
            _spellId = spellId;
            IsNeedForGuiRp.Value = true;
            SetIsImportantBadgeShown(true);
            _dialogOpponentBadgePoint = dialogOpponentBp;
            _dialogOpponentBadgePoint.SetIsImportantBadgeShown(true);
            _dialogOpponentBadgePoint.IsNeedForGuiRp.Value = true;
        }

        public void Detach()
        {
            SetDialog(default, default);
            _spellDoer = null;
            _target = new OuterRef<IEntity>();
            if (_dialogOpponentBadgePoint != null)
            {
                _dialogOpponentBadgePoint.SetIsImportantBadgeShown(false);
                _dialogOpponentBadgePoint.IsNeedForGuiRp.Value = false;
                _dialogOpponentBadgePoint = null;
            }

            _spellId = SpellId.Invalid;
            SetIsImportantBadgeShown(false);
            IsNeedForGuiRp.Value = false;
        }

        public void SetDialog(DialogDef dialog, LocalizedString phrase)
        {
            if (dialog?.Spell.IsValid ?? false)
            {
                AsyncUtils.RunAsyncTask(async () =>
                {
                    if (await _spellDoer.CanStartCast(new SpellCastBuilder().SetTargetIfValid(_target).SetSpell(dialog.Spell).Build()))
                        UnityQueueHelper.RunInUnityThreadNoWait(() => ChangeDialog(dialog, phrase));
                }, NodeAccessor.Repository);
            }
            else
            {
                ChangeDialog(dialog, phrase);
            }
        }

        private void ChangeDialog(DialogDef dialog, LocalizedString phrase)
        {
            if (_spellDoer.AssertIfNull(nameof(_spellDoer)) ||
                _dialogOpponentBadgePoint.AssertIfNull(nameof(_dialogOpponentBadgePoint))) //может случиться, если придет из тредпула
                return;

            if (dialog?.Spell.IsValid ?? false)
                _spellDoer.DoCast(new SpellCastBuilder().SetTargetIfValid(_target).SetSpell(dialog.Spell).Build());

            if (dialog?.Next != null)
            {
                _dialogOpponentBadgePoint.SetPhrase(string.IsNullOrEmpty(phrase.Key) ? dialog.Phrase : phrase);

                AsyncUtils.RunAsyncTask(async () =>
                {
                    var variants = new List<DialogVariantVmodel>();
                    var i = 0;
                    foreach (var next in dialog.Next)
                    {
                        if (next.Dialog.Target?.Spell.Target == null ||
                            await _spellDoer.CanStartCast(new SpellCastBuilder().SetTargetIfValid(_target).SetSpell(next.Dialog.Target.Spell.Target).Build()))
                        {
                            //foreach (var answer in next.Answer) //TODOM
                            variants.Add(new DialogVariantVmodel(next.Dialog, next.Answer1, next.OverwritePhrase, i++));
                        }
                    }

                    UnityQueueHelper.RunInUnityThreadNoWait(() => DialogSelfGuiBadge.UpdateDialogVariants(variants));
                }, NodeAccessor.Repository);
            }
            else
            {
                _spellDoer.StopCast(_spellId, FinishReasonType.None);
            }
        }

        public void OnVariantButton(int variantIndex)
        {
            var dialogVariantVms = DialogSelfGuiBadge?.DialogVariantViewModels;
            if (dialogVariantVms.AssertIfNull(nameof(dialogVariantVms), gameObject))
                return;

            if (dialogVariantVms.Count <= variantIndex)
            {
                UI.Logger.IfError()?.Message($"Wrong index={variantIndex}, VmsCount={dialogVariantVms.Count}").Write();
                return;
            }

            var dialogVariantVmodel = dialogVariantVms[variantIndex];
            if (dialogVariantVmodel == null)
            {
                UI.Logger.IfError()?.Message($"Null {nameof(dialogVariantVmodel)}[{variantIndex}] is null").Write();
                return;
            }

            SetDialog(dialogVariantVmodel.Dialog, dialogVariantVmodel.Phrase);
        }

        public void OnEscButton()
        {
            _spellDoer?.StopCast(_spellId, FinishReasonType.None);
        }
    }
}