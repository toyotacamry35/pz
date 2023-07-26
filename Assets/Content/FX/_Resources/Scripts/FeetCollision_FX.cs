using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollision_FX : MonoBehaviour
{

    private Transform player;

    [Header("PARTICLES AND DECALS")]
    [Header("")]
    public ParticleSystem dust;
    private ParticleSystem selectedParticles;


    [Header("INTERNAL")]
    [Header("")]
    public bool isAttacking = false;
    public bool isSpawned = false;




    [Header("PLACEMENT")]
    [Header("")]
    private Vector3 offset;
    public float offsetAmount;

    public GameObject sceneCamera;
    private Vector3 cameraVector;



    void Start()
    {
        //restrict collision between axe and player capsule   
        player = this.transform.root;
        Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());

        selectedParticles = dust;
    }

    private void Update()
    {
        //get camera vector and multiply it with offset amount for correct spawn point
        cameraVector = sceneCamera.transform.forward;
        offset = cameraVector * offsetAmount * -1f;
    }


    private void OnCollisionEnter(Collision collision)
    {
        //spawn particles and decals
        if (isAttacking & isSpawned)
        {
            Instantiate(selectedParticles, collision.contacts[0].point + offset, Quaternion.identity);
            isSpawned = false;
        }
        else return;
    }

}
