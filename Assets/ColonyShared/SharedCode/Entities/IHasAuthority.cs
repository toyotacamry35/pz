using SharedCode.Logging;
using Newtonsoft.Json;
using SharedCode.Cloud;
using SharedCode.Entities.GameObjectEntities;
using SharedCode.EntitySystem;
using SharedCode.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Environment.Logging.Extension;
using GeneratedCode.EntitySystem;
using GeneratedCode.Repositories;
using GeneratedCode.DeltaObjects.ReplicationInterfaces;

namespace Assets.ColonyShared.SharedCode.Entities
{
    public interface IHasAuthorityOwner
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        IAuthorityOwner AuthorityOwner { get; set; }
    }

    [GeneratorAnnotations.GenerateDeltaObjectCode]
    public interface IAuthorityOwner : IDeltaObject
    {
        [ReplicationLevel(ReplicationLevel.ClientFull)]
        [RuntimeData(SkipField = true)]
        bool HasClientAuthority { get; }
    }
}

namespace GeneratedCode.DeltaObjects
{ 
    public partial class AuthorityOwner
    {
        private static readonly List<Guid> EntitiesWithClientAuthority = new List<Guid>(16);

        // сугубо временное решение, пока Боря не сделает "как надо"
        public static bool CheckClientAuthority(IEntitiesRepository repo, Guid entityId)
        {
            return repo.TryGetLockfree<IWorldCharacterClientFull>(entityId, ReplicationLevel.ClientFull) != null;
        }

       

        [JsonIgnore]
        public bool HasClientAuthority
        {
            get
            {
                if (EntitiesRepository.CloudNodeType == CloudNodeType.Server)
                {
                    Log.Logger.IfError()?.Message("Must be called on client!").Write();
                    throw new Exception("Must be called on client!");
                }
                return CheckClientAuthority(EntitiesRepository, parentEntity.Id);
            }
        }

    }
}
