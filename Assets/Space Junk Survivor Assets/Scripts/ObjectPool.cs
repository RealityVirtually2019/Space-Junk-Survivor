using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectToPool;
    public int amountToPool;
    private List<Interactable> pool;

    void Start()
    {
        pool = new List<Interactable>();

        for (int i = 0; i < amountToPool; i++)
        {
            pool.Add(Instantiate(objectToPool).transform.GetChild(0).GetComponent<Interactable>());
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

        Interactable newObject = Instantiate(objectToPool).transform.GetChild(0).GetComponent<Interactable>();
        pool.Add(newObject);
        return newObject;
    }

}