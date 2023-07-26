using GeneratedCode.Repositories;
using SharedCode.Entities.Regions;
using SharedCode.EntitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceSystem.Utils;

namespace SharedCode.Utils.Extensions
{
    public static class OuterRefExtension
    {
        public static int RepTypeId<T>(this OuterRef<T> oRef, ReplicationLevel level)
        {
            return EntitiesRepository.GetReplicationTypeId(oRef.TypeId, level);
        }
        
        public static OuterRef<T> RefWithRepTypeId<T>(this OuterRef<T> oRef, ReplicationLevel level)
        {
            return new OuterRef<T>(oRef.Guid, oRef.RepTypeId(level));
        }
        
        public static OuterRef<T> To<T>(this OuterRef @ref)
        {
            return new OuterRef<T>(@ref.Guid, @ref.TypeId);
        }
    }
}
