using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TEMP SHIT FOR DENIS, PLEASE DELETE THIS

public class Bullet : MonoBehaviour {

    public Object fxStart;
    public Object fxImpact;
    public Transform trailRenderer;
    public float force;
    public float duration;
    public Rigidbody rigidbody;

    private float timeCurrent;
	// Use this for initialization
	void Start () {

        if (fxStart)
        {
            GameObject fxStartInstance = (GameObject)GameObject.Instantiate(fxStart);
            fxStartInstance.transform.position = transform.position;
            fxStartInstance.transform.rotation = transform.rotation;
        }
		
		Camera mainCam = Camera.main;
		if (mainCam!=null)
		{
			//Vector3 p = mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth/2, mainCam.pixelHeight / 2, -mainCam.farClipPlane));
			//Vector3 dir = Vector3.Normalize(p- transform.position);
			//transform.rotation.SetLookRotation(dir);
            
            timeCurrent = Time.realtimeSinceStartup;
            rigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
        }
	}

    void Update () {
        if (Time.realtimeSinceStartup - timeCurrent > 20)
        {
            if (trailRenderer != null)
            trailRenderer.transform.parent = null;
            Destroy(this.gameObject);
        }
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (fxImpact != null)
        {
            GameObject fxImpactInstance = (GameObject)GameObject.Instantiate(fxImpact);
            fxImpactInstance.transform.position = transform.position;
            fxImpactInstance.transform.rotation = transform.rotation;
        }
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.Sleep();
    }
}
