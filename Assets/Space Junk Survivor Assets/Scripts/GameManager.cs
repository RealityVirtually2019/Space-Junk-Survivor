using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boundary { Front, Back, Left, Right }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ObjectPool pool;
    public Transform junkTarget;
    public int startingJunkCount;
    public float junkTargetVariance;
    public float orbitTime;
    public float invincibilityTime;
    public float junkSpeed;
    public float junkRotationIntensity;

    public Transform frontBoundary;
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

        Vector3 nextSpawnLocation;
        Vector3 realTargetLocation = junkTarget.position;
        Vector3 nextTargetLocation;
        for (int i = 0; i < startingJunkCount; i++)
        {
            nextSpawnLocation = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            //print("Spawning cube " + i + " at " + nextSpawnLocation);
            Interactable junk = pool.GetFromPool();
            junk.transform.position = nextSpawnLocation;

            // X and Y will vary by 0.7 in either direction
            nextTargetLocation = realTargetLocation + new Vector3(Random.Range(-junkTargetVariance, junkTargetVariance), Random.Range(-junkTargetVariance, junkTargetVariance), 0f);
            //junk.transform.LookAt(junkTarget);
            junk.transform.LookAt(nextTargetLocation);
            junk.rigidbody.AddForce(junk.transform.forward * junkSpeed);
            //junk.rigidbody.AddRelativeForce(transform.forward * junkSpeed);
            junk.rigidbody.angularVelocity = new Vector3(Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity));
        }
    }

    public Vector3 GetRecycleLocation(Boundary boundary, Vector3 currentLocation)
    {
        if (boundary == Boundary.Back)
        {
            return new Vector3(-currentLocation.x * 6f, currentLocation.y * 6f, frontBoundary.position.z);
        }
        else
        {
            print("Unknown barrier hit");
            return Vector3.zero;
        }
    }

    public void SendJunkAtTarget(Interactable interactable)
    {
        Vector3 targetLocation = junkTarget.position + new Vector3(Random.Range(-junkTargetVariance, junkTargetVariance), Random.Range(-junkTargetVariance, junkTargetVariance), 0f);
        interactable.gameObject.transform.LookAt(targetLocation);
        interactable.rigidbody.AddForce(interactable.gameObject.transform.forward * junkSpeed);
    }

}