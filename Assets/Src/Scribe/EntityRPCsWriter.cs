// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Reflection;
// using System.Runtime.CompilerServices;
// using System.Threading;
// using GeneratedCode.EntitySystem;
// using GeneratedCode.Repositories;
// using Harmony;
// using NLog;
// using SharedCode.ActorServices;
// using SharedCode.EntitySystem;
// using SharedCode.Network;
// using SharedCode.Scribe;
// using UnityAsyncAwaitUtil;
// using UnityEngine.Profiling;
// using Core.Reflection;
// using SharedCode.Repositories;
//
// namespace Assets.ColonyShared.SharedCode.Scribe
// {
//     public class EntityRPCsWriter : IScribeWriter
//     {
//         private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
//
//         public static IEnumerable<Type> GetAllTypes<T>()
//         {
//             foreach (var asm in AppDomain.CurrentDomain.GetAssembliesSafe())
//                 foreach (var type in asm.GetTypesSafe())
//                     if (typeof(T).IsAssignableFrom(type))
//                         yield return type;
//         }
//
//         public void ConnectToCode(HarmonyInstance harmony)
//         {
//             return;
//             /*sampler = CustomSampler.Create("ThreadWork");
//             var entities = GetAllTypes<BaseEntity>();
//             foreach (var entity in entities)
//             {
//                 var interfaceWithRPCs = entity.GetInterfaces().FirstOrDefault(x => typeof(IEntity).IsAssignableFrom(x));
//                 if (interfaceWithRPCs == null)
//                 {
//                     Logger.IfInfo()?.Message($"Weird entity type without implementation {entity.Name}").Write();
//                 }
//                 else
//                     foreach (var method in interfaceWithRPCs.GetMethodsSafe(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).Where(x => !x.IsSpecialName))
//                     {
//                         var implMethod = entity.GetMethod(method.Name, method.GetParameters().Select(x => x.ParameterType).ToArray());
//                         MethodInfo implMethodImpl;
//                         if (implMethod.GetCustomAttribute<AddNetworkProxyParameterAttribute>() != null)
//                             implMethodImpl = entity.GetMethod(method.Name, method.GetParameters().Select(x => x.ParameterType).Add(typeof(INetworkProxy)).ToArray());
//                         else
//                             implMethodImpl = entity.GetMethod(method.Name, method.GetParameters().Select(x => x.ParameterType).ToArray());
//                         harmony.Patch(implMethod,
//                             new HarmonyMethod(this.GetType().GetMethod(nameof(PreRpc), BindingFlags.Static | BindingFlags.NonPublic)),
//                             new HarmonyMethod(this.GetType().GetMethod(nameof(PostRpc), BindingFlags.Static | BindingFlags.NonPublic)));
//                         harmony.Patch(implMethodImpl,
//                             new HarmonyMethod(this.GetType().GetMethod(nameof(PreRpcImpl), BindingFlags.Static | BindingFlags.NonPublic)),
//                             new HarmonyMethod(this.GetType().GetMethod(nameof(PostRpcImpl), BindingFlags.Static | BindingFlags.NonPublic)));
//                     }
//             }
//             var lockEntities = typeof(EntitiesRepository).GetMethod("lockBatch", BindingFlags.Instance | BindingFlags.NonPublic);
//             var releaseEntities = typeof(EntitiesRepository).GetMethod("Release");
//
//             harmony.Patch(lockEntities,
//                 new HarmonyMethod(this.GetType().GetMethod(nameof(PreLock), BindingFlags.Static | BindingFlags.NonPublic)),
//                 new HarmonyMethod(this.GetType().GetMethod(nameof(PostLock), BindingFlags.Static | BindingFlags.NonPublic))
//                 );
//             harmony.Patch(releaseEntities,
//                 new HarmonyMethod(this.GetType().GetMethod(nameof(PreRelease), BindingFlags.Static | BindingFlags.NonPublic)),
//                 new HarmonyMethod(this.GetType().GetMethod(nameof(PostRelease), BindingFlags.Static | BindingFlags.NonPublic)));
//             var targetMethod = typeof(IAsyncStateMachine).GetMethod("MoveNext");
//             int amountOfPatchedMethods = 0;
//             foreach (var type in AppDomain.CurrentDomain.GetAssembliesSafe().SelectMany(x => x.GetTypesSafe()).Where(t => typeof(IAsyncStateMachine).IsAssignableFrom(t)))
//             {
//                 try
//                 {
//
//                     var interfacesMap = type.GetInterfaceMap(typeof(IAsyncStateMachine));
//                     var index = Array.IndexOf(interfacesMap.InterfaceMethods, targetMethod);
//                     harmony.Patch(interfacesMap.TargetMethods[index],
//                     new HarmonyMethod(this.GetType().GetMethod(nameof(PreExecutionContextRun), BindingFlags.Static | BindingFlags.Public)),
//                     new HarmonyMethod(this.GetType().GetMethod(nameof(PostExecutionContextRun), BindingFlags.Static | BindingFlags.Public)));
//                     amountOfPatchedMethods++;
//                 }
//                 catch (Exception e)
//                 {
//
//                 }
//             }
//             Logger.IfInfo()?.Message($"Amount of patched methods {amountOfPatchedMethods}").Write();*/
//         }
//         public class MoveNextState
//         {
//             public Stopwatch Stopwatch;
//             public SyncContextMethodExecuteEvent StartEvent;
//         }
//         public static void PreLock(object __instance, IEnumerable<BatchItem> batch, ref EntitiesLocked __state)
//         {
//             if (batch == null)
//                 return;
//             var entitiesLocked = new EntitiesLocked();
//
//             foreach (var item in batch)
//             {
//                 entitiesLocked.Entities.Add(new OuterRef<IEntity>() { Guid = item.EntityId, TypeId = item.EntityTypeId });
//             }
//             __state = entitiesLocked;
//         }
//
//         static CustomSampler sampler;
//         public static void PreExecutionContextRun(object __instance)
//         {
//             UnityEngine.Profiling.Profiler.BeginThreadProfiling("ClusterThreads", $"{Thread.CurrentThread.ManagedThreadId}");
//             sampler.Begin();
//         }
//
//         public static void PostExecutionContextRun()
//         {
//             sampler.End();
//         }
//
//         public static void PostLock(object __instance, ref EntitiesLocked __state)
//         {
//             if (__state != null)
//                 ScribeProvider.Scribe.NotifyOfEvent(__state);
//         }
//
//         public static void PreRelease(object __instance, BatchItem[] entitiesContainerItems, ref EntitiesReleased __state)
//         {
//             if (entitiesContainerItems == null)
//                 return;
//             var entitiesReleased = new EntitiesReleased();
//             foreach (var item in entitiesContainerItems)
//             {
//                 entitiesReleased.Entities.Add(new OuterRef<IEntity>() { Guid = item.EntityId, TypeId = item.EntityTypeId });
//             }
//             __state = entitiesReleased;
//         }
//
//         public static void PostRelease(object __instance, ref EntitiesReleased __state)
//         {
//             if (__state != null)
//                 ScribeProvider.Scribe.NotifyOfEvent(__state);
//         }
//
//         public static AsyncLocal<ClusterRpcEvent> _currentRPCEvent = new AsyncLocal<ClusterRpcEvent>();
//         public class RpcHookState
//         {
//             public int ThreadId;
//             public ClusterRpcEvent AsyncClusterRpcEvent;
//         }
//         public static void PreRpc(object __instance, MethodInfo __originalMethod, ref RpcHookState __state)
//         {
//             var e = CreateRpcEvent<RpcStarted>(__instance, __originalMethod);
//             e.SessionGuid = Guid.NewGuid();
//             _currentRPCEvent.Value = e;
//             __state = new RpcHookState() { ThreadId = Thread.CurrentThread.ManagedThreadId, AsyncClusterRpcEvent = e };
//             ScribeProvider.Scribe.NotifyOfEvent(e);
//         }
//
//         public static void PostRpc(object __instance, MethodInfo __originalMethod, ref RpcHookState __state)
//         {
//             var e = CreateRpcEvent<RpcFinished>(__instance, __originalMethod);
//             e.SessionGuid = __state.AsyncClusterRpcEvent.SessionGuid;
//             ScribeProvider.Scribe.NotifyOfEvent(e);
//             if (__state.ThreadId != Thread.CurrentThread.ManagedThreadId || __state.AsyncClusterRpcEvent != _currentRPCEvent.Value)
//                  Logger.IfInfo()?.Message("ASync state mismatch, be aware").Write();;
//         }
//         public static void PreRpcImpl(object __instance, MethodInfo __originalMethod, ref RpcHookState __state)
//         {
//             var e = CreateRpcEvent<RpcImplStarted>(__instance, __originalMethod);
//             e.SessionGuid = _currentRPCEvent.Value.SessionGuid;
//             __state = new RpcHookState() { ThreadId = Thread.CurrentThread.ManagedThreadId, AsyncClusterRpcEvent = e };
//             ScribeProvider.Scribe.NotifyOfEvent(e);
//
//         }
//
//         public static void PostRpcImpl(object __instance, MethodInfo __originalMethod, ref RpcHookState __state)
//         {
//             RpcImplFinished e = CreateRpcEvent<RpcImplFinished>(__instance, __originalMethod);
//             e.SessionGuid = __state.AsyncClusterRpcEvent.SessionGuid;
//             if (__state.AsyncClusterRpcEvent != _currentRPCEvent.Value)
//                  Logger.IfInfo()?.Message("ASync state mismatch, be aware").Write();;
//             ScribeProvider.Scribe.NotifyOfEvent(e);
//         }
//
//         private static T CreateRpcEvent<T>(object __instance, MethodInfo __originalMethod) where T : ClusterRpcEvent, new()
//         {
//             return new T()
//             {
//                 EntityGUID = ((IEntity)__instance).Id,
//                 EntityType = ReplicaTypeRegistry.GetTypeById(((IEntity)__instance).TypeId),
//                 RPCName = __originalMethod.Name,
//                 IsInUnityThread = SyncContextUtil.IsInUnity,
//                 ThreadID = Thread.CurrentThread.ManagedThreadId
//             };
//         }
//     }
//
//     public class ClusterEntitiesEvent : ScribeEvent
//     {
//         public List<OuterRef<IEntity>> Entities = new List<OuterRef<IEntity>>();
//     }
//
//     public class EntitiesLocked : ClusterEntitiesEvent
//     {
//
//     }
//
//     public class EntitiesReleased : ClusterEntitiesEvent
//     {
//
//     }
//     public class SyncContextMethodExecuteEvent : ScribeEvent
//     {
//         public Guid AssociatedRpcSession;
//         public bool IsInUnity;
//         public int ThreadId;
//         public string MethodName;
//         public override string ToString()
//         {
//             return $"{MethodName} {(IsInUnity ? "Unity" : ThreadId.ToString())}";
//         }
//     }
//     public class ExecutedContinuationEvent : ScribeEvent, ILongEvent
//     {
//         public SyncContextMethodExecuteEvent StartEvent;
//
//         public long ElapsedTicks;
//
//         float ILongEvent.ElapsedMilliseconds => (float)ElapsedTicks / 10000f;
//
//         public override string ToString()
//         {
//             return $"{StartEvent.ToString()} {ElapsedTicks} ms.";
//         }
//     }
//
//     public class ClusterRpcEvent : ScribeEvent
//     {
//         public Guid SessionGuid;
//         public string RPCName;
//         public Guid EntityGUID;
//         public Type EntityType;
//         public bool IsInUnityThread = false;
//         public int ThreadID;
//         public override string ToString()
//         {
//             return $"{RPCName} {(IsInUnityThread ? "Unity" : ThreadID.ToString())}";
//         }
//     }
//
//     public class RpcStarted : ClusterRpcEvent
//     {
//
//     }
//     public class RpcFinished : ClusterRpcEvent
//     {
//
//     }
//     public class RpcImplStarted : ClusterRpcEvent
//     {
//
//     }
//     public class RpcImplFinished : ClusterRpcEvent
//     {
//
//     }
//
// }
