using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryButton : TooltipButton
{
    public ItemInventory itemInventory;
    public GameObject triggerOnClick;
    private Item item;

    [Header("Colors")]
    static public Color commonColor = Color.white;
    static public Color rareColor = Color.blue;
    static public Color uniqueColor = Color.magenta;
    static public Color legendaryColor = Color.yellow;

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
                    switch (item.rarity)
                    {
                        case Rarity.Common:
                            GetComponent<Image>().color = commonColor;
                            break;
                        case Rarity.Rare:
                            GetComponent<Image>().color = rareColor;
                            break;
                        case Rarity.Unique:
                            GetComponent<Image>().color = uniqueColor;
                            break;
                    case Rarity.Legendary:
                            GetComponent<Image>().color = legendaryColor;
                        break;
                        default:
                            break;
                    }
            }
            else {
                item = null;
                image.sprite = null;
                GetComponent<Image>().color = Color.white;
            }
            //todo:set sprite etc.
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        ShowTooltip(CarriedItem,itemInventory);
    }


    public void Click()
    {
        if (itemInventory != null)
        {
            itemInventory.ButtonClick(item);
        }
        if (triggerOnClick != null) {
            Debug.Log("Message sent");
            triggerOnClick.SendMessage("ButtonClicked",this,SendMessageOptions.RequireReceiver);
        }
    }

    public void OnScroll(UnityEngine.EventSystems.PointerEventData data) {
        itemInventory.GetComponent<ScrollRect>().OnScroll(data);
    }

    //todo: Tooltip
}
