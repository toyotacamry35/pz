using UnityEngine;

namespace Assets.Src.GameObjectAssembler.TestComponents
{
    public class TestComponent1 : MonoBehaviour
    {
        public TestComponent2 m_Comp2Ref;
        public MonoBehaviour Ref { get; set; }
    }
}
