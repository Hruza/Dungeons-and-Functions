using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Rychlost pohybu
    /// </summary>
    public float speed = 0.1f;

    public float acceleration = 5f;

    private Vector2 lookDir;
    /// <summary>
    /// Sklon kamery
    /// </summary>
    private float slope;
    private Rigidbody2D rbody;
    private Vector2 moveDir;

    public GameObject ball;

    void Start () {
        slope = Mathf.Cos(Mathf.Abs(Camera.main.transform.rotation.eulerAngles.x % 180));
        rbody = GetComponent<Rigidbody2D>();
    }

    public Vector3 forward() {
        Vector3 vect=new Vector3(lookDir.x,lookDir.y,0);
        return vect;
    }

    private void FixedUpdate() {
        Move();
        Rotate();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            GameObject projectile = (GameObject)Instantiate(ball, transform.position + forward(), transform.rotation);
            projectile.GetComponent<Rigidbody2D>().velocity = forward() * 10;
            Destroy(projectile, 15);
        }
	}


    /// <summary>
    /// Posune objekt ve směru dir ve 2d.
    /// </summary>
    /// <param name="dir"></param>
    void Move() {
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.y = Input.GetAxis("Vertical");
        //moveDir.Normalize();
        moveDir *= speed;
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
