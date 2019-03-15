using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Weapon
{
    public float velocity = 10f;
    public GameObject projectile;
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

    private void Reset()
    {
        ready = true;
    }

    protected override void Primary()
    {
        ready = false;
        Invoke("Reset", 1f/attackSpeed);
        Vector3 forward = PlayerMovement.Forward();
        GameObject ball  = (GameObject)Instantiate(projectile, transform.position +forward, transform.rotation);
        ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
        ball.GetComponent<Projectile>().damage = Random.Range(minDamage, maxDamage + 1);
    }
}
