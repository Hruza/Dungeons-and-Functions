using UnityEngine;

public class InfinityExplosion : MonoBehaviour
{
    public float duration = 1f;
    public float angle = 360;
    public AnimationCurve size;
    public float scale;
    void Start()
    {
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward ,angle, duration);
        LeanTween.scale(gameObject,Vector3.one*scale,duration).setEase(size);
    }

}
