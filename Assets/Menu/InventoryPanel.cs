using System.Collections;
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

    public bool itemPanel = true;

    private List<Item> items;
    public List<Item> Items {
        get {
            return items;
        }
        set {
            items = value;
            Refresh();
        }
    }

    private List<EnemyProperties> enemies;
    public List<EnemyProperties> Enemies
    {
        get
        {
            return enemies;
        }
        set
        {
            enemies = value;
            Refresh();
        }
    }


    private List<GameObject> buttons = new List<GameObject>();

    private void SetButton(GameObject button, int index) {
        if (itemPanel)
        {
            InventoryButton buttonComp = button.GetComponent<InventoryButton>();
            if (itemInventory != null) buttonComp.itemInventory = itemInventory;
            buttonComp.CarriedItem = Items[index];
        }
        else
        {
            EnemyButton buttonComp = button.GetComponent<EnemyButton>();
            buttonComp.CarriedEnemy = enemies[index];
 
        }
        button.SetActive(true);
    }

    public void Refresh() {
        int i = 1;
        for (i = 0; i < Mathf.Min(buttons.Count, itemPanel?Items.Count:Enemies.Count ) ; i++)
        {
            SetButton(buttons[i], i);
        }

        int width = Mathf.FloorToInt(GetComponent<RectTransform>().rect.width);
        int buttonSize = Mathf.FloorToInt(buttonPrefab.GetComponent<RectTransform>().rect.width);
        maxColumns = (width-10) / buttonSize;

        if (maxColumns > 1) delta = ((width-10) % buttonSize) / maxColumns - 1;
        else delta = 0;

        for (i = buttons.Count; i < (itemPanel ? Items.Count : Enemies.Count); i++)
        {
            GameObject button = (GameObject)Instantiate(buttonPrefab, this.transform);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                (buttonSize / 2) + (i % maxColumns) * (buttonSize + delta) + delta,
                -(buttonSize / 2) - (i / maxColumns) * (buttonSize + delta) - delta);
            SetButton(button, i);
            buttons.Add(button);
        }
        for (int j = (itemPanel ? Items.Count : Enemies.Count); j < buttons.Count; j++)
        {
            buttons[j].SetActive(false);
        }
        //if (GetComponent<RectTransform>().rect.height !=- (-(buttonSize / 2) - (i / maxColumns) * (buttonSize + delta) - delta))
        i = Mathf.Min(buttons.Count, itemPanel ? Items.Count : Enemies.Count)-1;
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (buttonSize / 2) + (i / maxColumns) * (buttonSize + delta) + delta+70); ;
    }
}