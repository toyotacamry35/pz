using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeCollision_FX : MonoBehaviour {

    private Transform player;

    [Header("PARTICLES AND DECALS")]
    [Header("")]
    public ParticleSystem sparks;
    public ParticleSystem blood;
    public ParticleSystem axe_blood;
    public ParticleSystem blood_swipe;
    public ParticleSystem dust;
    public GameObject test;
    public GameObject decal;
    private ParticleSystem selectedParticles;


    [Header("INTERNAL")]
    [Header("")]
    public bool isAttacking = false;
    public bool isSpawned = false;
    
    
    

    [Header("PLACEMENT")]
    [Header("")]
    private Vector3 offset;
    public float offsetAmount;
    public float offsetAmountCamera;

    public GameObject sceneCamera;
    private Vector3 cameraVector;

    

    void Start () {
        //restrict collision between axe and player capsule   
        player = this.transform.root;
        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());	
	}

    private void Update() {
        //get camera vector and multiply it with offset amount for correct spawn point
        cameraVector = sceneCamera.transform.forward;
        offset = cameraVector * offsetAmount * -1f;
    }


    private void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject.name;


        //check what object collision is with
        switch (obj) {
            case "Doppelganger":
                selectedParticles = blood;
                break;
            default:
                selectedParticles = sparks;
                break;
        }

        //spawn particles and decals
        if (isAttacking & isSpawned) {
            
            if (selectedParticles == blood)
            {
                Instantiate(selectedParticles, collision.contacts[0].point + offset, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
                var axeBlood = Instantiate(axe_blood, gameObject.transform.position, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
                axeBlood.transform.parent = gameObject.transform;
                Instantiate(blood_swipe, collision.contacts[0].point + offset*2, Quaternion.identity);
                Instantiate(decal, collision.contacts[0].point, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
                //Instantiate(test, collision.contacts[0].point, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
            }            
            else
            {
                Instantiate(selectedParticles, collision.contacts[0].point + offset, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
                Instantiate(decal, collision.contacts[0].point, Quaternion.LookRotation(Vector3.up, collision.contacts[0].normal));
            }
            
            isSpawned = false;
        }
        else return;
        
    }

}
