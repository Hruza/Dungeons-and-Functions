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
    public bool destroyOnCollision = true;
    private void Start()
    {
        Destroy(this.gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if ((tag == "Enemy" && damageEnemies) || (tag == "Player" && damagePlayer) || (tag == "Destroyable" && damageDestroyables))
        {
            collision.collider.SendMessage("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
            if (destroyOnCollision) Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if ((tag == "Enemy" && damageEnemies) || (tag == "Player" && damagePlayer) || (tag == "Destroyable" && damageDestroyables))
        {
            collision.gameObject.SendMessage("GetDamage", damage, SendMessageOptions.DontRequireReceiver);
            if (destroyOnCollision) Destroy(gameObject);
        }
    }
}
