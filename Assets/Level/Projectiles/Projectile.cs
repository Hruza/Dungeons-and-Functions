using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    public int damage = 1;
    public float lifetime = 10;
    [Header("Collisions")]
    public bool damageEnemies = true;
    public bool damagePlayer = false;
    public bool damageDestroyables = true;
    public bool destroyOnDamageDealt = true;
    public bool destroyOnAnyCollision = false;
    public bool destroyOnWorldCollision = true;
    public GameObject onDamageParticles;

    [Header("On Destroy")]
    public GameObject onDestroyParticles;


    public bool explosion=false;
    [HideInInspector]
    public float explosionDamageMultiplicator = 1;
    [HideInInspector]
    public float explosionRadius = 2;
    [HideInInspector]
    public AnimationCurve explosionDamageDistribution=AnimationCurve.Linear(0,1,1,0);
    [HideInInspector]
    public bool explosionDamageEnemies = true;
    [HideInInspector]
    public bool explosionDamagePlayer = false;
    [HideInInspector]
    public bool explosionDamageDestroyables = true;

    virtual protected void Start()
    {
        Invoke("End", lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collided(collision.collider);   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collided(collision);
    }

    private void Collided(Collider2D collision) {
        string tag = collision.gameObject.tag;
        if ((tag == "Enemy" && damageEnemies) || (tag == "Player" && damagePlayer) || (tag == "Destroyable" && damageDestroyables))
        {
            collision.gameObject.SendMessage("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
            if (onDamageParticles != null) {
                GameObject particles = (GameObject)Instantiate(onDamageParticles, transform.position, transform.rotation);
                Destroy(particles, 3);
            }
            if (destroyOnDamageDealt) End();
        }
        else if (tag == "Destroyable" && damageDestroyables)
        {
            collision.gameObject.SendMessage("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        if (destroyOnAnyCollision) End();
        Debug.Log(LayerMask.GetMask("Map"));
        if (destroyOnWorldCollision && collision.gameObject.layer == LayerMask.NameToLayer("Map")) End() ;
    }

    protected void End()
    {
        if (explosion)
        {
            Explode();
        }

        if (onDestroyParticles != null)
        {
            GameObject particles=(GameObject)Instantiate(onDestroyParticles, transform.position, transform.rotation);
            Destroy(particles, 3);
        }
        Destroy(this.gameObject);
    }

    public void Explode()
    {
        foreach (Collider2D coll in Physics2D.OverlapCircleAll(transform.position, explosionRadius))
        {
            string tag = coll.gameObject.tag;
            if ((tag == "Enemy" && explosionDamageEnemies) || (tag == "Player" && explosionDamagePlayer) || (tag == "Destroyable" && explosionDamageDestroyables))
            {
                coll.gameObject.SendMessage("GetDamage", explosionDamageMultiplicator*damage*explosionDamageDistribution.Evaluate(Mathf.Clamp((coll.gameObject.transform.position-transform.position).magnitude/explosionRadius,0,1)), SendMessageOptions.DontRequireReceiver);
            }
        }
        
    }
}
