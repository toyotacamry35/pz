using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.Entities;
using System.Linq;
using Core.Environment.Logging.Extension;
using SharedCode.Entities;
using ResourceSystem.Utils;

namespace GeneratedCode.DeltaObjects
{
    public partial class OpenMechanics : IOpenMechanicsImplementRemoteMethods
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly List<Guid> _openers = new List<Guid>();

        public async Task<OuterRef> TryOpenImpl(OuterRef outerRef)
        {
            var clientId = GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId;
            Logger.IfInfo()?.Message($"OpenMechanics: TryOpenImpl(). repo = {EntitiesRepository.Id}, {EntitiesRepository.CloudNodeType}; clientId = {clientId}").Write();

            if (_openers.Contains(clientId))
            {
                Logger.IfInfo()?.Message($"OpenMechanics: Attempt to open already opened typeId {parentEntity.TypeId} Id {parentEntity.Id}, {nameof(clientId)}{clientId}").Write();
                return default;
            }
            
            _openers.Add(clientId);
            if (!IsOpen)
            {
                IsOpen = true;
                Logger.IfInfo()?.Message($"OpenMechanics: await FirstOpenedOrLastClosedExternalCall(IsOpen = {IsOpen})").Write();
                await FirstOpenedOrLastClosedExternalCall(IsOpen);
            }
            var openable = parentEntity as IOpenable;
            if (openable == null)
                throw new Exception($"OpenMechanics.TryOpenImpl() typeId {parentEntity.TypeId} Id {parentEntity.Id} Not Implemented IOpenable");

            var targetRef = await openable.GetOpenOuterRef(outerRef);
            Logger.IfInfo()?.Message($"OpenMechanics: TryOpenImpl(). _openers = {string.Join(", ", _openers.Select(v => v.ToString()))}").Write();
            await EntitiesRepository.SubscribeReplication(targetRef.TypeId, targetRef.Guid, clientId, ReplicationLevel.ClientFull);
            return targetRef;
        }

        public async Task<bool> TryCloseImpl(OuterRef outerRef)
        {
            var clientId = GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId;
            Logger.IfInfo()?.Message($"OpenMechanics: TryCloseImpl(). repo = {EntitiesRepository.Id}, {EntitiesRepository.CloudNodeType}; clientId = {clientId}").Write();

            if (!_openers.Contains(clientId))
            {
                Logger.IfInfo()?.Message($"OpenMechanics: Attempt to close already closed typeId {parentEntity.TypeId} Id {parentEntity.Id}, {nameof(clientId)}{clientId}").Write();
                return false;
            }

            _openers.Remove(clientId);
            if (IsOpen && _openers.Count == 0)
            {
                IsOpen = false;
                Logger.IfInfo()?.Message($"OpenMechanics: await FirstOpenedOrLastClosedExternalCall(IsOpen = {IsOpen})").Write();
                await FirstOpenedOrLastClosedExternalCall(IsOpen);
            }

            var openable = parentEntity as IOpenable;
            if (openable == null)
                throw new Exception($"OpenMechanics.TryCloseImpl() typeId {parentEntity.TypeId} Id {parentEntity.Id} Not Implemented IOpenable");

            var targetRef = await openable.GetOpenOuterRef(outerRef);

            Logger.IfInfo()?.Message($"OpenMechanics: TryCloseImpl(). _openers = {string.Join(", ", _openers.Select(v => v.ToString()))}").Write();
            await EntitiesRepository.UnsubscribeReplication(targetRef.TypeId, targetRef.Guid, clientId, ReplicationLevel.ClientFull);            
            return true;
        }

        public Task FirstOpenedOrLastClosedExternalCallImpl(bool isOpened)
        {
            return OnFirstOpenedOrLastClosed(isOpened);
        }


        public ValueTask<bool> IsEmptyImpl()
        {
            var iHasInventory = parentEntity as IHasInventory;
            if (iHasInventory == null)
            {
                Logger.IfInfo()?.Message($"OpenMechanics: IsEmptyImpl(). iHasInventory == null").Write();
                return new ValueTask<bool>(false);
            }

            var result = !iHasInventory.Inventory.Items.Any();
            Logger.IfInfo()?.Message($"OpenMechanics: IsEmptyImpl(). result = {result}").Write();
            return new ValueTask<bool>(result);
        }
    }
}