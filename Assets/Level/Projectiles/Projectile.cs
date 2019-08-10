using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    public int damage = 1;
    public float lifetime = 10;
    public bool damageEnemies = true;
    public bool damagePlayer = false;
    public bool damageDestroyables = true;
    public bool destroyOnDamageDealt = true;
    public bool destroyOnCollision = false;
    public GameObject onDestroyParticles;

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
            if (destroyOnDamageDealt) End();
        }
        else if (tag == "Destroyable" && damageDestroyables)
        {
            collision.gameObject.SendMessage("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
        }
        if (destroyOnCollision) End();
    }

    protected void End()
    {
        if (onDestroyParticles != null)
        {
            GameObject particles=(GameObject)Instantiate(onDestroyParticles, transform.position, transform.rotation);
            Destroy(particles, 3);
        }
        Destroy(this.gameObject);
    }
}
