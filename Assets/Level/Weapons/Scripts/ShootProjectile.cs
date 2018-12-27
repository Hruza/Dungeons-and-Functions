using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : Weapon {
    
    public float velocity = 10f;
    public GameObject projectile;
   

    protected override void Primary()
    {
        Vector3 forward = PlayerMovement.forward();
        GameObject ball  = (GameObject)Instantiate(projectile, transform.position +forward, transform.rotation);
        ball.GetComponent<Rigidbody2D>().velocity = forward * velocity;
    }
}
