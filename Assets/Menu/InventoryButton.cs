using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public ItemInventory itemInventory;
    private Item item;
    public Image image;
    public Item CarriedItem{
        get {
            return item;
        }
        set {
            item = value;
            image.sprite = item.sprite;
            //todo:set sprite etc.
        }
    }

    public void Click() {
        itemInventory.ButtonClick(item);
    }
    
    //todo: Tooltip
}
