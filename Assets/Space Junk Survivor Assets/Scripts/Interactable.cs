using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand m_ActiveHand = null;
    public bool canShatter;
    public Transform intact;
    public List<Transform> pieces;

    private void OnTriggerEnter(Collider other)
    {
        if (m_ActiveHand && other.gameObject.CompareTag("Wieldable"))
        {
            other.gameObject.GetComponent<Interactable>().Hit();
        }
    }

    public void Hit()
    {
        if (canShatter)
        {
            intact.gameObject.SetActive(false);
            foreach (Transform t in pieces)
            {
                t.gameObject.SetActive(true);
                t.gameObject.GetComponent<Interactable>().Activate();
                //StartCoroutine("WaitToTurnOnColliders");
                //t.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        StartCoroutine("WaitToTurnOnColliders");
    }

    IEnumerator WaitToTurnOnColliders()
    {
        //yield return new WaitForSeconds(0.25f);

        GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);

        //foreach (Transform t in pieces)
        //{
            //t.gameObject.SetActive(true);
            //t.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            //t.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);
        //}

        yield return new WaitForSeconds(0.25f);

        GetComponent<BoxCollider>().enabled = true;

        //foreach (Transform t in pieces)
        //{
        //    t.GetComponent<BoxCollider>().enabled = true;
        //}
    }
}