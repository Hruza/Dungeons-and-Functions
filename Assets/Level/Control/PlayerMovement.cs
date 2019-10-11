using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Rychlost pohybu
    /// </summary>
    public float speed = 6f;

    public float knockbackResistance = 0.2f;

    static private Vector2 lookDir;
    /// <summary>
    /// Sklon kamery
    /// </summary>
    private float slope;
    private Rigidbody2D rbody;
    private Vector2 moveDir;

    public int playerMovementReduction;

    private bool knockbacked=false;

    void Start () {
        slope = Mathf.Cos(Mathf.Abs(Camera.main.transform.rotation.eulerAngles.x % 180));
        rbody = GetComponent<Rigidbody2D>();
    }

    static public Vector3 Forward() {
        Vector3 vect=new Vector3(lookDir.x,lookDir.y,0);
        return vect;
    }

    private void FixedUpdate() {
        if (!knockbacked) Move();
        Rotate();
    }
    void Update()
    {

	}

    public void Knockback(Vector2 direction) {
        knockbacked = true;
        StartCoroutine(Knocking(direction));
    }

    private IEnumerator Knocking(Vector2 direction) {
        Vector2 dampen = direction.normalized*knockbackResistance;
        while (dampen.x*direction.x>0) {
            rbody.velocity = direction;
            direction -= dampen*Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        knockbacked = false;
        yield return null;
    }

    /// <summary>
    /// Posune objekt ve směru dir ve 2d.
    /// </summary>
    void Move() {
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.y = Input.GetAxis("Vertical");

        //mapping square input into unit circle
        moveDir.y *= Mathf.Sqrt(1 - moveDir.x * moveDir.x * 0.5f);
        moveDir.x *= Mathf.Sqrt(1 - moveDir.y * moveDir.y * 0.5f);

        moveDir *= speed*(1-(playerMovementReduction*0.1f));
        rbody.velocity = moveDir;

    }

    /// <summary>
    /// Otoci objekt podle vzajemne pozice objektu a mysi
    /// </summary>
    void Rotate() {
        lookDir= Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        lookDir.y /= -slope;
        lookDir.Normalize();

        float rot = Mathf.Rad2Deg * Mathf.Atan2(lookDir.y, lookDir.x);
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
        
    }
    
}
