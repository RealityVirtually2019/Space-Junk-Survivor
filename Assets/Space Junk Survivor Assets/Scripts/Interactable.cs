using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand attachedHand;
    public bool partOfLargerObject;
    public bool canShatter;
    public Vector3 currentVelocity;
    public Rigidbody rigidbody;
    public BoxCollider collider;
    //public Transform intact; // Will instance always just be the object the script is attached to?
    public List<Transform> pieces;

    private void OnTriggerEnter(Collider other)
    {
        if (attachedHand && other.gameObject.CompareTag("Wieldable"))
        {
            other.gameObject.GetComponent<Interactable>().Hit();
        }
        else if (other.gameObject.CompareTag("Boundary"))
        {
            if (other.gameObject.name.Equals("Top Boundary") || other.gameObject.name.Equals("Bottom Boundary"))
            {
                other.gameObject.SetActive(false);
            }
            else if (other.gameObject.name.Equals("Back Boundary"))
            {
                // Wait and respawn in front
            }
            else if (other.gameObject.name.Equals("Left Boundary"))
            {
                // Wait and respawn on the right
            }
            else if (other.gameObject.name.Equals("Right Boundary"))
            {
                // Wait and respawn on the left
            }
        }
    }

    IEnumerator HoldUntilOrbited()
    {
        yield return new WaitForSeconds(GameManager.instance.orbitTime);
    }

    public void Hit()
    {
        if (canShatter)
        {
            //intact.gameObject.SetActive(false);
            gameObject.SetActive(false);
            foreach (Transform t in pieces)
            {
                t.gameObject.SetActive(true);
                t.gameObject.GetComponent<Interactable>().Activate();
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
        //GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);
        rigidbody.velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);

        yield return new WaitForSeconds(GameManager.instance.invincibilityTime);

        //GetComponent<BoxCollider>().enabled = true;
        collider.enabled = true;
    }
}