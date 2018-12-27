using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    public int damage=1;
    public float lifetime = 10;

    private void Start()
    {
        Destroy(this.gameObject,lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.SendMessage("GetDamage",damage,SendMessageOptions.DontRequireReceiver);
    }
}
