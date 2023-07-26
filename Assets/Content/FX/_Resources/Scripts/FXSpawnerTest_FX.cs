using UnityEngine;

[ExecuteInEditMode]

public class FXSpawnerTest_FX : MonoBehaviour {

    public GameObject whatToSpawn;
    public Transform whereToSpawn;
    public Vector3 offset;
    private GameObject spawned;

    public Vector3 spitRotation;
    public GameObject spitToSpawn;
    public Transform whereToSpawnSpit;    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!spawned)
            {                
                spawned = Instantiate(whatToSpawn, whereToSpawn.position + offset, Quaternion.Euler(-90, 0, 0));
            }
            else
            {
                Destroy(spawned);
                spawned = null;
            }
        }
    }

    public void SpawnSpit()
    {        
        Instantiate(spitToSpawn, whereToSpawnSpit);
    }
}
