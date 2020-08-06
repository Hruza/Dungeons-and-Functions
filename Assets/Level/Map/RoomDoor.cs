using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public GameObject door;
    public Transform closedPoint;
    public Transform openPoint;
    public Collider2D doorCollider;

    public float closingTime=0.5f;

    public enum OpenType {fromGround,fallDown,sliding ,none }
    public enum CloseType { toGround, riseUp, sliding,disappear ,none}
    public OpenType opening;
    public CloseType closing;

    public GameObject doorPart;
    public GameObject replacement;

    private bool isDoor = true;

    [ExecuteInEditMode]
    public bool IsDoor {
        set {
            doorPart.SetActive(value);
            replacement.SetActive(!value);
            isDoor = value;
        }
        get {
            return isDoor;
        }
    }

    public ParticleSystem closedParticles;
    public ParticleSystem openParticles;

    public void OnEnter()
    {
        if(door!=null) door.SetActive(true);
        if(doorCollider!=null)
            doorCollider.enabled = true;
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
        if (doorCollider != null)
            doorCollider.enabled = false;
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
