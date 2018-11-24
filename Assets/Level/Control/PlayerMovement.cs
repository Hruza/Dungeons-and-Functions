using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    /// <summary>
    /// Rychlost pohybu
    /// </summary>
    public float speed = 0.1f;
   
    /// <summary>
    /// Sklon kamery
    /// </summary>
    private float slope; 
	// Use this for initialization
	void Start () {
       slope = Mathf.Cos(Mathf.Abs(Camera.main.transform.rotation.eulerAngles.x % 180));
        Debug.Log(Mathf.Deg2Rad*(Camera.main.transform.rotation.eulerAngles.x % 180));
    }
	
	// Update is called once per frame
	void Update () {
        Move();
        Rotate();
	}

    private Vector2 moveInput() {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector2.up;
        }

        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector2.down;
        }

        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector2.right;
        }

        dir.Normalize();

        return dir;
    }

    /// <summary>
    /// Posune objekt ve směru dir ve 2d.
    /// </summary>
    /// <param name="dir"></param>
    void Move() {
        Vector2 dir = moveInput()*speed;
        Vector3 vect=new Vector3(dir.x, dir.y, 0);
    
        transform.position += vect;

    }

    /// <summary>
    /// Otoci objekt podle vzajemne pozice objektu a mysi
    /// </summary>
    void Rotate() {
        Vector3 screen= Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        screen.y /=-slope;
        screen.z = 0;
        Debug.Log(screen);
        this.transform.rotation = Quaternion.LookRotation(screen);

    }
    
}
