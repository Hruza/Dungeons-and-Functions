using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public GameObject door;
    public void OnEnter()
    {
        t = 0;
        door.SetActive(true);
        StartCoroutine(MoveDoor(2f,-1.5f,0.5f));
    }

    public void OnClear()
    {
        t = 0;
        StartCoroutine(MoveDoor(-1.5f, 2f, 0.5f, false));
    }
    private float t = 0;

    IEnumerator MoveDoor(float from, float to, float time,bool active=true) {
        
        while (t<time)
        {
            door.transform.localPosition= new Vector3(0, 0, from + ((t/time)*(to-from)));
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        door.transform.position.Set(0, 0, to);
        door.SetActive(active);
        yield return null;
    }
}
