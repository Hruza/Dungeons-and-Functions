﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : TooltipButton
{
    public ItemInventory itemInventory;
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

    public void MouseEnter()
    {
        ShowTooltip(CarriedItem,itemInventory);
    }


    public void Click()
    {
    itemInventory.ButtonClick(item);
    }



    //todo: Tooltip
}
