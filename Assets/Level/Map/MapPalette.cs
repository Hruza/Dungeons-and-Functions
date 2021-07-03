using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPalette : MonoBehaviour
{
    [SerializeField]
    public List<Palette> palletes;
}

[System.Serializable]
public class Palette{
    public Color wall;
    public Color corridor;
    public Color floor;
}