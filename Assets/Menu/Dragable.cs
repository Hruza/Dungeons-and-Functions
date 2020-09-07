using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler,IPointerDownHandler
{
    public InventorySlot button;
    public bool isStatic = true;
    private RectTransform tr;
    private void Awake() {
        tr = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if(!isStatic) {
            Debug.Log("works");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isStatic)
        tr.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isStatic)
        {
            Destroy(gameObject);
        }    
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isStatic)
        {
            GameObject draged = (GameObject)Instantiate(gameObject, transform.position, transform.rotation, transform.root);
            draged.GetComponent<Dragable>().isStatic = false;
        }
    }
}
