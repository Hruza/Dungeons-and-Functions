using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragedItem : MonoBehaviour
{
    static public Item dragedItem;
    private Item item;
    public Item CarriedItem
    {
        get
        {
            return item;
        }
        set
        {
            if (value != null)
            {
                item = value;
                GetComponent<Image>().sprite = item.sprite;
            }
            else
            {
                item = null;
                GetComponent<Image>().sprite = null;
            }
            //todo:set sprite etc.
        }

    }

    Vector3 relativePos;

    private void Start()
    {
        dragedItem = CarriedItem;
        relativePos= transform.position - Input.mousePosition;
    }

    private void Update()
    {
        transform.position = Input.mousePosition+relativePos;
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            Destroy(this.gameObject);
            dragedItem = null;
        }
    }
}