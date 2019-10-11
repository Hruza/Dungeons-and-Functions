using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public ItemInventory itemInventory;
    private Item item;

    [Header("Colors")]
    static public Color commonColor = Color.white;
    static public Color rareColor = Color.blue;
    static public Color uniqueColor = Color.magenta;
    static public Color legendaryColor = Color.yellow;

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

    public void Click() {
        itemInventory.ButtonClick(item);
    }

    private GameObject tooltip;

    public void ShowTooltip() {
        if (item != null && tooltip == null)
        {
            if(itemInventory!=null) tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, itemInventory.transform);
            else tooltip = (GameObject)Instantiate(tooltipPrefab, Input.mousePosition, tooltipPrefab.transform.rotation, transform.parent.parent);
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
