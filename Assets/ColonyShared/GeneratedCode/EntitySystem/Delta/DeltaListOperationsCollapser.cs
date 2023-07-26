// using System;
// using System.Collections.Generic;
//
// namespace SharedCode.EntitySystem.Delta
// {
//     public class DeltaListOperationsCollapser<T>
//     {
//         /// <summary>
//         /// Удаляет не нужные DeltaOperations. 
//         /// </summary>
//         /// <param name="deltaOperationsMap">Хранит актуальные индексы с учетом смещений из-за Insert, Remove</param>
//         public static void AddAndTryCollapseDeltaOperation(List<ListDeltaOperation<T>> deltaOperations,
//             Dictionary<int, DeltaList<T>.OldDeltaOperation> deltaOperationsMap, Dictionary<int, DeltaList<T>.OldDeltaOperation> deltaOperationsMap, ListDeltaOperation<T> operation)
//         {
//             var nextIndex = deltaOperations.Count;
//             switch (operation.Operation)
//             {
//                 case ListDeltaOperationType.InsertNew:
//                 {
//                     // смещать в мапе операции с индексом => текущего. Так как текущей вставкой мы сместим все елементы в права 
//                     // и вся последующая работа с ними будет по индексам+1 от текущих
//                     // в листе не смещаем, так как это сломает последовательность действий на реплике
//                     ShiftElementIndexMapRight(deltaOperationsMap,)
//                     if (deltaOperationsMap.TryGetValue(operation.Index, out var oldOperation))
//                     {
//                         switch (oldOperation.Operation.Operation)
//                         {
//                             case ListDeltaOperationType.InsertNew:
//                                 break;
//                             case ListDeltaOperationType.ReplaceNew:
//                                 break;
//                             case ListDeltaOperationType.Remove:
//                                 break;
//                             default:
//                                 throw new ArgumentOutOfRangeException();
//                         }
//                     }
//                     else
//                     {
//                         deltaOperationsMap[operation.Index] = new DeltaList<T>.OldDeltaOperation(nextIndex, operation);
//                         deltaOperations.Add(operation);
//                     }
//                     
//                     break;
//                 }
//                 case ListDeltaOperationType.ReplaceNew:
//                 {
//                     if (deltaOperationsMap.TryGetValue(operation.Index, out var oldOperation))
//                     {
//                         switch (oldOperation.Operation.Operation)
//                         {
//                             case ListDeltaOperationType.InsertNew:
//                                 
//                                 // траснформируем ReplaceNew в InsertNew
//                                 var insertNewOperation = new ListDeltaOperation<T>
//                                 {
//                                     Operation = ListDeltaOperationType.InsertNew,
//                                     Index = oldOperation.Operation.Index,
//                                     Value = operation.Value,
//                                     ValueRef = operation.ValueRef,
//                                     RuntimeDeltaObject = operation.RuntimeDeltaObject
//                                 };
//
//                                 if (oldOperation.Operation.RuntimeDeltaObject != null &&
//                                     oldOperation.Operation.RuntimeDeltaObject.LocalId != 0)
//                                 {
//                                     insertNewOperation.DeltaObjectLocalId =
//                                         oldOperation.Operation.RuntimeDeltaObject.LocalId;
//                                 }
//
//                                 ReplaceOperation(deltaOperations, deltaOperationsMap, oldOperation, insertNewOperation);
//
//                                 break;
//                             case ListDeltaOperationType.ReplaceNew:
//                                 ReplaceOperation(deltaOperations, deltaOperationsMap, oldOperation, operation);
//                                 break;
//                             default:
//                                 throw new ArgumentOutOfRangeException();
//                         }
//                     }
//                     break;
//                 }
//                 case ListDeltaOperationType.Remove:
//                 {
//                     if (deltaOperationsMap.TryGetValue(operation.Index, out var oldOperation))
//                     {
//                         switch (oldOperation.Operation.Operation)
//                         {
//                             case ListDeltaOperationType.InsertNew:
//                             {
//                                 ShiftElementIndexesLeft(deltaOperations, deltaOperationsMap,
//                                     oldOperation.OperationListIndex + 1);
//                                 
//                                 // трансформируем Insert в ничего
//                                 deltaOperations.RemoveAt(oldOperation.OperationListIndex);
//                                 deltaOperationsMap.Remove(operation.Index);
//                                 break;
//                             }
//                             case ListDeltaOperationType.ReplaceNew:
//                             {
//                                 ShiftElementIndexesLeft(deltaOperations, deltaOperationsMap,
//                                     oldOperation.OperationListIndex + 1);
//                                 
//                                 // траснформируем ReplaceNew в Remove
//                                 var removeOperation = new ListDeltaOperation<T>
//                                 {
//                                     Operation = ListDeltaOperationType.Remove,
//                                     Index = oldOperation.Operation.Index
//                                 };
//
//                                 ReplaceOperation(deltaOperations, deltaOperationsMap, oldOperation, removeOperation);
//                                 break;
//                             }
//                         }
//                     }
//                     else
//                     {
//                         deltaOperations.Add(operation);
//                     }
//
//                     break;
//                 }
//                 case ListDeltaOperationType.Clear:
//                     deltaOperationsMap.Clear();
//                     deltaOperations.Clear();
//                     deltaOperations.Add(operation);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }
//
//         private static void ReplaceOperation(List<ListDeltaOperation<T>> deltaOperations,
//             Dictionary<int, DeltaList<T>.OldDeltaOperation> deltaOperationsMap, DeltaList<T>.OldDeltaOperation oldOperation, ListDeltaOperation<T> newOperation)
//         {
//             deltaOperations[oldOperation.OperationListIndex] = newOperation;
//             deltaOperationsMap[oldOperation.Operation.Index] =
//                 new DeltaList<T>.OldDeltaOperation(oldOperation.OperationListIndex, newOperation);
//         }
//
//         private static void ShiftElementIndexesLeft(List<ListDeltaOperation<T>> deltaOperations,
//             Dictionary<int, DeltaList<T>.OldDeltaOperation> deltaOperationsMap, int fromOperationIndex)
//         {
//             for (int i = fromOperationIndex; i < deltaOperations.Count; i++)
//             {
//                 var deltaOperation = deltaOperations[i];
//                 var newElementIndex = deltaOperation.Index - 1;
//                 var oldElementIndex = deltaOperation.Index;
//                 if (deltaOperationsMap.TryGetValue(oldElementIndex, out var oldDeltaOperation))
//                 {
//                     deltaOperationsMap[newElementIndex] = oldDeltaOperation;
//                     deltaOperationsMap.Remove(oldElementIndex);
//                 }
//
//                 deltaOperation.Index = newElementIndex;
//             }
//         }
//         
//         private static void ShiftElementIndexMapRight(
//             Dictionary<int, DeltaList<T>.OldDeltaOperation> deltaOperationsMap,
//             Dictionary<int, DeltaList<T>.OldDeltaOperation> newDeltaOperationsMap, 
//             int fromElementIndex)
//         {
//             newDeltaOperationsMap.Clear();
//             foreach (var deltaOperation in deltaOperationsMap)
//             {
//                 int elementIndex = deltaOperation.Key;
//                 if (deltaOperation.Key >= fromElementIndex)
//                 {
//                     elementIndex = deltaOperation.Key + 1;
//                 }
//                 
//                 newDeltaOperationsMap.Add(elementIndex, deltaOperation.Value);
//             }
//         }
//     }
// }