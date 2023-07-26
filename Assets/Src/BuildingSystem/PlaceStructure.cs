using SharedCode.Entities.GameObjectEntities;
using System;
using System.Collections.Generic;

namespace Assets.Src.BuildingSystem
{
    public abstract class PlaceStructure<PlaceDefType, ElementDataType>
        where PlaceDefType : IEntityObjectDef
    {
        private Dictionary<Guid, ElementDataType> elements = new Dictionary<Guid, ElementDataType>();

        public bool Add(Guid key, ElementDataType data)
        {
            elements.Add(key, data);
            ElementAdded(key, data);
            return true;
        }

        public bool TryGetValue(Guid key, out ElementDataType data)
        {
            return elements.TryGetValue(key, out data);
        }

        public bool Remove(Guid key)
        {
            ElementDataType data;
            if (elements.TryGetValue(key, out data))
            {
                bool result = elements.Remove(key);
                ElementRemoved(key, data);
                return result;
            }
            return false;
        }

        public void ForEach(Action<ElementDataType> action)
        {
            foreach (var keyValuePair in elements)
            {
                action(keyValuePair.Value);
            }
        }

        protected abstract void ElementAdded(Guid key, ElementDataType data);
        protected abstract void ElementRemoved(Guid key, ElementDataType data);

        public abstract void Bind(PlaceDefType placeDef, SharedCode.Utils.Vector3 Position, SharedCode.Utils.Quaternion Rotation);
        public abstract void Unbind(PlaceDefType placeDef);

        public abstract void SetVisualCheat(bool enable);
    }
}