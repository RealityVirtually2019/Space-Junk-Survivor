using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    public Hand m_ActiveHand = null;

    private void OnTriggerEnter(Collider other)
    {
        if (m_ActiveHand && other.gameObject.CompareTag("Wieldable"))
        {
            Destroy(other.gameObject);
        }
    }
}