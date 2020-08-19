using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MeleeWeapon : Weapon
{
    public float angleOfSwing=120;
    public GameObject swingingThing;
    public float delay = 0f;
    public float knockback=1f;
    public bool autoAttack = false;
    private bool ready = true;
    private int lastSwingDir=1;
    public bool changeDirections = true;

    List<GameObject> attacked;

    protected override void Update()
    {
        if (ready)
        {
            if (autoAttack && Input.GetButton("Fire1")) Primary();
            else if (!autoAttack && Input.GetButtonDown("Fire1")) Primary();
        }
        if (Input.GetButtonDown("Fire2")) Secondary();
    }

    private void Reset()
    {
        ready = true;
    }

    protected override void Primary()
    {
        base.Primary();
        ready = false;
        attacked = new List<GameObject>();
        StartCoroutine(WeaponSwing());
    }

    protected IEnumerator WeaponSwing() {
        swingingThing.SetActive(true);
        swingingThing.transform.localRotation = Quaternion.Euler(0,0,-lastSwingDir*(angleOfSwing/2));
        float angle = 0;
        while (angle<angleOfSwing)
        {
            swingingThing.transform.Rotate(0,0,lastSwingDir*Time.deltaTime*angleOfSwing*attackSpeed/10);
            angle += Time.deltaTime*angleOfSwing*attackSpeed/10;
            yield return new WaitForEndOfFrame();
        }
        if(changeDirections) lastSwingDir *= -1;
        swingingThing.SetActive(false);
        Invoke("Reset", delay);
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Destroyable") && !attacked.Contains(collision.gameObject)) {
            int damage = Random.Range(minDamage, maxDamage + 1);
            Damager.InflictDamage(collision.gameObject, damage, (collision.transform.position - transform.position).normalized, damageType);
            attacked.Add(collision.gameObject);
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if(rb!=null) rb.AddForce((collision.transform.position-transform.position).normalized * knockback,ForceMode2D.Impulse);
        }
    }
}
