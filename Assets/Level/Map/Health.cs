using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    public int health=100;

    public float noDamageTime = 0f;
    private bool ready = true;
    private Animator anim;

    Color c;
    // Use this for initialization
    private void Start()
    {
        anim=GetComponent<Animator>();
    }

    public void GetDamage(int damage) {
        if (ready)
        {
            ready = false;
            if ((health -= damage) <= 0) Destroy(gameObject);
            Invoke("ResetDamage", noDamageTime);
            if (anim != null) anim.SetTrigger("getDamage");
        }
    }

    private void ResetDamage() {
        ready = true;
    }

}
