using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int minDamage;
    public int maxDamage;
    public int attackSpeed;
    public WeaponController controller;

    protected virtual void Update()
    {
        if (Input.GetButtonDown("Fire1")) Primary();
        if (Input.GetButtonDown("Fire2")) Secondary();
    }

    protected virtual void Primary()
    {
        controller.Cooldown(10f/attackSpeed);
    }

    protected virtual void Secondary()
    {
        
    }

    public virtual bool ReadyToChange()
    {
        return true;
    }
}
