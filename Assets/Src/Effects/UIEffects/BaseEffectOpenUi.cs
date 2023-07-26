using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.DeltaObjects;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using GeneratedDefsForSpells;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Serializers;
using SharedCode.Utils.Extensions;
using Uins;

namespace Assets.Src.Effects.UIEffects
{
    public abstract class BaseEffectOpenUi
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("UI");

        public async ValueTask Attach(SpellWordCastData cast, IEntitiesRepository repo, BaseEffectOpenUiDef def)
        {
            var characterRuntimeData = GameState.Instance?.CharacterRuntimeData;
            if (!cast.IsSlave || !cast.SlaveMark.OnClient || characterRuntimeData == null ||
                characterRuntimeData.CharacterId != cast.Caster.Guid)
                return;

            var effectData = new EffectData(def, cast, repo);
            await effectData.SetStartTargetOuterRef();

            if (effectData.Repo == null || !effectData.StartTargetOuterRef.IsValid)
            {
                LogError(effectData, $"{nameof(effectData)} is wrong");
                await StopSpellAsync(cast, repo);
                return;
            }

            var isSuccessAttachOpsResult = await OnSuccessAttach(effectData, repo);
            if (!isSuccessAttachOpsResult)
            {
                LogError(effectData, $"{nameof(OnSuccessAttach)}() is failed");
                await StopSpellAsync(cast, repo);
                return;
            }

            return;
        }

        public async ValueTask Detach(SpellWordCastData cast, IEntitiesRepository repo, BaseEffectOpenUiDef def)
        {
            var characterRuntimeData = GameState.Instance?.CharacterRuntimeData;
            if (!cast.IsSlave || !cast.SlaveMark.OnClient || characterRuntimeData == null ||
                characterRuntimeData.CharacterId != cast.Caster.Guid)
                return;

            var effectData = new EffectData(def, cast, repo);
            await effectData.SetStartTargetOuterRef();


            if (effectData.Repo == null || !effectData.StartTargetOuterRef.IsValid)
            {
                LogError(effectData, $"{nameof(effectData)} is wrong");
                return;
            }

            await OnSuccessDetach(effectData, repo);
        }

        public static async Task StopSpellAsync(SpellWordCastData cast, IEntitiesRepository repo)
        {
            try
            {
                if (cast.AssertIfNull(nameof(cast)))
                {
                    UI. Logger.IfError()?.Message("Wrong params cast").Write();;
                    return;
                }

                if (!cast.Wizard.IsValid)
                {
                    UI. Logger.IfError()?.Message("Wrong params cast.Wizard").Write();;
                    return;
                }

                if (!cast.SpellId.IsValid)
                {
                    UI. Logger.IfError()?.Message("Wrong params cast.SpellId").Write();;
                    return;
                }

                using (var wrapper = await repo.Get(cast.Wizard.TypeId, cast.Wizard.Guid))
                {
                    var wizardEntity =
                        wrapper.Get<IWizardEntityClientFull>(cast.Wizard.TypeId, cast.Wizard.Guid, ReplicationLevel.ClientFull);
                    var stopCastSpellResult = await wizardEntity.StopCastSpell(cast.SpellId);
                    if (!stopCastSpellResult)
                        UI.Logger.IfError()?.Message($"Unable to StopCastSpell {nameof(cast.SpellId)}={cast.SpellId}").Write();
                }
            }
            catch (Exception e)
            {
                UI.Logger.IfError()?.Message($"{nameof(StopSpellAsync)}(): {e}").Write();
            }
        }


        //=== Protected =======================================================

        //Выполняются быстро: запускают в UI, но его не ждут
        protected abstract Task OnOpenSuccess(EffectData effectData);

        protected abstract Task OnCloseAnyway(EffectData effectData);


        //=== Private =========================================================

        private async Task<bool> OnSuccessAttach(EffectData effectData, IEntitiesRepository repository)
        {
            using (var entitiesContainer = await repository.Get(effectData.StartTargetOuterRef))
            {
                var entityObject = entitiesContainer?.Get<IEntityObjectAlways>(effectData.StartTargetOuterRef, ReplicationLevel.Always);
                if (entitiesContainer == null || entityObject == null)
                {
                    LogError(effectData, $"{nameof(entitiesContainer)} or/and {nameof(entityObject)} is null");
                    return false;
                }

                effectData.StartTargetDef = entityObject.Def;

                var hasOpenMechanics = entitiesContainer.Get<IHasOpenMechanicsClientBroadcast>(effectData.StartTargetOuterRef,
                    ReplicationLevel.ClientBroadcast);

//                if (hasOpenMechanics == null) //TODOM Вернуть после приведения банка к типовой схеме работы
//                {
//                    LogError(effectData, $"{nameof(hasOpenMechanics)} is null");
//                    return false;
//                }

                if (hasOpenMechanics != null)
                {
                    effectData.FinalTargetOuterRef = (await hasOpenMechanics.OpenMechanics.TryOpen(effectData.Cast.Caster)).To<IEntityObject>();
                    if (!effectData.FinalTargetOuterRef.IsValid)
                    {
                        LogError(effectData, "TryOpen() is failed");
                        return false;
                    }
                }
            }

            AsyncUtils.RunAsyncTask(async () => await OnOpenSuccess(effectData), repository);
            return true;
        }

        private async Task OnSuccessDetach(EffectData effectData, IEntitiesRepository repository)
        {
            await OnCloseAnyway(effectData);

            var targetEntityGuid = effectData.StartTargetOuterRef.Guid;
            var replicationTypeId = effectData.StartTargetOuterRef.TypeId;

            AsyncUtils.RunAsyncTask(async () =>
            {
                using (var wrapper = await repository.Get(replicationTypeId, targetEntityGuid, ReplicationLevel.ClientBroadcast))
                {
                    var entityObject = wrapper?.Get<IEntityObjectAlways>(effectData.StartTargetOuterRef, ReplicationLevel.Always);
                    effectData.StartTargetDef = entityObject?.Def;
                    var hasOpenMechanics = wrapper?.Get<IHasOpenMechanicsClientBroadcast>(effectData.StartTargetOuterRef, ReplicationLevel.ClientBroadcast);
                    if (hasOpenMechanics != null && !await hasOpenMechanics.OpenMechanics.TryClose(effectData.Cast.Caster))
                        LogError(effectData, "TryClose() is failed");
                }
            }, repository);
        }

        protected void LogError(EffectData effectData, string message, [CallerMemberName] string callerMethodName = "")
        {
            Logger.Error($"<{GetType()}> {callerMethodName}({nameof(effectData.Cast.SpellId)}={effectData.Cast.SpellId}, " +
                         $"target={effectData.StartTargetOuterRef}): {message}");
        }

        private void DebugInfo(CloudNodeType cloudNodeType, string msg)
        {
            UI.CallerLogDefault($"<{GetType()}> {msg} repo={cloudNodeType}");
        }

        public class EffectData
        {
            public OuterRef<IEntityObject> StartTargetOuterRef;
            public IEntityObjectDef StartTargetDef;

            /// <summary>
            /// Валидна при успехе TryOpen()
            /// </summary>
            public OuterRef<IEntityObject> FinalTargetOuterRef;

            public IEntityObjectDef FinalTargetDef;
            public BaseEffectOpenUiDef SelfDef;
            public SpellWordCastData Cast;
            public IEntitiesRepository Repo;

            public EffectData(BaseEffectOpenUiDef def, SpellWordCastData cast, IEntitiesRepository repository)
            {
                SelfDef = def;
                Cast = cast;
                Repo = repository;

                SelfDef.AssertIfNull(nameof(SelfDef));
                Repo.AssertIfNull(nameof(Repo));
            }

            public async Task SetStartTargetOuterRef()
            {
                StartTargetOuterRef = (await SelfDef.Target.Target.GetOuterRef(Cast, Repo)).To<IEntityObject>();
            }

            public override string ToString()
            {
                return $"[{nameof(EffectData)}: Start(outerRef={StartTargetOuterRef}, def={StartTargetDef}), " +
                       $"Final(outerRef={FinalTargetOuterRef}, def={FinalTargetDef}) /ED]";
            }
        }
    }
}