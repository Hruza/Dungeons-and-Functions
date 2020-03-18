using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour {
    public int damage = 1;
    public float lifetime = 10;
    public enum ProjectileType { classic,sin,cos,homing }
    public ProjectileType projectileType;

    [HideInInspector]
    public Damager.DamageType damageType;

    [Header("Collisions")]
    public bool damageEnemies = true;
    public bool damagePlayer = false;
    public bool damageDestroyables = true;
    public bool destroyOnDamageDealt = true;
    public bool destroyOnAnyCollision = false;
    public bool destroyOnWorldCollision = true;
    public GameObject onDamageParticles;
    public float knockback = 0f;
    public float perpKnockback=0f;

    [Header("On Destroy")]
    public GameObject onDestroyParticles;

    public GameObject destroyFirst;
    public float destroyTime;

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

    [HideInInspector]
    public float detectionDistance = 6f;
    [HideInInspector]
    public float detectionAngle = 60f;
    [HideInInspector]
    public float turningSpeed = 1f;

    private Rigidbody2D rb;
    private float t;
    private Vector2 v0;
    private Vector2 perp;
    virtual protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.rotation = Quaternion.FromToRotation(Vector3.right, rb.velocity);
        if (projectileType != ProjectileType.classic)
        {
            t = 0;
            v0 = rb.velocity;
            perp = new Vector2(-v0.y, v0.x);
        }
        Invoke("End", lifetime);
        if (projectileType == ProjectileType.homing) InvokeRepeating("SearchForTarget",0,0.25f);
    }

    private void SearchForTarget() {
        Collider2D[] tgts=Physics2D.OverlapCircleAll(transform.position, detectionDistance,LayerMask.GetMask("Player","Enemy","Yuki"));
        GameObject candidate = null;
        float min=0;
        foreach (Collider2D tgt in tgts) {
            if (((tgt.tag == "Enemy" && damageEnemies) || ( tgt.tag == "Player" && damagePlayer)) && Vector2.Angle(tgt.transform.position - transform.position, rb.velocity) < detectionAngle)
            {
                if (candidate == null) {
                    candidate = tgt.gameObject;
                    min = Vector2.SqrMagnitude(tgt.transform.position-transform.position);
                }
                else if (min > Vector2.SqrMagnitude(tgt.transform.position - transform.position))
                {
                    min = Vector2.SqrMagnitude(tgt.transform.position - transform.position);
                    candidate = tgt.gameObject;
                }
            }

        }
        Debug.Log(candidate);
        if (candidate != null) target=candidate;
    }

    private GameObject target;

    private void FixedUpdate()
    {
        switch (projectileType)
        {
            case ProjectileType.classic:
                break;
            case ProjectileType.sin:
                t += Time.deltaTime;
                rb.velocity = v0 + perp * (3*t * Mathf.Cos(t*15));
                break;
            case ProjectileType.cos:
                break;
            case ProjectileType.homing:
                if (target != null)
                {
                    float angle = Vector2.SignedAngle(rb.velocity, target.transform.position - transform.position);
                    Quaternion rot= Quaternion.Euler(0, 0, turningSpeed*Time.fixedDeltaTime*Mathf.Sign(angle));
                    rb.velocity = rot * rb.velocity;
                    transform.rotation = Quaternion.FromToRotation(Vector3.right,rb.velocity);

                }
                break;
            default:
                break;
        }
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
        if ((tag == "Enemy" && damageEnemies) || ((tag=="Shield" || tag == "Player") && damagePlayer) || (tag == "Destroyable" && damageDestroyables))
        {
            Damager.InflictDamage(collision.gameObject, damage, damageType);
            if (knockback + perpKnockback != 0)
            {
                Vector2 dif=(Vector2)(collision.gameObject.transform.position - transform.position);
                Vector2 perpVelocity = Vector2.Perpendicular(rb.velocity.normalized);
                float coefficient = Vector2.Dot(dif, perpVelocity);

                collision.gameObject.SendMessage("Knockback", knockback *dif.normalized+(perpKnockback*Mathf.Sign(coefficient)*perpVelocity) , SendMessageOptions.DontRequireReceiver);
            }
                if (onDamageParticles != null) {
                GameObject particles = (GameObject)Instantiate(onDamageParticles, transform.position, transform.rotation);
                Destroy(particles, 3);
            }
            if (destroyOnDamageDealt) End();
        }
        else if (tag == "Destroyable" && damageDestroyables)
        {
            Damager.InflictDamage(collision.gameObject, damage, damageType);
        }
        if (destroyOnAnyCollision) End();
        if (destroyOnWorldCollision && (collision.gameObject.layer == LayerMask.NameToLayer("Map") || tag == "Map" || tag=="Destroyable")) End() ;
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
        if (destroyFirst != null) Destroy(destroyFirst) ;
        Destroy(this.gameObject,destroyTime);
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Destroy(this.GetComponent<Projectile>());
    }

    public void Explode()
    {
        foreach (Collider2D coll in Physics2D.OverlapCircleAll(transform.position, explosionRadius))
        {
            string tag = coll.gameObject.tag;
            if ((tag == "Enemy" && explosionDamageEnemies) || (tag == "Player" && explosionDamagePlayer) || (tag == "Destroyable" && explosionDamageDestroyables))
            {
                Vector3 dir = coll.gameObject.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, dir, dir.magnitude, LayerMask.GetMask("Map", "Shield"))) {
                    Damager.InflictDamage(coll.gameObject, explosionDamageMultiplicator * damage * explosionDamageDistribution.Evaluate(Mathf.Clamp((coll.gameObject.transform.position - transform.position).magnitude / explosionRadius, 0, 1)), damageType);
                }
            }
        }
        
    }
}

