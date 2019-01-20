using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    public GameObject deathText;
    private bool alive;

    private void Start()
    {
        alive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wieldable"))
        {
            if (other.gameObject.GetComponent<Interactable>().attachedHand == null)
            {
                if (alive)
                {
                    alive = false;
                    GameManager.instance.playerIsAlive = false;
                    StartCoroutine("DisplayText");
                }
            }
        }
    }

    IEnumerator DisplayText()
    {
        deathText.SetActive(true);

        yield return new WaitForSeconds(5f);

        deathText.SetActive(false);
    }

}