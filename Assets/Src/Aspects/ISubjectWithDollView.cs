using System;
using JetBrains.Annotations;
using ResourceSystem.Aspects.Misc;
using SharedCode.Aspects.Item.Templates;

namespace Assets.Src.Aspects
{
    public interface ISubjectWithDollView : ISubjectView
    {
        VisualDoll Doll { get; }
        
        [CanBeNull] VisualDollDef DollDef { get; }
        
        void OnPutItemInHand((BaseItemResource, Guid) item);
        
        void OnRemoveItemFromHand((BaseItemResource, Guid) item);
    }
}