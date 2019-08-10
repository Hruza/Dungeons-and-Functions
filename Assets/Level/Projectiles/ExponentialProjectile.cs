using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExponentialProjectile : Projectile
{
    public int degree = 2;
    public int depth = 3;

    /// <summary>
    /// Rozptyl krajnich projektilu ve stupnich
    /// </summary>
    public float spread = 60;

    /// <summary>
    /// Kolikrat se zmeni zivostnost nasledujicich projektilu
    /// </summary>
    public float lifetimeMultiplier = 0.8f;
    // Start is called before the first frame update
    protected override void Start()
    {
        Invoke("Duplicate",lifetime);
    }

    // Update is called once per frame
    void Duplicate()
    {
        float speed = GetComponent<Rigidbody2D>().velocity.magnitude;
        Quaternion newDir;
        float angle;
        GameObject proj;
        float vel = GetComponent<Rigidbody2D>().velocity.magnitude;
        if (depth > 0)
        {
            for (int i = 0; i < degree; i++)
            {
                angle = (spread / 2) - (spread * (i / (degree - 1f)));
                newDir = transform.rotation * Quaternion.Euler(0, 0, angle);
                proj = (GameObject)Instantiate(this.gameObject,transform.position,newDir);
                Debug.Log(newDir * Vector3.forward);
                proj.GetComponent<Rigidbody2D>().velocity = newDir * Vector3.right*speed;
                proj.GetComponent<Projectile>().damage = damage;
                proj.GetComponent<ExponentialProjectile>().depth = depth - 1;
            }
        }
        End();
    }
}
