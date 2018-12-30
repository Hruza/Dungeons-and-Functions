using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public virtual int Level{
        get;
        set;
    }

    protected virtual void Update() {
        if (Input.GetButtonDown("Fire1")) Primary();
        if (Input.GetButtonDown("Fire2")) Secondary();
    }

    protected virtual void Primary() {

    }

    protected virtual void Secondary() {
        
    }

    public virtual bool ReadyToChange() {
        return true;
    }
}
