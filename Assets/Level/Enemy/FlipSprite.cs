using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipSprite : MonoBehaviour {
    public SpriteRenderer sprite;
    public bool defaultLeft = false;
    private GameObject player;
    private bool isFlipped=false;
	// Use this for initialization
	void Start () {
        player = Player.player;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 dif = player.transform.position - transform.position;
        if (dif.x < 0)
        {
            if (!isFlipped)
            {
                sprite.flipX = true;
                isFlipped = !defaultLeft;
            }
        }
        else if (isFlipped)
        {
            sprite.flipX = false;
            isFlipped = defaultLeft;
        }
    }
}
