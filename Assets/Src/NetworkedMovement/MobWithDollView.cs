using System;
using Assets.Src.Aspects;
using Assets.Src.NetworkedMovement;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;
using Src.ResourceSystem.JdbRefs;
using UnityEngine;

namespace Src.NetworkedMovement
{
    public class MobWithDollView : MobView, ISubjectWithDollView
    {
        [SerializeField] private VisualDoll _doll;
        [SerializeField] private VisualDollDefRef _dollDef;
        
        public VisualDoll Doll => _doll;

        public VisualDollDef DollDef => _dollDef.Target;
        
        public void OnPutItemInHand((BaseItemResource, Guid) item)
        {}

        public void OnRemoveItemFromHand((BaseItemResource, Guid) item)
        {}
    }
}