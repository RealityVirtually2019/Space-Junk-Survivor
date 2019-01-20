using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaSwordAnim : MonoBehaviour
{
    public float offset;
    public float scrollSpeed;
    Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed);
        rend.materials[1].SetTextureOffset("_MainTex", new Vector2(offset, offset));
        rend.materials[2].SetTextureOffset("_MainTex", new Vector2(-offset, -offset));
    }
}
