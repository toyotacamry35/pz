using System;
using ReactivePropsNs;

namespace Uins
{
    public interface IPaginationVM : IDisposable
    {
        IListStream<IPaginationElementVM> PaginationElementVMs { get; }

        IReactiveProperty<IPaginationElementVM> Selected { get; }
        IReactiveProperty<int> SelectedIndex { get; }

        IStream<bool> PrevAvailable { get; }
        IStream<bool> NextAvailable { get; }
        IStream<IPaginationElementVM> Prev { get; }
        IStream<IPaginationElementVM> Next { get; }

        void SetSelected(IPaginationElementVM element);
    }


    public interface IPaginationElementVM : IDisposable
    {
        IStream<bool> Selected { get; }

        IReactiveProperty<bool> Available { get; }

        void SetSelected();
    }
}