using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public GameObject door;
    public Transform closedPoint;
    public Transform openPoint;
    public Collider2D collider;

    public float closingTime=0.5f;

    public enum OpenType {fromGround,fallDown,sliding }
    public enum CloseType { toGround, riseUp, sliding,disappear }
    public OpenType opening;
    public CloseType closing;



    public ParticleSystem closedParticles;
    public ParticleSystem openParticles;

    public void OnEnter()
    {
        door.SetActive(true);
        if(collider!=null)
            collider.enabled = true;
        switch (opening)
        {
            case OpenType.fromGround:
                LeanTween.moveLocalZ(door, closedPoint.position.z, closingTime).setEaseSpring().setOnComplete(OnClosed);
                break;
            case OpenType.fallDown:
                LeanTween.moveLocalZ(door, closedPoint.position.z, closingTime).setEaseOutQuad().setOnComplete(OnClosed);
                break;
            case OpenType.sliding:
                LeanTween.move(door, closedPoint.position, closingTime).setEaseOutCubic().setOnComplete(OnClosed);
                break;
            default:
                break;
        }
    }

    public void OnClosed() {
        if(closedParticles!=null)
            closedParticles.Play();
    }

    public void OnOpen() {
        if (openParticles != null)
            openParticles.Play();
    }

    public void OnClear()
    {
        if (collider != null)
            collider.enabled = false;
        switch (closing)
        {
            case CloseType.toGround:
                LeanTween.moveLocalZ(door, openPoint.position.z, closingTime).setEaseOutCubic().setOnComplete(OnOpen);
                break;
            case CloseType.riseUp:
                LeanTween.moveLocalZ(door, openPoint.position.z, closingTime).setEaseOutCubic().setOnComplete(OnOpen);
                break;
            case CloseType.sliding:
                LeanTween.move(door, openPoint.position, closingTime).setEaseOutCubic().setOnComplete(OnOpen);
                break;
            case CloseType.disappear:
                door.SetActive(false);
                OnOpen();
                break;
            default:
                break;
        }
    }
}
