using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [Header("Panels")]
    public InventoryPanel weaponPanel;
    public InventoryPanel armorPanel;
    public InventoryPanel otherPanel;

    private void OnEnable()
    {
        ReloadInventory();
    }

    public virtual void ReloadInventory(){
        PlayerProgress progress = MenuController.playerProgress;
        weaponPanel.Items = progress.weapons.ConvertAll(x => (Item)x);
        armorPanel.Items = progress.armors.ConvertAll(x => (Item)x);
    }

    public virtual void ItemAdded(InventorySlot sender, Item item) { 
    }

    public virtual void ItemRemoved(InventorySlot sender, Item item) { 

    }

}
