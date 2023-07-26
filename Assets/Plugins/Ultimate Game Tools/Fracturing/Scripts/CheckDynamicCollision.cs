using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This component will enable fracturable objects with dynamic properties
/// </summary>
public class CheckDynamicCollision : MonoBehaviour
{
    void Start()
    {
        // Enable fracturable object collider

        FracturedObject fracturedObject = GetComponent<FracturedObject>();

        // Disable chunk colliders

        if(fracturedObject != null)
        {
            if(fracturedObject.GetComponent<Collider>() != null)
            {
                fracturedObject.GetComponent<Collider>().enabled = true;
            }
            else
            {
                Debug.LogWarning("Fracturable Object " + gameObject.name + " has a dynamic rigidbody but no collider. Object will not be able to collide.");
            }

            for(int i = 0; i < fracturedObject.ListFracturedChunks.Count; i++)
            {
                EnableObjectColliders(fracturedObject.ListFracturedChunks[i].gameObject, false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.contacts == null)
        {
            return;
        }

        if(collision.contacts.Length == 0)
        {
            return;
        }

        // Was it a big enough hit?

        FracturedObject fracturedObject = gameObject.GetComponent<FracturedObject>();

        if(fracturedObject != null)
        {
            float fMass = collision.rigidbody != null ? collision.rigidbody.mass : Mathf.Infinity;
            
            if(collision.relativeVelocity.magnitude > fracturedObject.EventDetachMinVelocity && fMass > fracturedObject.EventDetachMinVelocity)
            {
                // Disable fracturable object collider

                fracturedObject.GetComponent<Collider>().enabled = false;

                Rigidbody fracturableRigidbody = fracturedObject.GetComponent<Rigidbody>();

                if(fracturableRigidbody != null)
                {
                    fracturableRigidbody.isKinematic = true;
                }

                // Enable chunk colliders

                for(int i = 0; i < fracturedObject.ListFracturedChunks.Count; i++)
                {
                    EnableObjectColliders(fracturedObject.ListFracturedChunks[i].gameObject, true);
                }

                // Explode

                fracturedObject.Explode(collision.contacts[0].point, collision.relativeVelocity.magnitude);
            }
        }
    }

    private void EnableObjectColliders(GameObject chunk, bool bEnable)
    {
        List<Collider> chunkColliders = new List<Collider>();

        SearchForAllComponentsInHierarchy<Collider>(chunk, ref chunkColliders);

        for(int i = 0; i < chunkColliders.Count; ++i)
        {
            chunkColliders[i].enabled = bEnable;

            if(bEnable)
            {
                chunkColliders[i].isTrigger = false;
            }
        }
    }

    private static void SearchForAllComponentsInHierarchy<T>(GameObject current, ref List<T> listOut) where T : Component
    {
      T myComponent = current.GetComponent<T>();

      if (myComponent != null)
      {
        listOut.Add(myComponent);
      }

      for (int i = 0; i < current.transform.childCount; ++i)
      {
        SearchForAllComponentsInHierarchy(current.transform.GetChild(i).gameObject, ref listOut);
      }
    }
}
