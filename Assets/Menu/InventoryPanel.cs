﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class InventoryPanel : MonoBehaviour
{
    public GameObject buttonPrefab;
    public ItemInventory itemInventory;

    private int delta;
    private int maxColumns;

    private List<WeaponItem> items;
    public List<WeaponItem> Items {
        get {
            return items;
        }
        set {
            items = value;
            Refresh();
        }
    }


    private List<GameObject> buttons=new List<GameObject>();

    public void Refresh() {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
            //ToDo: efektivne
        }

        int width = Mathf.FloorToInt(GetComponent<RectTransform>().rect.width);
        int buttonSize = Mathf.FloorToInt(buttonPrefab.GetComponent<RectTransform>().rect.width);
        maxColumns = width / buttonSize;

        if (maxColumns > 1) delta = (width % buttonSize) / maxColumns - 1;
        else delta = 0;
        int i = 0;
        foreach (Item item in items)
        {
            GameObject button = (GameObject)Instantiate(buttonPrefab, this.transform);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                (buttonSize/2)+(i % maxColumns)*(buttonSize+delta)+delta,
                -(buttonSize/2)-(i/maxColumns)*(buttonSize+delta)-delta);

            InventoryButton buttonComp = button.GetComponent<InventoryButton>();
            buttonComp.CarriedItem = item;
            buttonComp.itemInventory = itemInventory;

            buttons.Add(button);
            i++;
        }
    }
}