using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public List<GameObject> objectsToPool;
    //public GameObject objectToPool;
    public int amountToPool;
    private List<Interactable> pool;

    void Start()
    {
        pool = new List<Interactable>();

        for (int i = 0; i < amountToPool; i++)
        {
            int index = Random.Range(0, objectsToPool.Count);
            //pool.Add(Instantiate(objectToPool).transform.GetChild(0).GetComponent<Interactable>());
            //pool.Add(Instantiate(objectToPool).GetComponent<Interactable>());
            pool.Add(Instantiate(objectsToPool[index]).GetComponent<Interactable>());
            pool[i].gameObject.SetActive(false);
        }
    }

    public Interactable GetFromPool()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
            {
                pool[i].gameObject.SetActive(true);
                return pool[i];
            }
        }

        //Interactable newObject = Instantiate(objectToPool).transform.GetChild(0).GetComponent<Interactable>();
        //Interactable newObject = Instantiate(objectToPool).GetComponent<Interactable>();
        int index = Random.Range(0, objectsToPool.Count);
        Interactable newObject = Instantiate(objectsToPool[index]).GetComponent<Interactable>();
        pool.Add(newObject);
        return newObject;
    }

    // Restores debris to defaults and allows it to be drawn from the pool again when needed
    public void ReturnToPool(Interactable debris)
    {
        // Enable collider and mesh of parent
        debris.collider.enabled = true;
        debris.renderer.enabled = true;

        // Turn parent on
        debris.gameObject.SetActive(true);

        // Set parent position to Vector3.zero (probably not necessary)
        debris.transform.position = debris.defaultPosition;

        // Disable any velocity and angular velocity
        debris.rigidbody.velocity = Vector3.zero;
        debris.rigidbody.angularVelocity = Vector3.zero;

        foreach (Interactable piece in debris.pieces)
        {
            // Disable collider and enable mesh of children
            piece.collider.enabled = false; // Children debris pieces have their mesh disabled until after they've separated
            piece.renderer.enabled = true;

            // Turn children off
            piece.gameObject.SetActive(false);

            // Restore children to default positions
            piece.transform.position = piece.defaultPosition;

            // Disable any velocity and angular velocity
            piece.rigidbody.velocity = Vector3.zero;
            piece.rigidbody.angularVelocity = Vector3.zero;
        }

        debris.gameObject.SetActive(false);
    }

}