using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration=0.5f;
    public float speed=5;
    public float magnitudeModifier = 1;
    public AnimationCurve timeMagnitude;

    private Vector3 initPos;

    private void Start()
    {
        initPos=transform.localPosition;
    }


    Coroutine shake;

    public void Shake(float magnitude=1)
    {
        if (shake != null) {
            StopCoroutine(shake);
        }
        shake=StartCoroutine(Shaking(magnitude));
    }

    IEnumerator Shaking(float magnitude)
    {
        float seed = Random.Range(0, 500f);
        float time = 0;
        while (time<duration)
        {
            transform.localPosition = initPos + (magnitude*magnitudeModifier*timeMagnitude.Evaluate(time/duration) * Perlin(seed,time*speed));
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initPos;
    }

    Vector3 Perlin(float seed,float t) {
        Mathf.PerlinNoise(seed + t, 0);
        Mathf.PerlinNoise(seed + t, 100);
        Mathf.PerlinNoise(seed + t, 200);
        return new Vector3( 2 * Mathf.PerlinNoise(seed + t,   0) - 1,
                            2 * Mathf.PerlinNoise(seed + t, 100) - 1,
                            2 * Mathf.PerlinNoise(seed + t, 200) - 1
                           );
    }
}
