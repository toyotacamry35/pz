// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using SharedCode.Network;
using SharedCode.Logging;
using System;
using System.Threading.Tasks;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.EntityPropertyResolvers;
using GeneratedCode.EntitySystem;
using SharedCode.Refs;

namespace GeneratedCode.DeltaObjects
{
    public partial class WorldObjectsInformationDataSetEngine
    {
        [SharedCode.EntitySystem.RpcClassHashAttribute(-269827889)]
        internal static class ReceiveFuncs
        {
            [SharedCode.EntitySystem.RpcMethodReceiverFunc(0, "IWorldObjectsInformationDataSetEngine_RegisterWorldObjectsInNewInformationSet_EntityId_Message")]
            internal static async void IWorldObjectsInformationDataSetEngine_RegisterWorldObjectsInNewInformationSet_EntityId_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var worldObjectId = serializer.Deserialize<GeneratedCode.Repositories.EntityId>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = _repository.Get(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IWorldObjectsInformationDataSetEngine>();
                            await __entity__.RegisterWorldObjectsInNewInformationSet(worldObjectId);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(1, "IWorldObjectsInformationDataSetEngine_RegisterWorldObjectsInNewInformationSetBatch_List_Message")]
            internal static async void IWorldObjectsInformationDataSetEngine_RegisterWorldObjectsInNewInformationSetBatch_List_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var worldObjectsIds = serializer.Deserialize<System.Collections.Generic.List<GeneratedCode.Repositories.EntityId>>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = _repository.Get(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IWorldObjectsInformationDataSetEngine>();
                            await __entity__.RegisterWorldObjectsInNewInformationSetBatch(worldObjectsIds);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(2, "IWorldObjectsInformationDataSetEngine_UnregisterWorldObjectsInNewInformationSet_EntityId_Message")]
            internal static async void IWorldObjectsInformationDataSetEngine_UnregisterWorldObjectsInNewInformationSet_EntityId_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var worldObjectId = serializer.Deserialize<GeneratedCode.Repositories.EntityId>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = _repository.Get(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IWorldObjectsInformationDataSetEngine>();
                            await __entity__.UnregisterWorldObjectsInNewInformationSet(worldObjectId);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }

            [SharedCode.EntitySystem.RpcMethodReceiverFunc(3, "IWorldObjectsInformationDataSetEngine_UnregisterWorldObjectsInNewInformationSetBatch_List_Message")]
            internal static async void IWorldObjectsInformationDataSetEngine_UnregisterWorldObjectsInNewInformationSetBatch_List_Message_Func(IEntitiesRepository _repository, INetworkProxy networkProxy, Guid callback, byte[] __data__, int __offset__, PropertyAddress __propertyAddress__, IDeltaObject __targetObj__, Guid _, Guid __migrationId__)
            {
                try
                {
                    var serializer = ((IEntitiesRepositoryExtension)_repository).Serializer;
                    var worldObjectsIds = serializer.Deserialize<System.Collections.Generic.List<GeneratedCode.Repositories.EntityId>>(__data__, ref __offset__);
                    using (GeneratedCode.Manual.AsyncStack.AsyncStackIsolator.IsolateContext())
                    {
                        GeneratedCode.Manual.Repositories.CallbackRepositoryHolder.CurrentCallbackRepositoryId = callback;
                        if (__migrationId__ != Guid.Empty)
                            GeneratedCode.Manual.Repositories.MigrationIdHolder.SetCurrentMigrationId(ref __migrationId__);
                        var __task = _repository.Get(__propertyAddress__.EntityTypeId, __propertyAddress__.EntityId);
                        if (__task.IsCompleted)
                            await System.Threading.Tasks.Task.Yield();
                        using (await __task)
                        {
                            var __entity__ = __targetObj__.To<SharedCode.Entities.IWorldObjectsInformationDataSetEngine>();
                            await __entity__.UnregisterWorldObjectsInNewInformationSetBatch(worldObjectsIds);
                        }
                    }
                }
                catch (Exception e)
                {
                    Core.Environment.Logging.Extension.LoggerExtensions.IfError(Log.Logger)?.Message(e, "RPC receiver exception").Write();
                }
            }
        }
    }
}