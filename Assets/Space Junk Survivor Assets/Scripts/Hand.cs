using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Input_Sources inputSource;
    public SteamVR_Action_Boolean grabActon = null;
    public SteamVR_Behaviour_Pose pose = null;
    public SteamVR_Action_Vibration vibration = null;
    public FixedJoint joint = null;
    private Interactable currentInteractable = null;
    private List<Interactable> possibleInteractables = new List<Interactable>();

    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
        if(grabActon.GetStateDown(pose.inputSource))
        {
            Pickup();
        }

        if (grabActon.GetStateUp(pose.inputSource))
        {
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Wieldable") && !other.CompareTag("Weapon"))
        {
            return;
        }

        possibleInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Wieldable") && !other.CompareTag("Weapon"))
        {
            return;
        }

        possibleInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void Pickup()
    {
        currentInteractable = GetNearestInteractable();

        if (!currentInteractable)
        {
            return;
        }

        if (currentInteractable.attachedHand)
        {
            currentInteractable.attachedHand.Drop();
        }

        //m_CurrentInteractable.transform.position = transform.position;

        Rigidbody targetBody = currentInteractable.GetComponent<Rigidbody>();
        joint.connectedBody = targetBody;

        currentInteractable.attachedHand = this;
        print("Picked up " + currentInteractable.gameObject.name);
    }

    public void Drop()
    {
        if (!currentInteractable)
        {
            return;
        }

        Rigidbody targetBody = currentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = pose.GetVelocity();
        targetBody.angularVelocity = pose.GetAngularVelocity();

        joint.connectedBody = null;

        currentInteractable.attachedHand = null;
        currentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0f;

        foreach (Interactable interactable in possibleInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }

    public void TriggerVibration()
    {
        vibration.Execute(0f, 0.25f, 40f, 1f, inputSource);
    }

}