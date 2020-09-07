using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseProjectile : Weapon
{
    public Vector3 velocity = Vector3.zero;
    public GameObject projectile;
    public GameObject onShootParticles;
    public bool autoFire=false;
    private bool ready = true;
    public float spread = 0f;
    public int projectilesPerShot = 1;

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

    public override bool ReadyToChange()
    {
        return ready;
    }

    protected override void Primary()
    {
        base.Primary();
        ready = false;
        Invoke("Reset", 10f / attackSpeed);
        Vector3 mouse = PlayerMovement.MouseWorldPos();
        Quaternion spreadRotation = Quaternion.identity;
        for (int i = 0; i < projectilesPerShot; i++)
        {
            Vector3 spreadVector = Vector3.zero;
            if (spread > 0) {
                spreadVector = spread*Random.insideUnitCircle;
            }
            GameObject ball = (GameObject)Instantiate(projectile, mouse+spreadVector, Quaternion.identity);
            ball.GetComponent<Rigidbody2D>().velocity = transform.rotation*velocity;
            if (ball.GetComponent<FieldProjectile>() != null) {
                ball.GetComponent<FieldProjectile>().damage = minDamage;
                ball.GetComponent<FieldProjectile>().maxDamage = maxDamage;
            }
            else
                ball.GetComponent<Projectile>().damage = Random.Range(minDamage, maxDamage + 1);
            ball.GetComponent<Projectile>().damageType = damageType;
        }
        if (onShootParticles != null)
        {
            GameObject particles = (GameObject)Instantiate(onShootParticles, mouse, transform.rotation);
            Destroy(particles, 3);
        }
    }
}
