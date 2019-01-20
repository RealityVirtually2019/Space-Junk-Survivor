using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    public Transform iss;

    public void Click()
    {
        GameManager.instance.PressStart();

        iss.gameObject.SetActive(false);
    }

}
