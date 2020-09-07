using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effector : MonoBehaviour
{
    public int intensity = 100;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
            if (rb.CompareTag("Player"))
            {
                rb.GetComponent<PlayerMovement>().AddForce(-transform.up * intensity*UnitDecomposition(collision.transform.position));
            }
            else
            {
                rb.AddForce(-transform.up * intensity * rb.mass, ForceMode2D.Force);
            }
    }

    private float UnitDecomposition(Vector2 pos)
    {
        float value = Mathf.Min(2.5f-Mathf.Abs(pos.x-transform.position.x),2.5f-Mathf.Abs(pos.y-transform.position.y));
        return Mathf.Clamp(value,0,1);
    }
}
