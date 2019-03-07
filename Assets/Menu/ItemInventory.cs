using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    public InventoryPanel weaponPanel;
    public InventoryPanel armorPanel;
    public InventoryPanel otherPanel;
    public GameObject tooltipPrefab;
    


    private PlayerProgress progress;

    private void OnEnable()
    {
        progress = MenuController.playerProgress;
        weaponPanel.Items = progress.weapons;
    }


    private GameObject tooltip;

    public void ShowTooltip(Button sender) {
        if(tooltipPrefab!=null)
            tooltip = (GameObject)Instantiate(tooltipPrefab);
    }

    public void ButtonClick(Item item) {
        MenuController.equipManager.EquipWeapon((WeaponItem)item);
    }

    public void DeleteTooltip() {
        if(tooltip!=null)
            Destroy(tooltip);
    }
}
