using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayDebugger
{
    public class GameObjectId : ObjectId
    {
        object _go;
        public GameObjectId(GameObject go)
        {
            _go = go;
        }
        public override object Get()
        {
            return _go;
        }

        public override bool Is(object obj)
        {
            return _go == obj;
        }
    }

}