using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message : MonoBehaviour
{
    public TextMesh text;
    public Color TextColor {
        set {
            text.color = value;
        }
    }
    public float Size {
        set {
            if(value>0)
                text.characterSize = value/10;
        }
    }
    public string MessageText {
        set
        {
            text.text = value;
        }
    }
    private void Start()
    {
        Destroy(this.gameObject,1);
    }
}
