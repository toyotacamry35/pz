using System;
using System.Collections.Generic;
using System.Text;

namespace ReactivePropsNs
{
    public interface IHashSetStream<T> : ICollection<T>//, ISet<T>
    {
        IStream<T> AddStream { get; }
        IStream<T> RemoveStream { get; }
        IStream<int> CountStream { get; }
    }
}
