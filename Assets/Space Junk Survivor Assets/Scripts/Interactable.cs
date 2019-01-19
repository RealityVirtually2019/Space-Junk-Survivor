using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand attachedHand;
    public bool partOfLargerObject;
    public bool canShatter;
    //public Vector3 currentVelocity;
    public Rigidbody rigidbody;
    public BoxCollider collider;
    //public Transform intact; // Will instance always just be the object the script is attached to?
    public List<Transform> pieces;

    private void OnTriggerEnter(Collider other)
    {
        if (attachedHand && other.gameObject.CompareTag("Wieldable"))
        {
            print(gameObject.name + " just got hit");
            other.gameObject.GetComponent<Interactable>().Hit();
        }
        else if (other.gameObject.CompareTag("Boundary"))
        {
            if (other.gameObject.name.Equals("Top Boundary") || other.gameObject.name.Equals("Bottom Boundary"))
            {
                gameObject.SetActive(false);
                //Hit();
            }
            else if (other.gameObject.name.Equals("Back Boundary"))
            {
                // Wait and respawn in front
                Vector3 newPos = GameManager.instance.GetRecycleLocation(Boundary.Back, transform.position);
                StartCoroutine(HoldUntilOrbited(newPos));
            }
            else if (other.gameObject.name.Equals("Left Boundary"))
            {
                // Wait and respawn on the right
                //Vector3 initialPosition = transform.position;
                
            }
            else if (other.gameObject.name.Equals("Right Boundary"))
            {
                // Wait and respawn on the left
            }
        }
    }

    IEnumerator HoldUntilOrbited(Vector3 newPos)
    {
        //Vector3 savedVelocity = rigidbody.velocity;
        rigidbody.velocity = Vector3.zero;
        //yield return new WaitForSeconds(GameManager.instance.orbitTime);
        yield return null;

        transform.position = newPos;
        GameManager.instance.SendJunkAtTarget(this);
        //rigidbody.velocity = savedVelocity;
        //transform.LookAt(GameManager.instance.GetJunkTarget());
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
        //rigidbody.velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);

        yield return new WaitForSeconds(GameManager.instance.invincibilityTime);

        //GetComponent<BoxCollider>().enabled = true;
        collider.enabled = true;
    }
}