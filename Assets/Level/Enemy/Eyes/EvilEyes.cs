using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilEyes : MonoBehaviour
{
    public GameObject target;
    public SpriteMask mask;
    public GameObject rEye;
    public GameObject lEye;
    private Vector3 rCenter;
    private Vector3 lCenter;
    public float radius = 0.1f;
    public Sprite angry;
    public Sprite angryM;
    public Sprite closed;
    public Sprite closedM;
    public Sprite sad;
    public Sprite sadM;
    public Sprite sadClosed;
    public Sprite sadClosedM;
    public Sprite dead;
    public Sprite deadM;
    public enum EyeType { angry, sad, closed, closedSad, dead }
    public EyeType eyeType{
        set {
            switch (value)
            {
                case EyeType.angry:
                    mask.sprite = angryM;
                    GetComponent<SpriteRenderer>().sprite = angry;
                    rEye.SetActive(true);
                    lEye.SetActive(true);
                    break;
                case EyeType.sad:
                    mask.sprite = sadM;
                    GetComponent<SpriteRenderer>().sprite = sad;
                    rEye.SetActive(true);
                    lEye.SetActive(true);
                    break;
                case EyeType.closed:
                    mask.sprite = closedM;
                    GetComponent<SpriteRenderer>().sprite = closed;
                    rEye.SetActive(false);
                    lEye.SetActive(false);
                    break;
                case EyeType.closedSad:
                    mask.sprite = sadClosedM;
                    GetComponent<SpriteRenderer>().sprite = sadClosed;
                    rEye.SetActive(false);
                    lEye.SetActive(false);
                    break;
                case EyeType.dead:
                    mask.sprite = deadM;
                    GetComponent<SpriteRenderer>().sprite = dead;
                    rEye.SetActive(false);
                    lEye.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    public EyeType type;
    private EyeType lasttype;

    // Start is called before the first frame update
    void Start()
    {
        rCenter = rEye.transform.localPosition;
        lCenter = lEye.transform.localPosition;
        target = Player.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (type != lasttype) {
            eyeType = type;
            lasttype = type;
        }
        if (target != null) {
            LookDirection(rCenter, rEye, target.transform.position - (transform.localToWorldMatrix.MultiplyPoint( rCenter)));
            LookDirection(lCenter, lEye, target.transform.position - (transform.localToWorldMatrix.MultiplyPoint(lCenter)));
        }
    }

    void LookDirection(Vector2 center, GameObject eye,Vector2 dir) {
        eye.transform.localPosition = center + (radius * dir.normalized);
    }
}
