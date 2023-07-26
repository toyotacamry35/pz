using AwesomeTechnologies.Colliders;

namespace AwesomeTechnologies
{
    public partial class VegetationSystem
    {
        public void RefreshColliders()
        {
            ColliderSystem colliderSystem = gameObject.GetComponent<ColliderSystem>();
            if (colliderSystem)
            {
                colliderSystem.RefreshColliders();
            }
        }
    }
}
