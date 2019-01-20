using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Boundary { Top, Bottom, Front, Back, Left, Right }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static List<Interactable> currentInteractables;
    private float levelNumber;

    public float initialWaveTime;
    public float waveTimeIncrease;

    public float initialSpawnDelay;
    public float spawnDelayDecrease;

    public float defaultLevelDowntime;

    private bool debrisStillSpawning;
    private bool playerIsAlive;
    public ObjectPool pool;
    public List<Interactable> debrisInCurrentLevel;
    public Transform junkTarget;
    public float delayTime;
    public int startingJunkCount;
    public float junkTargetVariance;
    public float orbitTime;
    public float invincibilityTime;
    public float junkSpeed;
    public float junkRotationIntensity;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float minZ;
    private float maxZ;

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
        //yield return new WaitForSeconds(delayTime);

        levelNumber = 1;
        playerIsAlive = true;
        debrisInCurrentLevel = new List<Interactable>();
        Vector3 spawnAreaCenter = spawnArea.transform.position;
        minX = spawnAreaCenter.x - spawnArea.bounds.extents.x;
        maxX = spawnAreaCenter.x + spawnArea.bounds.extents.x;

        minY = spawnAreaCenter.y - spawnArea.bounds.extents.y;
        maxY = spawnAreaCenter.y + spawnArea.bounds.extents.y;

        minZ = spawnAreaCenter.z - spawnArea.bounds.extents.z;
        maxZ = spawnAreaCenter.z + spawnArea.bounds.extents.z;

        yield return new WaitForSeconds(delayTime);

        StartCoroutine("RunGame");
    }

    public void LoadLevel()
    {
        Vector3 nextSpawnLocation;
        Vector3 realTargetLocation = junkTarget.position;
        Vector3 nextTargetLocation;
        for (int i = 0; i < startingJunkCount; i++)
        {
            nextSpawnLocation = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            Interactable junk = pool.GetFromPool();
            junk.transform.position = nextSpawnLocation;

            // X and Y will vary by 0.7 in either direction
            nextTargetLocation = realTargetLocation + new Vector3(Random.Range(-junkTargetVariance, junkTargetVariance), Random.Range(-junkTargetVariance, junkTargetVariance), 0f);
            junk.transform.LookAt(nextTargetLocation);
            junk.rigidbody.AddForce(junk.transform.forward * junkSpeed);
            junk.rigidbody.angularVelocity = new Vector3(Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity));
        }
    }

    // TODO: Decide if we want to increase debris speed or spawning area by level
    IEnumerator RunGame()
    {
        //float levelTimeElapsed; // = 0f;
        float levelDuration = initialWaveTime;
        float debrisDelay = initialSpawnDelay;
        while (playerIsAlive)
        {
            print("Starting level " + levelNumber);
            print("Debris will spawn for " + levelDuration + " seconds at a rate of one every " + debrisDelay + " seconds.");
            float levelTimeElapsed = 0f;
            float timeSinceDebrisSpawned = debrisDelay;
            debrisStillSpawning = true;
            while (levelTimeElapsed < levelDuration)
            {
                if (timeSinceDebrisSpawned >= debrisDelay)
                {
                    timeSinceDebrisSpawned -= debrisDelay;

                    // Spawn debris
                    Interactable debris = SpawnDebris();
                    debrisInCurrentLevel.Add(debris);
                    SendDebrisAtTarget(debris);
                }

                timeSinceDebrisSpawned += Time.deltaTime;
                levelTimeElapsed += Time.deltaTime;
                yield return null;
            }
            // Before ending level, wait for all currently spawned debris to pass player
            debrisStillSpawning = false;

            //while (debrisInCurrentLevel.Count > 0)
            //{
            //    print(debrisInCurrentLevel.Count + " pieces of debris remaining");
            //    yield return null;
            //}
            yield return new WaitForSeconds(15f);

            // Increase difficulty for next wave
            levelNumber++;
            levelDuration += waveTimeIncrease;
            //debrisDelay -= spawnDelayDecrease;
            debrisDelay -= debrisDelay * spawnDelayDecrease;

            // Return all debris to pool
            //foreach (Interactable debris in debrisInCurrentLevel)
            //{
            //    pool.ReturnToPool(debris);
            //}
            //debrisInCurrentLevel = new List<Interactable>();

            // Check if there's anything that should be done before starting the next level (spawning a weapon, showing tutorial text)

            // Wait a few seconds before starting next level
            yield return new WaitForSeconds(defaultLevelDowntime);
        }
    }

    public void HitBoundary(Interactable debris, Boundary boundary)
    {
        if (boundary == Boundary.Top || boundary == Boundary.Bottom || boundary == Boundary.Front)
        {
            // Return to pool
            if (debris.parentDebris != null)
            {
                bool allChildrenDestroyed = true;
                for (int i = 0; i < 4; i++)
                {
                    if (debris.parentDebris.pieces[i].beenDestroyed == false)
                    {
                        allChildrenDestroyed = false;
                    }

                    //if (allChildrenDestroyed)
                    //{
                    //    pool.ReturnToPool(debris.parentDebris);
                    //    debrisInCurrentLevel.Remove(debris.parentDebris);
                    //}
                    //else
                    //{
                    //    debris.KillByBarrier();
                    //}
                }
                if (allChildrenDestroyed)
                {
                    pool.ReturnToPool(debris.parentDebris);
                    debrisInCurrentLevel.Remove(debris.parentDebris);
                }
                else
                {
                    debris.KillByBarrier();
                }
            }
            else
            {
                pool.ReturnToPool(debris);
                debrisInCurrentLevel.Remove(debris);
            }
        }
        else if (boundary == Boundary.Back || boundary == Boundary.Left || boundary == Boundary.Right)
        {
            if (debrisStillSpawning)
            {
                // Recycle to front
                Vector3 spawnLocation = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
                debris.transform.position = spawnLocation;
                debris.rigidbody.velocity = Vector3.zero;
                debris.rigidbody.angularVelocity = Vector3.zero;
                SendDebrisAtTarget(debris);
            }
            else
            {
                // Return to pool
                if (debris.parentDebris != null)
                {
                    bool allChildrenDestroyed = true;
                    for (int i = 0; i < 4; i++)
                    {
                        if (debris.parentDebris.pieces[i].beenDestroyed == false)
                        {
                            allChildrenDestroyed = false;
                        }                        
                    }
                    if (allChildrenDestroyed)
                    {
                        pool.ReturnToPool(debris.parentDebris);
                        debrisInCurrentLevel.Remove(debris.parentDebris);
                    }
                    else
                    {
                        debris.KillByBarrier();
                    }
                }
                else
                {
                    pool.ReturnToPool(debris);
                    debrisInCurrentLevel.Remove(debris);
                }
            }
        }
    }

    //public bool RegisterHit(Interactable debris)
    //{
    //    bool allChildrenDestroyed = true;
    //    for (int i = 0; i < 4; i++)
    //    {
    //        if (debris.parentDebris.pieces[i].beenDestroyed == false)
    //        {
    //            allChildrenDestroyed = false;
    //        }

    //        if (allChildrenDestroyed)
    //        {
    //            pool.ReturnToPool(debris.parentDebris);
    //            debrisInCurrentLevel.Remove(debris.parentDebris);
    //        }
    //    }
    //}

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

    public Interactable SpawnDebris()
    {
        Vector3 spawnLocation = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        Interactable debris = pool.GetFromPool();
        debris.transform.position = spawnLocation;
        return debris;
    }

    public void SendDebrisAtTarget(Interactable interactable)
    {
        Vector3 targetLocation = junkTarget.position + new Vector3(Random.Range(-junkTargetVariance, junkTargetVariance), Random.Range(-junkTargetVariance, junkTargetVariance), 0f);
        interactable.gameObject.transform.LookAt(targetLocation);
        interactable.rigidbody.AddForce(interactable.gameObject.transform.forward * junkSpeed);
        interactable.rigidbody.angularVelocity = new Vector3(Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity), Random.Range(-junkRotationIntensity, junkRotationIntensity));
    }

}