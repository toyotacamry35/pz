using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Config;
using SharedCode.Config;
using SharedCode.EntitySystem;

namespace Infrastructure.Cloud
{
    public interface ICloudHost
    {
        IReadOnlyDictionary<RepositoryId, IEntitiesRepository> Repositories { get; }
    }

    public struct RepositoryId : IEquatable<RepositoryId>
    {
        public readonly string Name;
        public readonly int Id;

        public RepositoryId(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public override bool Equals(object obj)
        {
            return obj is RepositoryId && Equals((RepositoryId)obj);
        }

        public bool Equals(RepositoryId other)
        {
            return Name == other.Name &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            var hashCode = 1460282102;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(RepositoryId id1, RepositoryId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(RepositoryId id1, RepositoryId id2)
        {
            return !(id1 == id2);
        }
    }
}
