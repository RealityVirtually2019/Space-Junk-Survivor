using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand attachedHand;
    public bool beenDestroyed;
    public Interactable parentDebris;
    public bool canShatter;
    public float velocityMultiplier;
    public ParticleSystem particleSystem;
    public MeshRenderer renderer;
    public Rigidbody rigidbody;
    public BoxCollider collider;
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
            //if (other.gameObject.name.Equals("Top Boundary") || other.gameObject.name.Equals("Bottom Boundary"))
            //{
            //    gameObject.SetActive(false);
            //}
            //else if (other.gameObject.name.Equals("Back Boundary"))
            //{
            //    Vector3 newPos = GameManager.instance.GetRecycleLocation(Boundary.Back, transform.position);
            //    StartCoroutine(HoldUntilOrbited(newPos));
            //}
            //else if (other.gameObject.name.Equals("Left Boundary"))
            //{
            //    // Wait and respawn on the right

            //}
            //else if (other.gameObject.name.Equals("Right Boundary"))
            //{
            //    // Wait and respawn on the left
            //}
            Boundary boundary;
            if (other.gameObject.name.Equals("Top Boundary"))
            {
                boundary = Boundary.Top;
            }
            else if (other.gameObject.name.Equals("Bottom Boundary"))
            {
                boundary = Boundary.Bottom;
            }
            else if (other.gameObject.name.Equals("Front Boundary"))
            {
                boundary = Boundary.Front;
            }
            else if (other.gameObject.name.Equals("Back Boundary"))
            {
                boundary = Boundary.Back;
            }
            else if (other.gameObject.name.Equals("Left Boundary"))
            {
                boundary = Boundary.Left;
            }
            //else if (other.gameObject.name.Equals("Right Boundary"))
            //{
            //    boundary = Boundary.Right;
            //}
            else
            {
                boundary = Boundary.Right;
            }
            GameManager.instance.HitBoundary(this, boundary);
        }
    }

    IEnumerator HoldUntilOrbited(Vector3 newPos)
    {
        rigidbody.velocity = Vector3.zero;
        yield return null;

        transform.position = newPos;
        GameManager.instance.SendDebrisAtTarget(this);
    }

    public void Hit(Interactable hittingBody)
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        renderer.enabled = false;
        collider.enabled = false;
        beenDestroyed = true;

        if (canShatter)
        {
            //renderer.enabled = false;
            //collider.enabled = false;

            Vector3 oldVelocity = rigidbody.velocity;
            rigidbody.velocity = Vector3.zero;
            Vector3 newVelocity;
            if (hittingBody.attachedHand)
            {
                // Get velocity from player hand
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
            //renderer.enabled = false;
            //collider.enabled = false;
            StartCoroutine("WaitToDeactivateGO");
        }
    }

    public void KillByBarrier()
    {
        renderer.enabled = false;
        collider.enabled = false;
        beenDestroyed = true;
    }

    public void Activate(Vector3 parentVelocity, Vector3 newVelocity)
    {
        rigidbody.velocity = new Vector3(parentVelocity.x + (newVelocity.x * Random.Range(0.4f, 0.8f)), parentVelocity.y + (newVelocity.y * Random.Range(0.4f, 0.8f)), parentVelocity.z + velocityMultiplier);
        StartCoroutine("WaitToTurnOnColliders");
    }

    IEnumerator WaitToTurnOnColliders()
    {
        yield return new WaitForSeconds(GameManager.instance.invincibilityTime);

        collider.enabled = true;
    }

    IEnumerator WaitToDeactivateGO()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}