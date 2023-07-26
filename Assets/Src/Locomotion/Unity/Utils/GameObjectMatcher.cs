using UnityEngine;

namespace Src.Locomotion.Unity
{
    public readonly struct GameObjectMatcher
    {
        private readonly LayerMask _layers;
        private readonly LayerMask _layersNot;
        private readonly string[] _tags;
        private readonly string[] _tagsNot;

        public GameObjectMatcher(LayerMask layers, string[] tags = null, LayerMask layersNot = new LayerMask(), string[] tagsNot = null)
        {
            _layers = layers;
            _tags = tags;
            _layersNot = layersNot;
            _tagsNot = tagsNot;
        }

        public LayerMask Layers => _layers;

        public bool IsMatch(GameObject go)
        {
            if (_layersNot != 0 && ((1 << go.layer) & _layersNot) != 0)
                return false;

            if (_layers != 0 && ((1 << go.layer) & _layers) == 0)
                return false;
            
            if (_tagsNot != null)
                foreach (var tag in _tagsNot)
                    if (go.CompareTag(tag))
                        return false;

            if (_tags == null)
                return true;

            foreach (var tag in _tags)
                if (go.CompareTag(tag))
                    return true;

            return false;
        }

        public bool IsMatch(Component co)
        {
            return IsMatch(co.gameObject);
        }
    }
}