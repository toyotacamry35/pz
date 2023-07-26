using System.Linq;
using Assets.ColonyShared.SharedCode.Arithmetic.Calcers;
using Assets.Src.ResourcesSystem.Base;
using ColonyShared.SharedCode;
using ResourcesSystem.Loader;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers
{
    
/*    Не работает с ProtoBuf: System.InvalidOperationException: It was not possible to prepare a serializer for: SharedCode.EntitySystem.IDeltaObject ---> System.InvalidOperationException: No serializer defined for type: Assets.ColonyShared.SharedCode.Arithmetic.Templates.Calcers.SaveableCalcerDef`1
    Поэтому каждый тип ручками...
    
    public class SaveableCalcerDef<T> : SaveableBaseResource
    {
        public ResourceRef<CalcerDef<T>> Calcer;
        
        public static readonly SaveableCalcerDef<T> Empty = GameResourcesHolder.Instance.LoadResource<SaveableCalcerDef<T>>($"/UtilPrefabs/EmptyCalcer{Value.SupportedTypes.Single(x => x.Item2 == typeof(T)).Item1.ToString()}");
    }
  */

    public class SaveableCalcerFloatDef : SaveableBaseResource
    {
        public ResourceRef<CalcerDef<float>> Calcer;

        public static SaveableCalcerFloatDef Empty => _Empty.Target;

        private static readonly ResourceRef<SaveableCalcerFloatDef> _Empty = new ResourceRef<SaveableCalcerFloatDef>("/UtilPrefabs/EmptyCalcerFloat");
    }

    public class SaveableCalcerResourceDef : SaveableBaseResource
    {
        public ResourceRef<CalcerResourceDef> Calcer;
        
        public static SaveableCalcerResourceDef Empty => _Empty.Target;

        private static readonly ResourceRef<SaveableCalcerResourceDef> _Empty = new ResourceRef<SaveableCalcerResourceDef>("/UtilPrefabs/EmptyCalcerResource");
    }

}
