using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand attachedHand;
    public bool partOfLargerObject;
    public bool canShatter;
    public float velocityMultiplier;
    public ParticleSystem particleSystem;
    //public Vector3 currentVelocity;
    public MeshRenderer renderer;
    public Rigidbody rigidbody;
    public BoxCollider collider;
    //public Transform intact; // Will instance always just be the object the script is attached to?
    //public List<Transform> pieces; // Why references to Transforms and not Interactables?
    public List<Interactable> pieces;
    public Vector3 defaultPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (attachedHand && other.gameObject.CompareTag("Wieldable"))
        {
            print(other.gameObject.name + " just got hit");
            attachedHand.TriggerVibration();
            other.gameObject.GetComponent<Interactable>().Hit(this);
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
        GameManager.instance.SendDebrisAtTarget(this);
        //rigidbody.velocity = savedVelocity;
        //transform.LookAt(GameManager.instance.GetJunkTarget());
    }

    public void Hit(Interactable hittingBody)
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
        if (canShatter)
        {
            //intact.gameObject.SetActive(false);
            //gameObject.SetActive(false);
            renderer.enabled = false;
            collider.enabled = false;
            //print(gameObject.name + "'s velocity was: " + rigidbody.velocity);
            //print("And the object that hit it had velocity: " + hittingBody.velocity);
            Vector3 oldVelocity = rigidbody.velocity;
            rigidbody.velocity = Vector3.zero;
            Vector3 newVelocity;
            if (hittingBody.attachedHand)
            {
                // Get velocity from hand
                //if (attachedHand != null)
                //{
                //    print("Successfully found hand");
                //}
                //if (attachedHand.pose != null)
                //{
                //    print("Successfully found pose");
                //}
                //if (attachedHand.pose.GetVelocity() != null)
                //{
                //    print("Successfully found velocity");
                //}
                newVelocity = hittingBody.attachedHand.pose.GetVelocity();
            }
            else
            {
                // Get velocity from object
                newVelocity = hittingBody.rigidbody.velocity;
            }
            newVelocity *= hittingBody.velocityMultiplier;

            foreach (Interactable piece in pieces)
            {
                piece.gameObject.SetActive(true);
                piece.Activate(oldVelocity, newVelocity);
            }
        }
        else
        {
            //gameObject.SetActive(false);
            renderer.enabled = false;
            collider.enabled = false;
            StartCoroutine("WaitToDeactivateGO");
        }
    }

    public void Activate(Vector3 parentVelocity, Vector3 newVelocity)
    {
        //rigidbody.velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), -6f) - hittingBody.velocity;
        //rigidbody.velocity = -hittingBody.velocity;
        //print("Velocity of object hit was: " + parentVelocity);
        //print("Velocity of wrench was: " + newVelocity);
        //rigidbody.velocity = parentVelocity + newVelocity;
        rigidbody.velocity = new Vector3(parentVelocity.x + (newVelocity.x * Random.Range(0.4f, 0.8f)), parentVelocity.y + (newVelocity.y * Random.Range(0.4f, 0.8f)), parentVelocity.z + velocityMultiplier);
        //rigidbody.AddForce(newVelocity);
        StartCoroutine("WaitToTurnOnColliders");
    }

    IEnumerator WaitToTurnOnColliders()
    {
        //GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-0.5f, 0.5f), 0f, -1f);
        //rigidbody.velocity = new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), -6f);

        yield return new WaitForSeconds(GameManager.instance.invincibilityTime);

        //GetComponent<BoxCollider>().enabled = true;
        collider.enabled = true;
    }

    IEnumerator WaitToDeactivateGO()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}