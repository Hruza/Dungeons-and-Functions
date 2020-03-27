using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    public TextMeshPro text;
    public Color TextColor {
        set {
            text.color = value;
        }
    }
    public Color OutlineColor
    {
        set
        {
            text.fontMaterial.SetFloat("_OutlineWidth", 0.25f);
            text.fontMaterial.SetColor("_OutlineColor",value);
        }
    }
    public float Size {
        set {
            if(value>0)
                text.fontSize = value*10;
        }
    }
    public string MessageText {
        set
        {
            text.text=value;
        }
    }
    private void Start()
    {
        Destroy(this.gameObject,1);
    }
}
