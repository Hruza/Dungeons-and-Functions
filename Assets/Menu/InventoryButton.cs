using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public ItemInventory itemInventory;
    private Item item;

    public GameObject tooltipPrefab;

    public Image image;
    public Item CarriedItem{
        get {
            return item;
        }
        set {
            if (value != null)
            {
                item = value;
                image.sprite = item.sprite;
            }
            else {
                item = null;
                image.sprite = null;
            }
            //todo:set sprite etc.
        }
    }

    public void Click() {
        itemInventory.ButtonClick(item);
    }

    private GameObject tooltip;

    public void ShowTooltip() {
        if (item != null && tooltip == null)
        {
            tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, itemInventory.transform);
            tooltip.GetComponent<InvTooltip>().Item = item;
            StartCoroutine(MoveTooltip());
        }
    }

    public IEnumerator MoveTooltip() {
        while (tooltip != null)
        {
            tooltip.transform.position = Input.mousePosition;
            yield return new WaitForEndOfFrame();
        }
        
    }

    public void DeleteTooltip() {
        if(tooltip!=null)
            Destroy(tooltip);
    }

    //todo: Tooltip
}
