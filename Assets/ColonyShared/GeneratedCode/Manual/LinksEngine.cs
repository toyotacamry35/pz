using Assets.ColonyShared.SharedCode.Entities.Engine;
using Assets.ResourceSystem.Aspects.Links;
using Assets.Src.Aspects.Impl.Definitions;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;
using ResourceSystem.Utils;
using SharedCode.Entities.Service;
using SharedCode.EntitySystem;
using SharedCode.EntitySystem.Delta;
using SharedCode.Refs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;


namespace GeneratedCode.DeltaObjects
{
    public partial class LinksEngine
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool DEBUG_LOG = false;
        public async ValueTask<OuterRef<IEntity>> GetLinkedImpl(LinkTypeDef link)
        {
            if (Links.TryGetValue(link, out var ents))
                return new OuterRef<IEntity>(ents.Links.FirstOrDefault().Key);
            return default;
        }
        public async ValueTask<IEnumerable<OuterRef<IEntity>>> GetLinkedsImpl(LinkTypeDef link)
        {
            List<OuterRef<IEntity>> returnEnts = new List<OuterRef<IEntity>>();
            if (Links.TryGetValue(link, out var ents))
                returnEnts.AddRange(ents.Links.Select(x => new OuterRef<IEntity>(x.Key)));
            if (SavedLinks.TryGetValue(link, out var sents))
                returnEnts.AddRange(ents.Links.Select(x => new OuterRef<IEntity>(x.Key)));
            return returnEnts;
        }
        public async Task SetLinksFromSceneImpl(Dictionary<LinkTypeDef, List<OuterRef<IEntity>>> links)
        {
            if (links != null)
                foreach (var linkType in links)
                    foreach (var ent in linkType.Value)
                        await AddLinkRef(linkType.Key, ent, true, false);
        }

        public async Task AddLinkRefImpl(LinkTypeDef key, OuterRef<IEntity> outerRef, bool watched, bool saved)
        {
            if (DEBUG_LOG)
            {
                Logger.IfError()?.Message($"\n---------------AddLinksImpl START [key: {key} | outerRef: {outerRef}]---------------\n").Write();
                Dump();
            }
            var links = saved ? SavedLinks : Links;
            if(!links.TryGetValue(key, out var dict))
            {
                dict = new LinksHolder();
                links.Add(key, dict);
                dict.Links.Add(outerRef, watched);

                if (DEBUG_LOG)
                {
                    Dump();
                    Logger.IfError()?.Message("\n---------------AddLinksImpl END---------------\n").Write();
                }
                if (watched)
                    await SubscribeOnDestroy(outerRef);
            }
            else
            {
                dict.Links.Add(outerRef, watched);

                if (DEBUG_LOG)
                {
                    Dump();
                    Logger.IfError()?.Message("\n---------------AddLinksImpl END---------------\n").Write();
                }
                if (watched)
                    await SubscribeOnDestroy(outerRef);
            }
            

        }

        public async Task OnDatabaseLoadImpl()
        {
            //foreach (var item in Links)
            //{
            //    foreach (var inner_item in item.Value)
            //    {
            //        if (inner_item.Key.IsValid)
            //            SubscribeOnDestroy(inner_item.Key);
            //    }
            //}
        }

        public async Task RemoveLinkKeyImpl(LinkTypeDef key)
        {
            if (DEBUG_LOG)
            {
                Logger.IfError()?.Message($"\n---------------RemoveLinkKeyImpl START [{key}]---------------\n").Write();
                Dump();
            }
            await RemoveFromLinks(key, Links);
            await RemoveFromLinks(key, SavedLinks);
            if (DEBUG_LOG)
            {
                Dump();
                Logger.IfError()?.Message("\n---------------RemoveLinkKeyImpl END---------------\n").Write();
            }
        }

        private async Task RemoveFromLinks(LinkTypeDef key, IDeltaDictionary<LinkTypeDef, ILinksHolder> links)
        {
            if (links.ContainsKey(key))
            {
                if (links[key] != null)
                {
                    foreach (var item in links[key].Links)
                    {
                        if (item.Value)
                            await UnsubscribeOnDestroy(item.Key);
                    }
                }
                links.Remove(key);
            }
        }

        public async Task RemoveLinkRefImpl(OuterRef<IEntity> outerRef)
        {
            if (DEBUG_LOG)
            {
                Logger.IfError()?.Message($"\n---------------RemoveLinkImpl START [{outerRef}]---------------\n").Write();
                Dump();
            }
            await ClearLinks(outerRef, Links);
            await ClearLinks(outerRef, SavedLinks);

            if (DEBUG_LOG)
            {
                Dump();
                Logger.IfError()?.Message("\n---------------RemoveLinkImpl END---------------\n").Write();
            }
        }

        private async Task ClearLinks(OuterRef<IEntity> outerRef, IDeltaDictionary<LinkTypeDef, ILinksHolder> links)
        {
            bool check = false;
            foreach (var item in links)
            {
                if (item.Value.Links.TryGetValue(outerRef, out var watched))
                {
                    if (watched)
                        await UnsubscribeOnDestroy(outerRef);
                    links[item.Key].Links.Remove(outerRef);
                    check = true;
                }
            }
            if (check)
                RemoveEmptyKeys();
        }

        public async Task RemoveLinkRefByKeyImpl(LinkTypeDef key, OuterRef<IEntity> outerRef)
        {
            if (DEBUG_LOG)
            {
                Logger.IfError()?.Message($"\n---------------RemoveLinkByKeyImpl START [key: {key} | outerRef: {outerRef}]---------------\n").Write();
                Dump();
            }
            await RemoveFromLinksByKey(key, outerRef, Links);
            await RemoveFromLinksByKey(key, outerRef, SavedLinks);

            if (DEBUG_LOG)
            {
                Dump();
                Logger.IfError()?.Message("\n---------------RemoveLinkByKeyImpl END---------------\n").Write();
            }
        }

        private async Task RemoveFromLinksByKey(LinkTypeDef key, OuterRef<IEntity> outerRef, IDeltaDictionary<LinkTypeDef, ILinksHolder> links)
        {
            bool check = false;
            if (links.TryGetValue(key, out var l))
            {
                if (l.Links.TryGetValue(outerRef, out var watched))
                {
                    if (watched)
                        await UnsubscribeOnDestroy(outerRef);
                    links[key].Links.Remove(outerRef);
                    check = true;
                }
            }

            if (check)
                RemoveEmptyKeys();
        }

        private void Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\n----------------------[LINKS ENGINE]----------------------");
            if (Links != null)
                foreach (var item in Links)
                {
                    if (!item.Equals(default))
                    {
                        sb.Append("KEY: ")
                            .Append(item.Key?.ToString())
                            .Append("\n\tVALUE: ");

                        if (item.Value != null)
                        {
                            foreach (var inner_item in item.Value.Links)
                            {
                                var k = inner_item.Key;
                                sb.Append("\n\t\t").Append(k.TypeId).Append(" ").AppendLine(k.Guid.ToString());
                            }
                        }
                        else
                            sb.Append("\n\t\t").AppendLine("NULL");
                    }
                }
            else
                sb.AppendLine("\nLinks is NULL");
            sb.AppendLine("\n----------------------[LINKS ENGINE]----------------------");
            Logger.IfError()?.Message(sb).Write();
        }
        private async Task RemoveLinkOnDestroy(int typeId, Guid guid, IEntity entity)
        {
            if (DEBUG_LOG) Logger.IfError()?.Message($"RemoveLinkOnDestroy {typeId} {guid} {entity}").Write();
            if (entity is IHasWorldSpaced hwd)
                if (!hwd.WorldSpaced.Destroyed)
                    return;
            var target = new OuterRef<IEntity>(guid, typeId);
            var oref = new OuterRef<IEntity>(parentEntity);
            using (var ent = await EntitiesRepository.Get(parentEntity.TypeId, parentEntity.ParentEntityId))
            {
                var le = ent.Get<IHasLinksEngineServer>(oref, ReplicationLevel.Server)?.LinksEngine;

                if (le != null)
                    await le.RemoveLinkRef(target);
                else
                    Logger.IfWarn()?.Message($"ImpactRemoveLink: Unexpected - Target({oref}) is not {nameof(IHasLinksEngineServer)}.").Write();
            }
        }
        private async Task SubscribeOnDestroy(OuterRef<IEntity> outerRef)
        {
            if (RefCountInDictionary(outerRef) > 0)
                return;

            EntitiesRepository.SubscribeOnDestroyOrUnload(outerRef.TypeId, outerRef.Guid, RemoveLinkOnDestroy);
            if (DEBUG_LOG) Logger.IfError()?.Message($"SubscribeOnDestroy success! {outerRef}").Write();
        }
        private async Task UnsubscribeOnDestroy(OuterRef outerRef)
        {
            if (RefCountInDictionary(outerRef) == 1)
                return;
            EntitiesRepository.UnsubscribeOnDestroyOrUnload(outerRef.TypeId, outerRef.Guid, RemoveLinkOnDestroy);
            if (DEBUG_LOG) Logger.IfError()?.Message($"UnsubscribeOnDestroy success! {outerRef}").Write();
        }
        private int RefCountInDictionary(OuterRef outerRef)
        {
            int count = Links.Values.Count(x => x.Links.ContainsKey(outerRef));
            if (DEBUG_LOG) Logger.IfError()?.Message($"\n---------------RefCountInDictionary [count: {count} ref:{outerRef}]---------------\n").Write();
            return count;
        }
        private void RemoveEmptyKeys()
        {
            if (DEBUG_LOG)
            {
                Logger.IfError()?.Message($"\n---------------RemoveEmptyKeys START---------------\n").Write();
                Dump();
            }

            List<LinkTypeDef> keys = new List<LinkTypeDef>();
            foreach (var item in Links)
            {
                if (item.Value == null
                    || item.Value.Links.Count == 0
                    || item.Value.Links.Keys.All(x => !x.IsValid))
                    keys.Add(item.Key);
            }
            foreach (var item in keys)
            {
                Links.Remove(item);
            }

            if (DEBUG_LOG)
            {
                Dump();
                Logger.IfError()?.Message($"\n---------------RemoveEmptyKeys END---------------\n").Write();
            }
        }
    }
}
