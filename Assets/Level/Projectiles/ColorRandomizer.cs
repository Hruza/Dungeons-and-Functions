using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour
{
    public Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        Color col = colors[Random.Range(0, colors.Length)];
        GetComponentInChildren<MeshRenderer>().material.color = col;
        GetComponentInChildren<ParticleSystem>().startColor = col;
    }
}
