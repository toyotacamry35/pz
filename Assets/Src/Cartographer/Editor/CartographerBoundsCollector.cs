using UnityEngine;

namespace Assets.Src.Cartographer.Editor
{
    public class CartographerBoundsCollectior
    {
        public bool Collected { get; private set; } = false;
        public Bounds Bounds { get; private set; } = new Bounds();
        public void Reset()
        {
            Collected = false;
            Bounds = new Bounds();
        }

        public void Collect(Bounds bounds)
        {
            if (Collected)
            {
                Bounds.Encapsulate(bounds);
            }
            else
            {
                Collected = true;
                Bounds = bounds;
            }
        }
    }
};