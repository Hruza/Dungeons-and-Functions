using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Weapon
{
    public float velocity = 10f;
    public GameObject projectile;
    public bool autoFire=false;
    private bool ready = true;
    public float spread = 0f;
    public int projectilesPerShot = 1;
    public bool uniformSpread = false;

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
        Invoke("Reset", 10f / attackSpeed);
        Vector3 forward = PlayerMovement.Forward();
        Quaternion spreadRotation = Quaternion.identity;
        for (int i = 0; i < projectilesPerShot; i++)
        {
            GameObject ball = (GameObject)Instantiate(projectile, transform.position + forward, transform.rotation);
            if (spread > 0) {
                if (projectilesPerShot > 1 && uniformSpread) {
                    spreadRotation = Quaternion.Euler(0, 0, -spread+(((2f*i)/(projectilesPerShot-1))*spread));
                }
                else
                    spreadRotation = Quaternion.Euler(0,0,Random.Range(-spread,spread));
            }
            ball.GetComponent<Rigidbody2D>().velocity = spreadRotation*forward * velocity;
            ball.GetComponent<Projectile>().damage = Random.Range(minDamage, maxDamage + 1);
        }
    }
}
