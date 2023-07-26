using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;

namespace ColonyShared.SharedCode.InputActions
{
    public class InputActionsListDef : BaseResource
    {
       [UsedImplicitly] public ResourceRef<InputActionDef>[] Actions;
       [UsedImplicitly] public ResourceRef<InputActionsListDef>[] SubLists;

        public InputActionDef[] AllActions
        {
            get
            {
                if (_allActions == null)
                {
                    var actions = Enumerable.Empty<InputActionDef>();
                    if (Actions != null)
                        actions = Actions.Select(x => x.Target);
                    if (SubLists != null)
                    {
                        if(_passedLists.Value.Contains(this)) throw new Exception($"Cyclic reference detected in {this}");
                        _passedLists.Value.Add(this);
                        actions = actions.Concat(SubLists.SelectMany(x => x.Target?.AllActions));
                        _passedLists.Value.Remove(this);
                    }
                    _allActions = actions.Distinct().ToArray();
                }
                return _allActions;
            }
        }

        private InputActionDef[] _allActions;
        private static readonly ThreadLocal<HashSet<InputActionsListDef>> _passedLists = new ThreadLocal<HashSet<InputActionsListDef>>(() => new HashSet<InputActionsListDef>());        
    }
}