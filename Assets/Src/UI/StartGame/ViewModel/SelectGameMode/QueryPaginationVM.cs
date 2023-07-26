using System;
using System.Linq;
using Assets.ReactiveProps;
using ReactivePropsNs;
using SharedCode.Aspects.Sessions;

namespace Uins
{
    public class QueryPaginationVM : BindingVmodel, IPaginationVM
    {
        private readonly ListStream<IPaginationElementVM> _paginationElementVMs = new ListStream<IPaginationElementVM>();
        private readonly ReactiveProperty<bool> _prevAvailable = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _nextAvailable = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<IPaginationElementVM> _prev = new ReactiveProperty<IPaginationElementVM>();
        private readonly ReactiveProperty<IPaginationElementVM> _next = new ReactiveProperty<IPaginationElementVM>();
        private readonly ReactiveProperty<IPaginationElementVM> _selected = new ReactiveProperty<IPaginationElementVM>();
        private readonly ReactiveProperty<int> _selectedIndex = new ReactiveProperty<int>();

        public IListStream<IPaginationElementVM> PaginationElementVMs => _paginationElementVMs;
        public IReactiveProperty<IPaginationElementVM> Selected => _selected;
        public IReactiveProperty<int> SelectedIndex => _selectedIndex;
        public IStream<bool> PrevAvailable => _prevAvailable;
        public IStream<bool> NextAvailable => _nextAvailable;
        public IStream<IPaginationElementVM> Prev => _prev;
        public IStream<IPaginationElementVM> Next => _next;

        public QueryPaginationVM(IStream<int> countStream)
        {
            countStream.Action(
                D,
                count =>
                {
                    while (_paginationElementVMs.Count < count)
                    {
                        var paginationElementVM = new QueryPaginationElementVM(this);
                        D.Add(paginationElementVM);
                        _paginationElementVMs.Add(paginationElementVM);
                    }

                    while (_paginationElementVMs.Count > count)
                    {
                        var paginationElementVM = _paginationElementVMs.Last();
                        _paginationElementVMs.Remove(paginationElementVM);
                        D.Remove(paginationElementVM);
                        paginationElementVM.Dispose();
                    }
                }
            );

            InitSelected();

            _paginationElementVMs.InsertStream.Action(D, e => UpdateNavigation());
            _paginationElementVMs.RemoveStream.Action(D, e => UpdateNavigation());
            _paginationElementVMs.ChangeStream.Action(D, e => UpdateNavigation());
            _selected.Action(D, e => UpdateNavigation());
        }

        public void SetSelected(IPaginationElementVM element)
        {
            SetSelectedIndex(_paginationElementVMs.IndexOf(element));
        }
        
        public void SetSelectedIndex(int index)
        {
            if (index >= 0 && index < _paginationElementVMs.Count)
                _selected.Value = _paginationElementVMs[index];
            else
                _selected.Value = null;
        }

        private void InitSelected()
        {
            _paginationElementVMs.Transform(
                D,
                (vm, vmD) =>
                {
                    if (Selected.Value == null)
                    {
                        _selected.Value = vm;
                        UpdateNavigation();
                    }

                    return new DisposeAgent(
                        () =>
                        {
                            if (Selected.Value == vm)
                            {
                                _selected.Value = _paginationElementVMs.FirstOrDefault();
                                UpdateNavigation();
                            }
                        });
                }
            );
        }

        private void UpdateNavigation()
        {
            var index = _paginationElementVMs.IndexOf(Selected.Value);
            _selectedIndex.Value = index;
            _prevAvailable.Value = index > 0;
            _nextAvailable.Value = index < _paginationElementVMs.Count - 1;
            _prev.Value = _prevAvailable.Value ? _paginationElementVMs[index - 1] : Selected.Value;
            _next.Value = _nextAvailable.Value ? _paginationElementVMs[index + 1] : Selected.Value;
        }
    }

    public class QueryPaginationElementVM : BindingVmodel, IPaginationElementVM
    {
        private readonly IPaginationVM _parentVM;
        public IStream<bool> Selected { get; }

        public IReactiveProperty<bool> Available { get; } = new ReactiveProperty<bool> {Value = true};

        public void SetSelected()
        {
            _parentVM.SetSelected(this);
        }

        public QueryPaginationElementVM(IPaginationVM parentVM)
        {
            _parentVM = parentVM;
            Selected = _parentVM.Selected.Func(D, vm => vm == this);
        }

        public override void Dispose()
        {
            Available.Dispose();
            base.Dispose();
        }
    }
}