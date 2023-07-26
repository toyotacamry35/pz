using Assets.Src.ResourcesSystem.Base;
using JetBrains.Annotations;
using SharedCode.Utils;

namespace Assets.ColonyShared.SharedCode.Arithmetic.Calcers
{
    public class CalcerVectorComponentDef : CalcerDef<float>
    {
        public enum VectorComponent { X, Y, Z }
        
        public ResourceRef<CalcerDef<Vector3>> Vector { get; [UsedImplicitly] set; }
        public VectorComponent Component { get; [UsedImplicitly] set; }
    }
    
    
    public class CalcerVector2ComponentDef : CalcerDef<float>
    {
        public enum VectorComponent { X, Y }
        
        public ResourceRef<CalcerDef<Vector2>> Vector { get; [UsedImplicitly] set; }
        public VectorComponent Component { get; [UsedImplicitly] set; }
    }
}