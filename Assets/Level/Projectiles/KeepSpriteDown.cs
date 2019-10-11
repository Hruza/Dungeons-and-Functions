using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepSpriteDown : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float flipMin = 90;
    public float flipMax = 270;
    
    // Update is called once per frame
    void Start()
    {
        if (transform.rotation.eulerAngles.z % 360 > flipMin && transform.rotation.eulerAngles.z % 360 < flipMax) sprite.flipY = true;
    }
}
