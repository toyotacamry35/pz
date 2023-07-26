using System.Collections.Generic;
using Assets.Src.Aspects.Impl.Factions.Template;
using ColonyShared.SharedCode.Entities.Reactions;
using ResourceSystem.Utils;

namespace ColonyShared.GeneratedCode.Manual.DeltaObjects
{
    public static class Relationship
    {
        public static void MapRelationshipArgs(RelationshipArgsMappingDef args, in Context ctx, List<ArgTuple> rv)
        {
            if (args == null)
                return;
            if (args.ThisEntity != null)
                rv.Add(ArgTuple.Create(args.ThisEntity, ArgValue.Create(ctx.ThisEntity)));
            if (args.OtherEntity != null)
                rv.Add(ArgTuple.Create(args.OtherEntity, ArgValue.Create(ctx.OtherEntity)));
        }
        
        public readonly struct Context
        {
            public readonly OuterRef ThisEntity;
            public readonly OuterRef OtherEntity;
            
            public Context(
                OuterRef thisEntity,
                OuterRef otherEntity)
            {
                ThisEntity = thisEntity;
                OtherEntity = otherEntity;
            }

            public override string ToString() => $"(ThisEntity:{ThisEntity} OtherEntity:{OtherEntity})";
        }
    }
}