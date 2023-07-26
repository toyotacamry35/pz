using System;
using Assets.Src.Aspects;
using Assets.Src.NetworkedMovement;
using ResourceSystem.Utils;
using Src.Aspects.Impl;

namespace Src.Debugging
{
    public static class EntityInDebugFocus
    {
        private static IEntityPawn _pawn;

//        public static ReactiveProperty<CharacterClientView> Character = new ReactiveProperty<CharacterClientView>();
//        public static ReactiveProperty<Pawn> Pawn = new ReactiveProperty<Pawn>();
//        public static IStream<OuterRef> EntityRef = FuncStreamExtensions.ZipFunc<CharacterClientView, Pawn, OuterRef>(Character, null, Pawn, (c, m) => c ? c.GetOuterRef<IEntity>() : m ? m.GetOuterRef<IEntity>() : default);
        
        public static event Action<(IEntityPawn oldPawn, IEntityPawn newPawn)> Changed = delegate {};

        public static IEntityPawn Entity
        {
            get => _pawn;
            set
            {
                if (_pawn != value)
                {
                    var oldPawn = _pawn;
                    _pawn = value;
                    Changed.Invoke((oldPawn, _pawn));
                }
            }
        }

        public static ISubjectPawn Subject { get => Entity as ISubjectPawn; set => Entity = value; }

        public static ICharacterPawn Character { get => Entity as ICharacterPawn; set => Entity = value; }

        public static IMobPawn Mob { get => Entity as IMobPawn; set => Entity = value; }
        
        public static OuterRef EntityRef => _pawn?.EntityRef ?? default;
    }
}