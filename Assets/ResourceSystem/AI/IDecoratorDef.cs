using Assets.Src.ResourcesSystem.Base;

namespace Assets.Src.RubiconAI.BehaviourTree.NodeTypes
{
    public interface IDecoratorDef
    {
        ResourceRef<BehaviourNodeDef> SubNode { get; }
    }
}
