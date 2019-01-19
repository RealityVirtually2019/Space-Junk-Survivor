using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Behaviour_Pose m_Pose = null;
    public FixedJoint m_Joint = null;
    private Interactable m_CurrentInteractable = null;
    private List<Interactable> m_ContactInteractables = new List<Interactable>();

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    private void Update()
    {
       if(m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            print(gameObject.name + "trigger down");
            Pickup();
        }

        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            print(gameObject.name + "trigger down");
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Wieldable"))
        {
            return;
        }

        m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Wieldable"))
        {
            return;
        }

        m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
    }

    public void Pickup()
    {
        m_CurrentInteractable = GetNearestInteractable();

        if (!m_CurrentInteractable)
        {
            return;
        }

        if (m_CurrentInteractable.m_ActiveHand)
        {
            m_CurrentInteractable.m_ActiveHand.Drop();
        }

        //m_CurrentInteractable.transform.position = transform.position;

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop()
    {
        if (!m_CurrentInteractable)
        {
            return;
        }

        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();

        m_Joint.connectedBody = null;

        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;
    }

    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0f;

        foreach (Interactable interactable in m_ContactInteractables)
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

}