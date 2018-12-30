using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Weapon {

    public float velocity = 10f;
    public GameObject projectile;
    private int damage;
    public int baseDamage;
    public int damageIncresePerLevel;
    public float delay=0.5f;
    public bool autoFire=false;
    private bool ready = true;

    protected override void Update()
    {
        if (ready)
        {
            if (autoFire && Input.GetButton("Fire1")) Primary();
            else if (!autoFire && Input.GetButtonDown("Fire1")) Primary();
        }
        if (Input.GetButtonDown("Fire2")) Secondary();
    }

    public override int Level{
        get {
            Debug.LogError("Tady level zbrane nezjistis! Koukni se do WeaponProperties");
            return 0;
        }
        set {
            damage = baseDamage + (damageIncresePerLevel*value);
            
        }
    }

    private void Reset()
    {
        ready = true;
    }

    protected override void Primary()
    {
        ready = false;
        Invoke("Reset", delay);
        Vector3 forward = PlayerMovement.forward();
        GameObject ball  = (GameObject)Instantiate(projectile, transform.position +forward, transform.rotation);
        ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
        ball.GetComponent<Projectile>().damage = damage;
    }
}
