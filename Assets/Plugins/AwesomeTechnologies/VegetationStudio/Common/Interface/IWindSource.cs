using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindSource
{
    void RegisterObject(GameObject go);
    void UnregisterObject(GameObject go);
}
