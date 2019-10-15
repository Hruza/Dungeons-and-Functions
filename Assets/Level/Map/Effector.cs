using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effector : MonoBehaviour
{
    public int intensity=100;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if(rb!=null)rb.AddForce(-transform.up*intensity*rb.mass,ForceMode2D.Force);
    }
}
