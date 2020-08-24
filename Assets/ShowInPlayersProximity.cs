using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInPlayersProximity : MonoBehaviour
{

    public void Start()
    {
        Color color = GetComponent<MeshRenderer>().material.color;
        color.a = Mathf.Clamp(distanceToAlpha.Evaluate((transform.position - Player.player.transform.position).magnitude / 4), 0, 1);
        GetComponent<MeshRenderer>().material.color = color;
    }
    public AnimationCurve distanceToAlpha=AnimationCurve.Linear(0,1,1,0);
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) { 
            Color color = GetComponent<MeshRenderer>().material.color;
            color.a = Mathf.Clamp(distanceToAlpha.Evaluate((transform.position - collision.transform.position).magnitude/4),0,1);
            GetComponent<MeshRenderer>().material.color = color;
        }
    }
}
