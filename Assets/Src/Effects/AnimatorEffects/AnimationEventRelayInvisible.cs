using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventRelayInvisible : MonoBehaviour
{
    [SerializeField] Component[] _components;
    [SerializeField] GameObject[] _objects;


    void Invisible(AnimationEvent evt)
    {
        Switch(false);
    }

    void Visible(AnimationEvent evt)
    {
        Switch(true);
    }

    void Switch(bool enable)
    {
        foreach (var c in _components)
            if (c)
            {
                switch (c)
                {
                    case MonoBehaviour mb:
                        mb.enabled = enable;
                        break;
                    case Renderer r:
                        r.enabled = enable;
                        break;
                    case Animator anim:
                        anim.enabled = enable;
                        break;
                    case Collider col:
                        col.enabled = enable;
                        break;
                    default:
                        throw new NotSupportedException($"Object:{c.name} Type:{c.GetType()}");
                }
            }
        foreach (var c in _objects)
            if (c)
                c.SetActive(enable);

    }
}

