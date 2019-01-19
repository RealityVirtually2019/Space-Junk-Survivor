using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ObjectPool pool;
    public float orbitTime;
    public float invincibilityTime;

    public BoxCollider spawnArea;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //DontDestroyOnLoad(this.gameObject);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2.5f);
        LoadLevel();
    }

    public void LoadLevel()
    {
        Vector3 spawnAreaCenter = spawnArea.transform.position;
        float minX = spawnAreaCenter.x - spawnArea.bounds.extents.x;
        float maxX = spawnAreaCenter.x + spawnArea.bounds.extents.x;

        float minY = spawnAreaCenter.y - spawnArea.bounds.extents.y;
        float maxY = spawnAreaCenter.y + spawnArea.bounds.extents.y;

        float minZ = spawnAreaCenter.z - spawnArea.bounds.extents.z;
        float maxZ = spawnAreaCenter.z + spawnArea.bounds.extents.z;

        //print("minX: " + minX);
        //print("maxX: " + maxX);
        //print("minY: " + minY);
        //print("maxY: " + maxY);
        //print("minZ: " + minZ);
        //print("maxZ: " + maxZ);


        Vector3 nextSpawnLocation; // = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        for (int i = 0; i < 30; i++)
        {
            nextSpawnLocation = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            print("Spawning cube " + i + " at " + nextSpawnLocation);
            Interactable junk = pool.GetFromPool();
            //junk.transform.position = nextSpawnLocation;
            junk.rigidbody.position = nextSpawnLocation;

            //junk.rigidbody.velocity = Vector3.zero;
            //junk.rigidbody.angularVelocity = Vector3.zero;

            junk.rigidbody.isKinematic = false;
            junk.transform.LookAt(Vector3.zero);
            //junk.rigidbody.velocity = new Vector3(-1f, 0f, 0f);
            //junk.rigidbody.angularVelocity = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        }
    }

}