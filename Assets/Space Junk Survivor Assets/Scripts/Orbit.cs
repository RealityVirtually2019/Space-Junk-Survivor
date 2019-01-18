using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Vector3 orbitCenter;
    public Vector3 orbitVector;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, orbitVector, speed * Time.deltaTime);
    }

}