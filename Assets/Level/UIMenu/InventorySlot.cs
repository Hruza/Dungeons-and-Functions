using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : TooltipButton,IDropHandler,IBeginDragHandler,IDragHandler
{
    public ItemInventory itemInventory;
    public GameObject triggerOnClick;
    private Item item;

    public bool container = false;

    public ItemType onlyContained;

    [Header("Colors")]
    static public Color commonColor = Color.white;
    static public Color rareColor = Color.blue;
    static public Color uniqueColor = Color.magenta;
    static public Color legendaryColor = Color.yellow;

    public Image image;

    public GameObject plusC;

    public Item CarriedItem{
        get {
            return item;
        }
        set {
            if (value != null)
            {
                item = value;
                image.sprite = item.sprite;
                plusC.SetActive(item.quality==Quality.C);
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
                plusC.SetActive(false);
                image.sprite = null;
                GetComponent<Image>().color = Color.white;
            }
            //todo:set sprite etc.
        }
    }

    public override void OnPointerEnter(PointerEventData data)
    {
        ShowTooltip(CarriedItem);
    }

    public void DoubleClick() {
        LevelPreaparationInventory.instance.EquipItem(item);
    }

    private float lastClick=0;

    public void Click()
    {
        if (triggerOnClick != null) {
            Debug.Log("Message sent");
            triggerOnClick.SendMessage("ButtonClicked",this,SendMessageOptions.RequireReceiver);
        }
        if(container && itemInventory != null){
            CarriedItem = null;
            itemInventory.ItemRemoved(this,CarriedItem);
        }
        if (Time.time - lastClick < 0.5f)
        {
            lastClick = 0;
            DoubleClick();
        }
        else
        {
            lastClick = Time.time;
        }
     }

    public GameObject dragedItemObject;

    Vector2 defaultSize;
    void Start() {
        defaultSize = GetComponent<RectTransform>().sizeDelta;
    }
    public void OnDrop(PointerEventData eventData)
    {
        SetItem(DragedItem.dragedItem);
    }

    public void SetItem(Item item) {
        if (item != null && (onlyContained == ItemType.none || onlyContained == item.itemType) && container)
        {
            itemInventory.ItemAdded(this, item);
            CarriedItem = item;
            ClickEffect();
        }
    }

    public void ClickEffect() {
        LeanTween.size(GetComponent<RectTransform>(), defaultSize * 1.1f, 0.125f).setFrom(defaultSize).setRepeat(2).setLoopPingPong();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("drag");
        if (CarriedItem!=null && dragedItemObject != null)
        {
            GameObject draged = (GameObject)Instantiate(dragedItemObject, transform.position, transform.rotation, transform.root);
            draged.GetComponent<DragedItem>().CarriedItem = CarriedItem;
            if (container) {
                CarriedItem = null;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    //todo: Tooltip
}
